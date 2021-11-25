using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D.Animation;

public class ApearanceCustimiser : MonoBehaviour
{
    public static ApearanceCustimiser instance;
    // public List<Color> currentColorPallet = new List<Color>();
    private CharacterAppearance characterAppearance;
    private SpriteHandler spriteHandler;
    private List<string> curentPartCatagories= new List<string>(){"Head"};
    private int currentColorIndex = 0;
    private Color currentColor;
    private Dictionary<string, SpriteResolver> partSprites = new Dictionary<string, SpriteResolver>();

    private Dictionary<string, int> catagorySelectedIndex = new Dictionary<string, int>();
    private Dictionary<string, List<Color>> catagoryColors = new Dictionary<string, List<Color>>();
    public event EventHandler<List<string>> onPartCatagorChange;

    public event EventHandler<Tuple<Color, int, int>> OnColorSelectChange;
    public event EventHandler<int> OnColorPickIndexChange;

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

    public Color GetCurrentColor(){
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

    public void ChangeSelectedColor(Color newColor, int selectedRainbowIndex=-1){
        currentColor = newColor;
        OnColorSelectChange?.Invoke(this, new Tuple<Color, int, int>(newColor, currentColorIndex, selectedRainbowIndex));
        foreach (var partName in curentPartCatagories)
        {
            spriteHandler.SetColor(partName, newColor, currentColorIndex);
            catagoryColors[partName][currentColorIndex] = newColor;
        }
    }

    public void ChangeSelectedCOlorIndex(int index, Color color){
        currentColorIndex = index;
        currentColor = color;
        // currentColorPallet[index] = color;
        foreach(var catagory in curentPartCatagories){
            catagoryColors[catagory][index] = color;
        }
        OnColorPickIndexChange?.Invoke(this, index);
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
