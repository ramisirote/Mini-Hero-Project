using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D.Animation;

public class ApearanceCustimiser : ColorCustimizer
{
    public static ApearanceCustimiser instance;
    // public List<Color> currentColorPallet = new List<Color>();
    private CharacterAppearance characterAppearance;
    private SpriteHandler spriteHandler;
    private List<string> curentPartCatagories= new List<string>(){"Head"};
    // private Color currentColor;
    private Dictionary<string, SpriteResolver> partSprites = new Dictionary<string, SpriteResolver>();

    private Dictionary<string, int> catagorySelectedIndex = new Dictionary<string, int>();
    private Dictionary<string, List<Color>> catagoryColors = new Dictionary<string, List<Color>>();
    public event EventHandler<List<string>> onPartCatagorChange;

    private void Awake() {
        if (instance == null){
            instance = this;
        }
        // currentColor = currentColorPallet[0];
        characterAppearance = GameControler.GetPlayerManager().GetComponent<AppearanceInstance>().characterBase;
        spriteHandler = GameControler.GetPlayerManager().GetComponent<SpriteHandler>();

        foreach(var catagory in characterAppearance.bodyPartKeys.Keys){
            int index = characterAppearance.bodyPartKeys[catagory];
            if(index >= spriteHandler.bodyParts.Length) break;
            SpriteResolver partSprite = spriteHandler.bodyParts[index].GetComponent<SpriteResolver>();
            partSprites[catagory] = partSprite;

            catagorySelectedIndex[catagory] = characterAppearance.GetSelectedPart(catagory);
            catagoryColors[catagory] = characterAppearance.GetColors(catagory);
        }
        catagorySelectedIndex["Colors"] = characterAppearance.GetSelectedPart("Logo");
        catagoryColors["Colors"] = characterAppearance.GetColors("Logo");
    }

    public int GetSelectedIndex(string catagory){
        return catagorySelectedIndex[catagory];
    }

    public List<string> GetSelectedCatagory(){
        return curentPartCatagories;
    }

    public bool IsPartsSelected(List<string> parts){
        return parts.Count == curentPartCatagories.Count && parts[0] == curentPartCatagories[0];
    }

    public List<Color> GetCatagoryColors(string catagory){
        return catagoryColors[catagory];
    }

    public override Color GetCurrentColor(){
        return GetCatagoryColors(curentPartCatagories[0])[currentColorIndex];
    }

    public void SelectNewPartCatagory(List<string> newCatagories){
        curentPartCatagories = newCatagories;
        onPartCatagorChange?.Invoke(this, newCatagories);
    }

    public void SelectNewPart(string catagory, string lable, int index){
        SpriteResolver partSprite = partSprites[catagory];
        partSprite.SetCategoryAndLabel(catagory, lable);
        if(lable.Contains("f")){
            lable = lable.Split('f')[0];
            characterAppearance.SetGender(CharacterAppearance.Gender.Female);
        }
        else{
            characterAppearance.SetGender(CharacterAppearance.Gender.Female);
        }
        characterAppearance.SelectPart(characterAppearance.bodyPartKeys[catagory], Int32.Parse(lable)-1);
        catagorySelectedIndex[catagory] = index;
    }

    public override void ChangeSelectedColor(Color newColor, int selectedRainbowIndex=-1){
        base.ChangeSelectedColor(newColor, selectedRainbowIndex);
        foreach (var partName in curentPartCatagories)
        {
            print(partName);
            if(partName == "Colors"){
                foreach(var partNameKey in catagoryColors.Keys){
                    if(partNameKey != "Colors") spriteHandler.SetColor(partNameKey, newColor, currentColorIndex);
                    catagoryColors[partNameKey][currentColorIndex] = newColor;
                }
                continue;
            }
            spriteHandler.SetColor(partName, newColor, currentColorIndex);
            catagoryColors[partName][currentColorIndex] = newColor;
        }
    }

    public override void ChangeSelectedColorIndex(int index, Color color){
        foreach(var catagory in curentPartCatagories){
            catagoryColors[catagory][index] = color;
        }
        base.ChangeSelectedColorIndex(index, color);
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
