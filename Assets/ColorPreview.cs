using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ColorPreview : MonoBehaviour
{

    public GameObject colorPickPrefab;
    public float hues;
    public float shades;

    public bool apreance;

    private ColorCustimizer custimizer;

    private List<colorCell> cells = new List<colorCell>();

    private int selectedIndex = 0;

    private List<Color> rainbow;

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
        if(apreance){
            custimizer = ApearanceCustimiser.instance;
        } else {
            custimizer = powerCustimizer.instance;
        }
        custimizer.OnColorPickIndexChange += OnColorIndexChange;
        rainbow = GenerateRainbow();
        int i=0;
        foreach(var color in rainbow){
            var instObj = Instantiate(colorPickPrefab, this.transform);
            var imageComp = instObj.GetComponent<Image>();
            var buttonComp = instObj.GetComponent<Button>();
            var outlineComp = instObj.GetComponent<Outline>();
            var index = i;
            cells.Add(new colorCell{gameObject=instObj, image=imageComp, button=buttonComp, outline=outlineComp, index=index});
            imageComp.color = color;
            buttonComp.onClick.AddListener(delegate {
                ChangeSelectedColor(index);
            });
            i++;
        }

    }

    private void ChangeSelectedColor(int index){
        Utils.SetOutlineAmount(cells[selectedIndex].outline, 0);
        Utils.SetOutlineAmount(cells[index].outline, 5);
        selectedIndex = index;
        custimizer.ChangeSelectedColor(cells[index].image.color, selectedIndex);
    }

    public List<Color> GenerateRainbow(){
        var colors = new List<Color>();
        var shadeStep = 1f/shades;
        var satStep = 1f/(shades);
        float graySteps = 2*shades - 1;
        float grayIndex = graySteps;
        for(float s = satStep; s < 1f; s += satStep){
            colors.Add(Color.HSVToRGB(H:0, S:0, V:grayIndex/graySteps));
            grayIndex -= 1;
            for(float h=0; h <= hues; h++){
                colors.Add(Color.HSVToRGB(H: h/hues, S: s, V: 1f));
            }
        }
        for(float v=1f; v > shadeStep; v -= shadeStep){
            colors.Add(Color.HSVToRGB(H:0, S:0, V:grayIndex/graySteps));
            grayIndex -= 1;
            for(float h=0; h <= hues; h++){
                colors.Add(Color.HSVToRGB(H: h/hues, S: 1f, V: v));
            }
        }
  
        return colors;
    }

    private void OnColorIndexChange(object sender, int index){
        var currentSelectedColor = custimizer.GetCurrentColor();
        var closestIndex = GetClosestColorIndex(currentSelectedColor);
        ChangeSelectedColor(closestIndex);
    }

    private int GetClosestColorIndex(Color targetColor){
        var colorDiffs = rainbow.Select(n => ColorDiff(n, targetColor)).Min(n =>n);
        return rainbow.FindIndex(n => ColorDiff(n, targetColor) == colorDiffs);
    }

    private float ColorDiff(Color c1, Color c2){ 
        return  Mathf.Sqrt((c1.r - c2.r) * (c1.r - c2.r) + (c1.g - c2.g) * (c1.g - c2.g) + (c1.b - c2.b) * (c1.b - c2.b));
    }

    private void Update() {
        for(int i=0; i<cells.Count; i++){
            if (EventSystem.current.currentSelectedGameObject == cells[i].gameObject){
                if(cells[i].selected) continue;
                ChangeSelectedColor(i);
                cells[i].setSelected(true);
            }
            else{
                cells[i].setSelected(false);
            }
        }
    }
}
