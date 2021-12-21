using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ColorCustimizer : MonoBehaviour
{
    public event EventHandler<Tuple<Color, int, int>> OnColorSelectChange;
    public event EventHandler<int> OnColorPickIndexChange;

    protected int currentColorIndex;

    private Color currentColor;

    public abstract Color GetCurrentColor();

    public virtual void ChangeSelectedColor(Color newColor, int selectedRainbowIndex=-1){
        currentColor = newColor;
        OnColorSelectChange?.Invoke(this, new Tuple<Color, int, int>(newColor, currentColorIndex, selectedRainbowIndex));
    }

    public virtual void ChangeSelectedColorIndex(int index, Color color){
        currentColorIndex = index;
        currentColor = color;
        OnColorPickIndexChange?.Invoke(this, index);
    }
}
