using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ColorSelect : MonoBehaviour
{
    protected ColorCustimizer colorCustimizer;

    protected int selectedColorIndex;

    public List<Image> colors;
    protected List<colorCell> colorCells = new List<colorCell>();

    public List<int> selectedColorsRainbowIndex = new List<int>(){-1, -1, -1};

    protected bool apreance;

    protected struct colorCell{
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

    protected abstract List<Color> GetDefaultColors();


    protected void init()
    {
        if(apreance){
            colorCustimizer = ApearanceCustimiser.instance;
        } else {
            colorCustimizer = powerCustimizer.instance;
        }
        colorCustimizer.OnColorSelectChange += OnColorSelectChange;
        colorCustimizer.OnColorPickIndexChange += OnColorIndexChange;
        var catagoryColors = GetDefaultColors();
        for(int i=0; i<catagoryColors.Count && i<colors.Count; i++){
            var index = i;
            var colorGameObject = colors[i].gameObject;
            var button = colorGameObject.GetComponent<Button>();
            var outline = colorGameObject.GetComponent<Outline>();
            Utils.SetOutlineAmount(outline);
            outline.enabled = false;
            colors[i].color = catagoryColors[i];
            colorCells.Add(new colorCell(){gameObject=colorGameObject, image=colors[i], button=button, outline=outline, index=index});
            button.onClick.AddListener(delegate {
                ColorPickButtonOnClick(index);
            });
        }
    }

    protected abstract bool IsCatagorySelected();

    private void ColorPickButtonOnClick(int index){
        if(IsCatagorySelected() && selectedColorIndex != index){
            ChangeColorIndexSelect(index);
        }
        else{
            selectedColorIndex = index;
            OnClickSelectCatagory();
        }
    }

    public void ChangeColorIndexSelect(int index){
        colorCells[index].outline.enabled = true;
        colorCustimizer.ChangeSelectedColorIndex(index, colors[index].color);
    }

    public virtual void OnClickSelectCatagory(){
        ChangeColorIndexSelect(selectedColorIndex);
    }

    protected void SetColor(Tuple<Color, int, int> tuple){
        Color color = tuple.Item1;
        int index = tuple.Item2;
        selectedColorsRainbowIndex[index] = tuple.Item3;
        colors[index].color = color;
    }

    public virtual void OnColorSelectChange(object sender, Tuple<Color, int, int> tuple){
        if(!IsCatagorySelected()) return;
        SetColor(tuple);
    }

    private void OnColorIndexChange(object sender, int index){
        if(!IsCatagorySelected()){
            colorCells[selectedColorIndex].outline.enabled = false;
            return;
        }
        if(selectedColorIndex == index){
            return;
        }

        colorCells[selectedColorIndex].outline.enabled = false;
        colorCells[index].outline.enabled = true;
        selectedColorIndex = index;
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

    private void Update() {
        CheckColorPickButtonSelected();
    }
}
