using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static Gradient CreateGradient(Color[] colors, float[] alphas, float[] colorTimes = null, float[] alphaTimes = null) {
        Gradient newGrad = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        
        // Color keys
        colorKey = new GradientColorKey[colors.Length];
        for(int i=0; i<colorKey.Length; i++) {
            colorKey[i].color = colors[i];
        }

        if (colorTimes != null && colorTimes.Length == colors.Length) {
            for (int i = 0; i < colorKey.Length; i++) {
                colorKey[i].time = colorTimes[i];
            }
        }
        else {
            for (int i = 0; i < colorKey.Length; i++) {
                colorKey[i].time = (float)i/(colorKey.Length-1);
            }
        }
        
        
        // Alpha keys
        alphaKey = new GradientAlphaKey[alphas.Length];
        for(int i=0; i<alphaKey.Length; i++) {
            alphaKey[i].alpha = alphas[i];
        }

        if (alphaTimes != null && alphaTimes.Length == alphas.Length) {
            for (int i = 0; i < alphaKey.Length; i++) {
                alphaKey[i].time = alphaTimes[i];
            }
        }
        else {
            for (int i = 0; i < alphaKey.Length; i++) {
                alphaKey[i].time = (float)i/(alphaKey.Length-1);
            }
        }
        
        newGrad.SetKeys(colorKey, alphaKey);

        return newGrad;
    }


    public static float[] ColorsArrayToFloatArray(Color[] colors) {
        float[] floats = new float[colors.Length*4];

        for (int i = 0; i < colors.Length; i++) {
            floats[i * 4] = colors[i].r;
            floats[(i * 4) + 1] = colors[i].g;
            floats[(i * 4) + 2] = colors[i].b;
            floats[(i * 4) + 3] = colors[i].a;
        }

        return floats;
    }

    public static Color[] FloatArrayToColorArray(float[] floats) {
        int len = floats.Length / 4;
        Color[] colors = new Color[floats.Length/4];

        for (int i = 0; i < len; i++) {
            colors[i] = new Color(floats[i*4], floats[1 + i*4], floats[2+i*4], floats[3 + i*4]);
        }

        return colors;
    }
}
