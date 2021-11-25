using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class SpriteHandler : MonoBehaviour
{
    public GameObject[] bodyParts;
    public Material mat;

    private CharacterAppearance _appearance;
    private CharacterPowerManager _powerManager;
    private SpriteRenderer[] _spriteRenderers;

    private void Start() {
        _appearance = GetComponent<AppearanceInstance>().characterBase;
        
        _spriteRenderers = new SpriteRenderer[bodyParts.Length];
        for (var i = 0; i < bodyParts.Length; i++) {
            _spriteRenderers[i] = bodyParts[i].GetComponent<SpriteRenderer>();
        }

        _powerManager = GetComponent<CharacterPowerManager>();
        SpriteRenderer sp;
        UnityEngine.U2D.Animation.SpriteResolver sr;
        Material m;
        string label;
        string catagory;
        
        for (var i = 0; i < bodyParts.Length; i++) {
            _spriteRenderers[i].material = mat;
            m = _spriteRenderers[i].material;
            for (var j = 0; j < 3; j++) {
                m.SetColor("_Color"+(j+1), _appearance.GetColor(i, j));
            }

            sr = bodyParts[i].GetComponent<UnityEngine.U2D.Animation.SpriteResolver>();
            catagory = sr.GetCategory();
            
            label = (_appearance.GetSelectedPart(catagory) + 1).ToString();
            if (catagory == "Chest" && _appearance.IsFemale()) {
                label += "f";
            }
            sr.SetCategoryAndLabel(catagory, label);
        }
    }

    public void ColorizeAllSprites(Color color) {
        foreach (var part in _spriteRenderers) {
            if (part!=null) {
                part.color = color;
            }
        }
    }

    public void SetColor(string partName, Color color, int subColorIndex) {
        if (partName != "Power1" && partName != "Power2") {
            int partNum = _appearance.bodyPartKeys[partName];
            SpriteRenderer sp = _spriteRenderers[partNum];
            Material m = sp.material;
            m.SetColor("_Color"+(subColorIndex+1), color);
        }
        else if(_powerManager) {
            _powerManager.SetSubColor(color, partName == "Power1", subColorIndex);
        }
        _appearance.SetColor(partName, subColorIndex, color);
    }
}
