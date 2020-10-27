using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchColor : MonoBehaviour
{

    
    // [SerializeField] Image[] colorOptions;
    // public Button currentButton = null;
    //
    // private void Start() {
    //     throw new NotImplementedException();
    // }
    public SpriteHandler sh;
    public GameObject currentSelecterd = null;
    public String[] bodyParts = null;
    public int colorNum=0;
    [SerializeField] private CharacterPowers cp;

    public void ChangeColor(Image m) {
        if (currentSelecterd != null) {
            Color targetColor = m.color;
            currentSelecterd.GetComponent<Image>().color = targetColor;
            foreach (var part in bodyParts) {
                colorNum = currentSelecterd.GetComponent<ColorPickerVariables>().materialProperty;
                sh.SetColor(part, targetColor, colorNum);
            }
            
        }
    }
    
}
