using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitColor : MonoBehaviour
{
    public CharacterAppearance appearance;

    public void Init(string part) {
        Color c = appearance.GetColor(part, this.GetComponent<ColorPickerVariables>().materialProperty);
        GetComponent<Image>().color = c;
    }

    
}
