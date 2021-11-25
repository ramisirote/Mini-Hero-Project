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

    private ApearanceCustimiser apearanceCustimiser;

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
        apearanceCustimiser = ApearanceCustimiser.instance;
        apearanceCustimiser.OnColorPickIndexChange += OnColorIndexChange;
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
        apearanceCustimiser.ChangeSelectedColor(cells[index].image.color, selectedIndex);
    }

    public List<Color> GenerateRainbow(){
        float hueSteps = 1f/hues;
        float shadeSteps = 1f/shades;
        var colors = new List<Color>();

        float[] RGB = new float[]{1f, 0f, 0f};
        for(float adjustVal=1-shadeSteps; adjustVal>=-1+shadeSteps; adjustVal-=shadeSteps){
            var adjustColor = Color.white * adjustVal;
            adjustColor.a = 1f;

            RGB = new float[]{1f, 0f, 0f};
            for(int i=0; i<3; i++){
                var j = (i+1)%3;
                while(RGB[j] < 1){
                    colors.Add(new Color(){r=RGB[0], g=RGB[1], b=RGB[2], a=1} + adjustColor);
                    RGB[j] = Mathf.Min(RGB[j] + hueSteps, 1);
                }
                while(RGB[i] > hueSteps){
                    colors.Add(new Color(){r=RGB[0], g=RGB[1], b=RGB[2], a=1} + adjustColor);
                    RGB[i] = Mathf.Max(RGB[i] - hueSteps, 0);
                }
            }
        }
        var grayScale = 1f;
        while(grayScale >= 0){
            float r,g,b;
            r = g = b = grayScale;
            colors.Add(new Color(r,g,b,1));
            grayScale -= hueSteps/4.8f;
        }

        return colors;
    }

    private void OnColorIndexChange(object sender, int index){
        var currentSelectedColor = apearanceCustimiser.GetCurrentColor();
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
