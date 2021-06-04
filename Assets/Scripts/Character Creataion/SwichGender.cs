using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SwichGender : MonoBehaviour
{
    public UnityEngine.U2D.Animation.SpriteResolver[] spritesToChangeGender;
    public CharacterAppearance appearance;
    [SerializeField] private Slider slider;

    private bool _isMale = true;

    private void Start() {
        slider.value = (float)appearance.GetGenderInt();
        _isMale = !appearance.IsFemale();
    }

    public void SwitchGender(int toGender) {
        string label;
        slider.value = (float)toGender;
        CharacterAppearance.Gender gender = (CharacterAppearance.Gender)(toGender);
        appearance.SetGender(gender);
        foreach (var spriteResolver in spritesToChangeGender) {
            label = spriteResolver.GetLabel();
            if (gender == CharacterAppearance.Gender.Female) {
                if (_isMale) {
                    _isMale = false;
                    label += "f";
                }
            }
            else {
                if (!_isMale) {
                    _isMale = true;
                    label = label.Substring(0, label.Length - 1);
                }
            }
            spriteResolver.SetCategoryAndLabel(spriteResolver.GetCategory(), label);
        }
    }
}
