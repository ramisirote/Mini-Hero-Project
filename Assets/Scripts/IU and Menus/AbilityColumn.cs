using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityColumn : MonoBehaviour
{
    [SerializeField] private Button unlockButton;
    [SerializeField] private Image abilityIcon;
    
    [SerializeField] private List<UpgradeCell> upgradeCells;

    private AbilityStatic abilityData;

    private bool isLocked = true;
    
    private AbilityStatic.AbilityEnum _abilityId;

    private CharacterPowerManager _powersManager;

    private CharacterStatsData _statsData;

    // Start is called before the first frame update
    void Awake()
    {
        var playerObj = GameObject.FindWithTag("Player");
        _powersManager = playerObj.GetComponent<CharacterPowerManager>();
        _statsData = playerObj.GetComponent<CharacterStats>().GetCharacterStats();
        unlockButton.onClick.AddListener(delegate {
            if(_statsData.UnlockPoints <= 0){
                return;
            }
            _statsData.UnlockPoints--;
            _powersManager.UnlockAbility(_abilityId);
            isLocked = false;
            unlockButton.gameObject.SetActive(false);
        });
    }

    public void SetData(AbilityState abilityState){
        this._abilityId = abilityState.id;
        this.abilityData = abilityState.abilityData;
        this.isLocked = abilityState.unlocked;
        this.abilityIcon.sprite = abilityState.abilityData.icon;
        this.unlockButton.gameObject.SetActive(!this.isLocked);
        for(var i=0; i < upgradeCells.Count; i++){
            upgradeCells[i].SetData(this.abilityData.upgradeData[i], abilityState.upgradeStatus[i], this._abilityId, i);
        }
    }
    
}
