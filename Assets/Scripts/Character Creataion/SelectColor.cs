using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectColor : MonoBehaviour
{
    public SwitchColor colorController;
    public string[] bodyParts;

    private void Start() {
        InitColor[] ic = GetComponentsInChildren<InitColor>();
        foreach (var i in ic) {
            foreach (var part in bodyParts) { 
                i.Init(part);
            }
        }
    }

    public void pickThisColorToSet(GameObject o) {
        colorController.currentSelecterd = o;
        colorController.bodyParts = bodyParts;
        o.GetComponent<SelecterThing>().MoveSelectToMe();
    }
}
