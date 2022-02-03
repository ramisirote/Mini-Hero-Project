using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class partCustMenu : ColorSelect
{
    private ApearanceCustimiser apearanceCustimiser;

    public List<Refarences.EBodyParts> partIds = new List<Refarences.EBodyParts>();

    private List<string> partNames = new List<string>();

    // private List<colorCell> colorCells = new List<colorCell>();

    public Button button;

    public Outline outline;

    private bool becomeSelected = false;


    // Start is called before the first frame update
    void Start()
    {
        apearanceCustimiser = ApearanceCustimiser.instance;
        foreach(var partId in partIds){
            partNames.Add(Refarences.BodyPartEnumToString(partId));
            print(Refarences.BodyPartEnumToString(partId));
        }
        apearanceCustimiser.onPartCatagorChange += onPartCatagorChange;
        apreance = true;
        base.init();
    }

    public void onPartCatagorChange(object sender, List<string> newCatagory){
        if(outline == null) return;
        int outlineDistance = 0;
        if(newCatagory[0] == Refarences.BodyPartEnumToString(partIds[0])){
            outlineDistance = 4;
        }
        var effectDistance = outline.effectDistance;
        effectDistance.x = outlineDistance;
        effectDistance.y = -outlineDistance;
        outline.effectDistance = effectDistance;
    }

    public override void OnClickSelectCatagory(){
        apearanceCustimiser.SelectNewPartCatagory(partNames);
        base.OnClickSelectCatagory();
    }

    private void Update() {
        CheckButtonSelected();
    }

    private void CheckButtonSelected(){
        if (button != null && EventSystem.current.currentSelectedGameObject == button.gameObject){
            if(becomeSelected) return;
            becomeSelected = true;
            OnClickSelectCatagory();
        }
        else{
            becomeSelected = false;
        }
    }

    public override void OnColorSelectChange(object sender, Tuple<Color, int, int> tuple){
        base.OnColorSelectChange(sender, tuple);
        if(apearanceCustimiser.GetSelectedCatagory()[0]=="Colors"){
            SetColor(tuple);
        }
    }

    protected override List<Color> GetDefaultColors()
    {
        return apearanceCustimiser.GetCatagoryColors(Refarences.BodyPartEnumToString(partIds[0]));
    }

    protected override bool IsCatagorySelected()
    {
        return apearanceCustimiser.IsPartsSelected(partNames);
    }
}
