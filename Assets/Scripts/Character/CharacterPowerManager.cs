using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/*
 * Power Instance is the mono behaviour that links up the instance of the player and the Character Powers.
 * This class creates the ability objects and inits the abilities scripts, attaching them to the player.  
 */
public class CharacterPowerManager : MonoBehaviour
{
    public static string ABILITY_SAVE_PATH = "/AbilityStateSave.sav";
    public List<AbilityState> abilities;

    [SerializeField] private int[] actives;

    [SerializeField] private PlayerManager manager;

    [SerializeField] private bool LoadData = true;

    private void Awake()
    {
        // load abilities from save
        if (LoadData){
            if(!LoadAbilities()){
                SaveAbilities();
            }
        } 
        if (this.actives == null || this.actives.Length < 2){ this.actives = new int[2]{-1, -1};}


        manager = GetComponent<PlayerManager>();

        // add the power Objects to the player, and save the abilityScripts
        // Set the active powers


        var pos = transform.position;

        foreach (var abilityState in this.abilities)
        {
            SetUpAbility(abilityState);
        }

        if (this.actives[0] <= -1){
            this.actives[0] = this.abilities.FindIndex(abilityState => abilityState.isMain && abilityState.unlocked);   
        }
        if (this.actives[1] <= -1){
            this.actives[1] = this.abilities.FindIndex(abilityState => !abilityState.isMain && abilityState.unlocked);
            if(this.actives[1] < -1){
                SetNextUnocked(this.actives[0], 1);
            }
        }

        for(var i=0; i < actives.Length; i++){
            if(actives[i] > -1){
                manager.SetAbility(abilities[actives[i]].abilityScript, i);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearPowers();
        }
    }

    public void RotateAbility(int activeSlot)
    {
        if (activeSlot < 0 || activeSlot > this.actives.Length-1)
        {
            Debug.LogError($"RotateAbility given illegal value: {activeSlot}");
            return;
        }
        SetActiveAbility(actives[activeSlot] + 1, activeSlot);
    }

    public void SetActiveAbility(AbilityStatic.AbilityEnum abilityId, int activeSlot)
    {
        var index = abilities.FindIndex(abilityState => (int)abilityState.id == (int)abilityId);
        SetActiveAbility(index, activeSlot);
    }

    private void SetActiveAbility(int abilityIndex, int activeSlot)
    {
        if (UnlocksCount() <= 2)
        {
            SwapActives();
            return;
        }
        abilities[actives[activeSlot]].abilityScript.SetAbilityOff();

        SetNextUnocked(activeSlot, abilityIndex);
        manager.SetAbility(abilities[actives[activeSlot]].abilityScript, activeSlot);
    }

    // Sets the active slot to point to the given index, or the next legal index
    private void SetNextUnocked(int targetSlot, int index)
    {
        var startingIndex = index;
        if (actives[targetSlot] == index || index < 0)
        {
            return;
        }

        while(true){
            index = index + 1 < abilities.Count ? index + 1 : 0;
            if (index == startingIndex){
                return;
            }
            if(abilities[index].unlocked && !actives.Contains(index)){
                break;
            }
        }
        actives[targetSlot] = index;
    }

    private void SwapActives()
    {
        foreach (var active in actives)
        {
            if (active > -1) abilities[active].abilityScript.SetAbilityOff();
        }
        manager.ClearPowers();

        Debug.Log(actives);
        var temp = actives[0];
        actives[0] = actives[1];
        actives[1] = temp;
        Debug.Log(actives[0]);
        Debug.Log(actives[1]);
        for(var i=0; i<actives.Length; i++)
        {
            if (actives[i] > -1) manager.SetAbility(abilities[actives[i]].abilityScript, i);
        }
    }

    private int UnlocksCount()
    {
        int unlockCount = 0;
        foreach (var abilityState in this.abilities)
        {
            if (abilityState.unlocked)
            {
                unlockCount++;
            }
        }
        return unlockCount;
    }

    private void SetUpAbility(AbilityState abilitySate)
    {
        var abilityObject = Instantiate(abilitySate.abilityGameObject, transform.position, Quaternion.identity);
        var ability = abilityObject.GetComponent<Ability>();
        abilitySate.abilityScript = ability;
        ability.Init(gameObject, abilitySate.colors, abilitySate.upgradeStatus);
    }

    public void UnlockAbility(AbilityStatic.AbilityEnum abilityEnum){
        var abilityStateIndex = abilities.FindIndex(abState => (int)abState.id == (int)abilityEnum);

        var abilityState = abilities[abilityStateIndex];
        print(abilityState.id);
        abilityState.unlocked = true;
        if(abilityState.isMain && actives[0] <= -1){
            actives[0] = abilityStateIndex;
            manager.SetAbility(abilityState.abilityScript, abilityStateIndex);
        }
        if(!abilityState.isMain && actives[1] <= -1){
            actives[1] = abilityStateIndex;
            manager.SetAbility(abilityState.abilityScript, abilityStateIndex);
        }
    }


    public void AddNewAbility(AbilityStatic newPower, Color[] colors = null, bool unlocked = false, bool isMain = false)
    {
        var newAbilityStatus = new AbilityState(newPower, colors, unlocked);
        SetUpAbility(newAbilityStatus);
        // if (abilities.Count < 2 && unlocked)
        // {
        //     actives[abilities.Count] = abilities.Count;
        //     manager.SetAbility(newAbilityStatus.abilityScript, abilities.Count);
        // }
        if(isMain && unlocked && actives[0] <= -1){
            actives[0] = abilities.Count;
            manager.SetAbility(newAbilityStatus.abilityScript, abilities.Count);
        }
        if(!isMain && unlocked && actives[1] <= -1){
            actives[1] = abilities.Count;
            manager.SetAbility(newAbilityStatus.abilityScript, abilities.Count);
        }
        // after setting active to not have to use 'Count - 1'
        abilities.Add(newAbilityStatus);
    }

    public void AddNewAbilities(List<AbilityStatic> abilities, Color[] colors = null, int unlockInt=-1, bool isMain = true){
        for(var i=0; i<abilities.Count; i++){
            AddNewAbility(abilities[i], colors, unlockInt==i, isMain);
        }
    }

    public void UpgradeAbility(AbilityStatic.AbilityEnum abilityEnum, int upgradeIndex)
    {
        var abilityState = abilities.Find(abState => (int)abState.id == (int)abilityEnum);
        abilityState.upgradeStatus[upgradeIndex] = true;
        abilityState.abilityScript.Upgrade(upgradeIndex);
    }

    public void UpgradeAbility(int upgradeIndex, int activeSlot = 0)
    {
        UpgradeAbility(abilities[actives[activeSlot]].id, upgradeIndex);
    }

    public void ClearPowers()
    {
        this.abilities.Clear();
        actives = new int[]{-1, -1};
    }

    public bool IsSetUp(){
        if (abilities.Count < 0 || GetSecondaryAbility() == null){
            return false;
        }
        return true;
    }

    public bool IsAbilityUnlocked(AbilityStatic abilityStatic)
    {
        return this.abilities.Find(abilityState => (int)abilityState.id == (int)abilityStatic.id).unlocked;
    }

    public bool IsAbilityUnlocked(AbilityStatic.AbilityEnum abilityId)
    {
        return this.abilities.Find(abilityState => (int)abilityState.id == (int)abilityId).unlocked;
    }

    public AbilityStatic[] GetMainAbilitiesData()
    {
        AbilityStatic[] mainAbilityStatics = (from abilitySate in this.abilities 
                                                where abilitySate.isMain 
                                                select abilitySate.abilityData).ToArray();
        return (AbilityStatic[])mainAbilityStatics;
    }

    public AbilityStatic GetSecondaryAbilityData()
    {
        var secondaryAbility = GetSecondaryAbility();
        if (secondaryAbility != null) return secondaryAbility.abilityData;
        return null;
    }

    public AbilityState GetSecondaryAbility(){
        return this.abilities.Find(abilitySate => !abilitySate.isMain);
    }

    public PowerClassData.PowerClasses GetMainPower(){
        var powers = (from abilityData in GetMainAbilitiesData() 
                        select abilityData.powerClass).ToArray();
        if (powers.GroupBy(p => p).Count() == 1){
            return PowerClassData.PowerClasses.Custome;
        }
        return abilities[0].abilityData.powerClass;
    }


    public void SetSubColor(Color color, bool isMain, int colorIndex)
    {
        if (isMain)
        {
            foreach (var abilitySate in this.abilities)
            {
                if (abilitySate.isMain)
                {
                    abilitySate.colors[colorIndex] = color;
                }
            }
        }
        else
        {
            this.abilities.Find(abilitySate => !abilitySate.isMain).colors[colorIndex] = color;
        }
    }

    public void SaveAbilities() {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + ABILITY_SAVE_PATH;
        try{
            using(FileStream stream = File.Create(path)){
                AbilitiesSave save = new AbilitiesSave {
                    abilities = (from ability in this.abilities select ability.GetSerializedAbilityState()).ToArray(),
                    actives = this.actives
                };
                formatter.Serialize(stream, save); 
                Debug.Log($"Abilities saved to {path}"); 
            }
        }
        catch(Exception e){
            Debug.Log($"An error acured: {e.ToString()} Closing the file");
        }
        
    }

    public bool LoadAbilities(){
        string path = Application.persistentDataPath + ABILITY_SAVE_PATH;
        if (File.Exists(path)) {
            
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            try{
                var abilitiesSave = (AbilitiesSave)formatter.Deserialize(stream);

                this.actives = abilitiesSave.actives;
                this.abilities.Clear();

                foreach (var serializedAbility in abilitiesSave.abilities)
                {
                    var savedAbility = new AbilityState(serializedAbility);
                    this.abilities.Add(savedAbility);
                }
                Debug.Log($"Abilities loaded from {path}");
            }
            catch (Exception ex){
                Debug.Log($"An error acured. Closing the file. {ex.ToString()}");
                stream.Close();
            }
            return true;
        }
        else {
            Debug.Log("Save File Does Not Exist");
            return false;
        }
    }

    private void OnDisable() {
        SaveAbilities();
    }

    [System.Serializable]
    private struct AbilitiesSave {
        public AbilityState.SerializedAbilityState[] abilities;

        public int[] actives;

    }

}

[System.Serializable]
public class AbilityState
{
    public AbilityState(AbilityStatic abilityStatic, Color[] colors = null,
                        bool unlocked = false, bool isMain = true)
    {
        this.name = abilityStatic.name;
        this.abilityData = abilityStatic;
        if (colors != null)
        {
            colors.CopyTo(this.colors, 0);
        }
        this.unlocked = unlocked;
        this.isMain = true;
        this.id = abilityStatic.id;
    }

    public AbilityState(SerializedAbilityState serializedSate){
        this.id = serializedSate.id;
        this.abilityData = AbilityManager.instance.GetAbilityStatic(serializedSate.id);
        this.upgradeStatus = serializedSate.upgradeStatus;
        this.name = serializedSate.name;
        this.unlocked = serializedSate.unlocked;
        this.colors = Utils.FloatArrayToColorArray(serializedSate.colors);
        this.isMain = serializedSate.isMain;
    }

    public string name;
    public AbilityStatic.AbilityEnum id;

    private AbilityStatic _abilityData = null;
    public AbilityStatic abilityData{
        get{
            if (this._abilityData == null){
                this._abilityData = AbilityManager.instance.GetAbilityStatic(id);
            }
            return this._abilityData;
        }
        set{
            this._abilityData = value;
        }
    }
    public bool unlocked = false;
    public bool[] upgradeStatus = new bool[4];
    public Color[] colors = new Color[3];
    private Ability _abilityScript = null;
    public bool isMain = true;

    public Ability abilityScript
    {
        set
        {
            this._abilityScript = value;
        }
        get
        {
            return this._abilityScript;
        }
    }

    public GameObject abilityGameObject
    {
        get
        {
            return this.abilityData.abilityObject;
        }
    }

    public SerializedAbilityState GetSerializedAbilityState(){
        return new SerializedAbilityState{
            name = this.name,
            id = this.id,
            unlocked = this.unlocked,
            upgradeStatus = this.upgradeStatus,
            colors = Utils.ColorsArrayToFloatArray(this.colors),
            isMain = this.isMain
        };
    }

    [System.Serializable]
    public struct SerializedAbilityState
    {
        public string name;
        public AbilityStatic.AbilityEnum id;

        public bool unlocked; 

        public bool[] upgradeStatus;

        public float[] colors;

        public bool isMain;
    }
}
