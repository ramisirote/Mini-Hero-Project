using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class partCustMenu : MonoBehaviour
{
    private ApearanceCustimiser apearanceCustimiser;

    public List<Refarences.EBodyParts> partIds = new List<Refarences.EBodyParts>();

    private List<string> partNames = new List<string>();

    public List<Image> colors;

    private List<colorCell> colorCells = new List<colorCell>();

    public List<int> selectedColorsRainbowIndex = new List<int>(){-1, -1, -1};

    private int selectedColorIndex = 0;

    public Button button;

    public Outline outline;

    private bool becomeSelected = false;

    private struct colorCell{
        public GameObject gameObject;
        public Image image;
        public Button button;
        public Outline outline;
        public int index;
        public bool selected;

        public void setSelected(bool val){
            selected = val;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        apearanceCustimiser = ApearanceCustimiser.instance;
        foreach(var partId in partIds){
            partNames.Add(Refarences.BodyPartEnumToString(partId));
        }
        apearanceCustimiser.onPartCatagorChange += onPartCatagorChange;
        apearanceCustimiser.OnColorSelectChange += OnColorSelectChange;
        apearanceCustimiser.OnColorPickIndexChange += OnColorIndexChange;
        var catagoryColors = apearanceCustimiser.GetCatagoryColors(Refarences.BodyPartEnumToString(partIds[0]));
        for(int i=0; i<catagoryColors.Count && i<colors.Count; i++){
            var index = i;
            var colorGameObject = colors[i].gameObject;
            var button = colorGameObject.GetComponent<Button>();
            var outline = colorGameObject.GetComponent<Outline>();
            colors[i].color = catagoryColors[i];
            colorCells.Add(new colorCell(){gameObject=colorGameObject, image=colors[i], button=button, outline=outline, index=index});
            button.onClick.AddListener(delegate {
                ColorPickButtonOnClick(index);
            });
        }
    }

    private void ColorPickButtonOnClick(int index){
        if(apearanceCustimiser.IsPartsSelected(partNames) && selectedColorIndex != index){
            ChangeColorIndexSelect(index);
        }
        else{
            selectedColorIndex = index;
            OnClickSelectPart();
        }
    }

    public void onPartCatagorChange(object sender, List<string> newCatagory){
        int outlineDistance = 0;
        if(newCatagory[0] == Refarences.BodyPartEnumToString(partIds[0])){
            outlineDistance = 4;
        }
        var effectDistance = outline.effectDistance;
        effectDistance.x = outlineDistance;
        effectDistance.y = -outlineDistance;
        outline.effectDistance = effectDistance;
    }

    public void ChangeColorIndexSelect(int index){
        Utils.SetOutlineAmount(colorCells[index].outline);
        apearanceCustimiser.ChangeSelectedCOlorIndex(index, colors[index].color);
    }

    // Update is called once per frame
    public void OnClickSelectPart(){
        apearanceCustimiser.SelectNewPartCatagory(partNames);
        ChangeColorIndexSelect(selectedColorIndex);
    }

    public void OnColorSelectChange(object sender, Tuple<Color, int, int> tuple){
        if(!apearanceCustimiser.IsPartsSelected(partNames)) return;
        Color color = tuple.Item1;
        int index = tuple.Item2;
        selectedColorsRainbowIndex[index] = tuple.Item3;
        colors[index].color = color;
    }

    private void OnColorIndexChange(object sender, int index){
        if(!apearanceCustimiser.IsPartsSelected(partNames)){
            Utils.SetOutlineAmount(colorCells[selectedColorIndex].outline, 0);
            return;
        }
        if(selectedColorIndex == index){
            return;
        }

        Utils.SetOutlineAmount(colorCells[selectedColorIndex].outline, 0);
        Utils.SetOutlineAmount(colorCells[index].outline);
        selectedColorIndex = index;
    }

    private void Update() {
        CheckButtonSelected();
        CheckColorPickButtonSelected();
    }

    private void CheckColorPickButtonSelected(){
        for(int i=0; i<colorCells.Count; i++){
            if (EventSystem.current.currentSelectedGameObject == colorCells[i].gameObject){
                if(colorCells[i].selected) continue;
                ColorPickButtonOnClick(i);
                colorCells[i].setSelected(true);
            }
            else{
                colorCells[i].setSelected(false);
            }
        }
    }

    private void CheckButtonSelected(){
        if (EventSystem.current.currentSelectedGameObject == button.gameObject){
            if(becomeSelected) return;
            becomeSelected = true;
            OnClickSelectPart();
        }
        else{
            becomeSelected = false;
        }
    }
}
