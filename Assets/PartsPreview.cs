using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using System;

public class PartsPreview : MonoBehaviour
{
    public float spriteScale = 5.0f;
    public GameObject childPrefab;

    public Material material;

    public Sprite emptySprite;
    public Color baseColor;
    public Color highlightColor;

    private ApearanceCustimiser apearanceCustimiser;
    private SpriteLibraryAsset spriteLibrary;
    private GridLayoutGroup grid;

    private List<string> catagoryNames = new List<string>();

    private List<child> children = new List<child>();

    private int selectedChildIndex = 0;

    private struct child{
        public GameObject gameObject;
        public Image image;

        public Button button;

        public Outline backColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        apearanceCustimiser = ApearanceCustimiser.instance;
        apearanceCustimiser.onPartCatagorChange += onPartCatagorChange;
        apearanceCustimiser.OnColorSelectChange += OnColorSelectChange;
        spriteLibrary = gameObject.GetComponent<SpriteLibrary>().spriteLibraryAsset;
        grid = gameObject.GetComponent<GridLayoutGroup>();
        for(int i=0; i<50; i++){
            var instObj = Instantiate(childPrefab, this.transform);
            var imageComp = instObj.GetComponentsInChildren<Image>()[1];
            var buttonComp = instObj.GetComponent<Button>();
            var outlineComp = instObj.GetComponent<Outline>();
            children.Add(new child{gameObject=instObj, image=imageComp, button=buttonComp, backColor=outlineComp});
            outlineComp.enabled = i==selectedChildIndex;
            imageComp.gameObject.SetActive(false);
        }
        var enumirator = spriteLibrary.GetCategoryNames().GetEnumerator();
        enumirator.MoveNext();
        enumirator.MoveNext();
        catagoryNames = new List<string>(){enumirator.Current};
        updateCatagoty(catagoryNames[0]);
    }

    public void onPartCatagorChange(object sender, List<string> newCatagories){
        if(catagoryNames[0] == newCatagories[0]){
            catagoryNames = newCatagories;
            return;
        }
        catagoryNames = newCatagories;
        updateCatagoty(newCatagories[0]);
    }

    private void OnColorSelectChange(object sender, Tuple<Color, int, int> tuple){
        var newColor = tuple.Item1;
        var currentColorIndex = tuple.Item2;

        foreach(var cell in children){
            if(!cell.gameObject.activeSelf){
                break;
            }

            cell.image.material.SetColor("_Color" + (currentColorIndex + 1), newColor);
        }
    }

    public void updateCatagoty(string newCatagory){
        foreach (var child in children){
            child.gameObject.SetActive(false);
            // child.image.gameObject.SetActive(false);
        }
        var index = apearanceCustimiser.GetSelectedIndex(newCatagory);
        children[selectedChildIndex].backColor.enabled = false;
        children[index].backColor.enabled = true;
        selectedChildIndex = index;
        int i = 0;
        foreach(var lable in spriteLibrary.GetCategoryLabelNames(newCatagory)){
            var spriteBase = spriteLibrary.GetSprite(newCatagory, lable);
            var gridCellSize = grid.cellSize;
            if(spriteBase != null){
                gridCellSize.x = spriteScale * spriteBase.rect.width / spriteBase.rect.height;
                gridCellSize.y = spriteScale;
                grid.cellSize = gridCellSize;
                children[i].image.sprite = spriteBase;
            }
            else{
                children[i].image.sprite = emptySprite;
            }
            children[i].image.material = Instantiate<Material>(material);
            var catagoryColors = apearanceCustimiser.GetCatagoryColors(newCatagory);
            for (var j = 0; j < catagoryColors.Count; j++) {
                children[i].image.material.SetColor("_Color" + (j + 1), catagoryColors[j]);
            }
            OnClick(children[i], lable, i);
            children[i].gameObject.SetActive(true);
            children[i].image.gameObject.SetActive(true);
            i++;
        }
    }

    private void OnClick(child child, string lable, int index){
        child.button.onClick.RemoveAllListeners();
        child.button.onClick.AddListener(delegate {
            foreach(var catagoryName in catagoryNames){
                apearanceCustimiser.SelectNewPart(catagoryName, lable, index);
            }
            children[selectedChildIndex].backColor.enabled = false;
            children[index].backColor.enabled = true;
            selectedChildIndex = index;
        });
    }
}
