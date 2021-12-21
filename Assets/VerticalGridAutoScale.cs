using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalGridAutoScale : MonoBehaviour
{

    VerticalLayoutGroup grid;
    RectTransform rectTransform;
    int numberOfChilred;

    bool changed = false;

    private void OnEnable() {
        grid = gameObject.GetComponent<VerticalLayoutGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        numberOfChilred = transform.childCount;
        EventManager.instance.AddListener("resetGrid", SetHight);
        SetHight();
    }

    private void OnDisable() {
        EventManager.instance.RemoveListener("resetGrid", SetHight);
    }

    private void SetHight(){
        float newHight = grid.padding.top;
        int i=0;
        foreach (RectTransform child in transform)
        {
            // if(!child.gameObject.activeSelf) continue;
            newHight += child.sizeDelta.y;
            newHight += grid.spacing;
            i++;
        }
        print(transform.childCount);
        newHight -= grid.spacing;
        newHight += grid.padding.bottom;

        if (rectTransform.sizeDelta.y != newHight){
            rectTransform.sizeDelta = Vector2.up * newHight;
        }
        // numberOfChilred = transform.childCount;
        var anchor = rectTransform.anchoredPosition;
        anchor.y = -rectTransform.sizeDelta.y/2;
        rectTransform.anchoredPosition = anchor;

        changed = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private void Update() {
        if(changed){
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            changed = false;
        }
    }
}
