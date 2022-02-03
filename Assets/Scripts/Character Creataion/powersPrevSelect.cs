using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class powersPrevSelect : MonoBehaviour
{

    public GameObject powerCardsPrefab;
    public GameObject abilityCardPrefab;

    public GameObject powerAbilityCard;
    public VerticalLayoutGroup grid;

    public ScrollRect scrollRect;

    public GameObject colorPicker;

    private List<GameObject> children = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        powerCustimizer.instance.onSelectTypeChange += OnSelectChanged;
        powerCustimizer.instance.OnColorPickIndexChange += ColorSelected;
        OnSelectChanged(null, powerCustimizer.SelectTypes.powers);
    }

    private void OnSelectChanged(object sender, powerCustimizer.SelectTypes selectType){
        SwitchToGrid();
        ClearGrid();
        switch (selectType){
            case powerCustimizer.SelectTypes.powers: 
                PopulatePowers();
                break;
            case powerCustimizer.SelectTypes.abilities:
                PopulateAbilities();
                break;
            case powerCustimizer.SelectTypes.powerAbilities:
                PopulatePowerAbilities();
                break;
        }
    }

    private void ClearGrid(){
        foreach(var child in children){
            Destroy(child);
        }
        children.Clear();
    }

    private void PopulatePowers(){
        var powers = AbilityManager.instance.powerClassDatas;
        float selectedIndex = 0;
        for (int i = 0; i < powers.Count; i++)
        {
            PowerClassData power = powers[i];
            var cardObj = Instantiate(powerCardsPrefab, grid.transform);
            children.Add(cardObj);
            var cardScript = cardObj.GetComponent<powerCard>();
            cardScript.init(power);
            if(cardScript.isSelected) selectedIndex = i;
        }
        
        focusGrid(1f - (selectedIndex/(powers.Count - 1f)));
    }

    private void PopulateAbilities(){
        var abilities = AbilityManager.instance.GetAbiitiesExceptClass(powerCustimizer.instance.getCurrentMainPower());
        abilities = abilities.OrderByDescending(o=>o.powerClass).ToList();
        float selectedIndex = 0;
        for (int i = 0; i < abilities.Count; i++)
        {
            AbilityStatic ability = abilities[i];
            var cardObj = Instantiate(abilityCardPrefab, grid.transform);
            children.Add(cardObj);
            var cardScript = cardObj.GetComponent<abilityCard>();
            cardScript.init(ability);
            if(cardScript.isSelected) {
                selectedIndex = i;
                print(cardScript.nameText.text);
                print(selectedIndex);
            }
        }
        focusGrid(1f - (selectedIndex/(abilities.Count - 1f)));
    }

    private void PopulatePowerAbilities(){
        var cardObj = Instantiate(powerAbilityCard, grid.transform);
        children.Add(cardObj);
        var cardScript = cardObj.GetComponent<PowerAbilityCard>();
        cardScript.init(powerCustimizer.instance.getCurrentMainPower());
    }

    private void focusGrid(float scrollAmount){
        Canvas.ForceUpdateCanvases();
        print(scrollAmount);
        scrollRect.verticalNormalizedPosition = scrollAmount;
    }

    private void ColorSelected(object sender, int index){
        SwitchToColor();
    }

    public void SwitchToColor(){
        colorPicker.SetActive(true);
        ClearGrid();
    }

    public void SwitchToGrid(){
        colorPicker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
