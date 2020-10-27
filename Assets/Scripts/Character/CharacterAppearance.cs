using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

[CreateAssetMenu(fileName = "CharacterAppearance", menuName = "CharacterAppearance/CharacterAppearance", order = 1)]
[System.Serializable]
public class CharacterAppearance : ScriptableObject
{
    [SerializeField]private string prefabName;
    [SerializeField]private BodyPartApearance[] bodyParts;
    [SerializeField]private Gender gender = Gender.Male;
    
    // Important! Keys must match categories in the spriteResolverLibrary!!!!
    public Dictionary<string, int> bodyPartKeys = new Dictionary<string, int>() {
        {"Head", 0}, {"Chest", 1}, {"ArmL", 2}, {"ArmR", 3}, {"Waist", 4}, {"LegL", 5},
        {"LegR", 6}, {"Cape", 7}, {"Hair", 8}, {"Logo", 9}, {"Face", 10}, {"HairBack", 11},
        {"Power1", 12}, {"Power2", 13}, {"Power3", 14}
    };

    [System.Serializable]
    public struct BodyPartApearance
    {
        public string partName;
        public Color color1;
        public Color color2;
        public Color color3;
        public int selected;

        public float[] SerializeColors() {
            float[] s = {
                color1.r, color1.g, color1.b, color1.a,
                color2.r, color2.g, color2.b, color2.a,
                color3.r, color3.g, color3.b, color3.a
            };

            return s;
        }

        public void DeserializeColors(float[] s) {
            color1 = new Color(s[0], s[1], s[2], s[3]);
            color2 = new Color(s[4], s[5], s[6], s[7]);
            color3 = new Color(s[8], s[9], s[10], s[11]);
        }
    }
    
    [System.Serializable]
    public enum Gender
    {
        Male, Female
    }
    

    public void CopyFromOther(CharacterAppearance other) {
        prefabName = other.prefabName;
        bodyParts = other.bodyParts;
        gender = other.gender;
    }

    
    public bool IsFemale() {
        return gender == Gender.Female;
    }

    public void SetGender(Gender newGender) {
        gender = newGender;
    }

    public int GetGenderInt() {
        return (int) gender;
    }

    public void SelectPart(int partNum, int selectionNum) {
        bodyParts[partNum].selected = selectionNum;
    }
    
    public void SelectPart(string part, int selectionNum) {
        bodyParts[bodyPartKeys[part]].selected = selectionNum;
    }

    public int GetSelectedPart(int partNum) {
        return bodyParts[partNum].selected;
    }
    
    public int GetSelectedPart(string part) {
        return bodyParts[bodyPartKeys[part]].selected;
    }

    public void SetColor(string part, int subColor, Color c) {
        switch (subColor) {
            case 0: bodyParts[bodyPartKeys[part]].color1 = c; break;
            case 1: bodyParts[bodyPartKeys[part]].color2 = c; break;
            case 2: bodyParts[bodyPartKeys[part]].color3 = c; break;
            default: break;
        }
    }
    public Color GetColor(string part, int subColor) {
        switch (subColor) {
            case 0: return bodyParts[bodyPartKeys[part]].color1;
            case 1:  return bodyParts[bodyPartKeys[part]].color2; 
            case 2: return bodyParts[bodyPartKeys[part]].color3; 
            default:
                return Color.black;
        }
    }
    
    public void SetColor(int part, int subColor, Color c) {
        switch (subColor) {
            case 0: bodyParts[part].color1 = c; break;
            case 1: bodyParts[part].color2 = c; break;
            case 2: bodyParts[part].color3 = c; break;
            default: break;
        }
    }
    
    public Color GetColor(int part, int subColor) {
        switch (subColor) {
            case 0: return bodyParts[part].color1;
            case 1: return bodyParts[part].color2;
            case 2: return bodyParts[part].color3;
            default:
                return Color.black;
        }
    }
    
    public SerializedCharacterAppearance GetSerialize() {
        return new SerializedCharacterAppearance(this);
    }

    public void CopyFromSerialized(SerializedCharacterAppearance sa) {
        prefabName = sa.name;
        gender = (Gender)sa.gender;
        
        bodyParts = new BodyPartApearance[sa.partsLength];

        for (int i = 0; i < bodyParts.Length; i++) {
            bodyParts[i] = new BodyPartApearance {
                partName = sa.partsNames[i],
                color1 = new Color(sa.partsColor1Red[i], sa.partsColor1Green[i], sa.partsColor1Blue[i], sa.partsColor1Alpha[i]),
                color2 = new Color(sa.partsColor2Red[i], sa.partsColor2Green[i], sa.partsColor2Blue[i], sa.partsColor2Alpha[i]),
                color3 = new Color(sa.partsColor3Red[i], sa.partsColor3Green[i], sa.partsColor3Blue[i], sa.partsColor3Alpha[i]),
                selected = sa.partsSelected[i]
            };
        }
    }

    [System.Serializable]
    public class SerializedCharacterAppearance
    {
        public string name;
        public int partsLength;
        public int gender;

        public string[] partsNames;
        public float[] partsColor1Red;
        public float[] partsColor1Green;
        public float[] partsColor1Blue;
        public float[] partsColor1Alpha;
        public float[] partsColor2Red;
        public float[] partsColor2Green;
        public float[] partsColor2Blue;
        public float[] partsColor2Alpha;
        public float[] partsColor3Red;
        public float[] partsColor3Green;
        public float[] partsColor3Blue;
        public float[] partsColor3Alpha;
        public int[] partsSelected;

        public SerializedCharacterAppearance(CharacterAppearance other) {
            name = other.prefabName;
            
            partsLength = other.bodyParts.Length;
            partsNames = new string[partsLength];
            partsColor1Red = new float[partsLength];
            partsColor1Green = new float[partsLength];
            partsColor1Blue = new float[partsLength];
            partsColor1Alpha = new float[partsLength];
            partsColor2Red = new float[partsLength];
            partsColor2Green = new float[partsLength];
            partsColor2Blue = new float[partsLength];
            partsColor2Alpha = new float[partsLength];
            partsColor3Red = new float[partsLength];
            partsColor3Green = new float[partsLength];
            partsColor3Blue = new float[partsLength];
            partsColor3Alpha = new float[partsLength];

            partsSelected = new int[partsLength];
            
            for (var i = 0; i < partsLength; i++) {
                partsNames[i] = other.bodyParts[i].partName;
                
                partsColor1Red[i] = other.bodyParts[i].color1.r;
                partsColor1Green[i] = other.bodyParts[i].color1.g;
                partsColor1Blue[i] = other.bodyParts[i].color1.b;
                partsColor1Alpha[i] = other.bodyParts[i].color1.a;
                partsColor2Red[i] = other.bodyParts[i].color2.r;
                partsColor2Green[i] = other.bodyParts[i].color2.g;
                partsColor2Blue[i] = other.bodyParts[i].color2.b;
                partsColor2Alpha[i] = other.bodyParts[i].color2.a;
                partsColor3Red[i] = other.bodyParts[i].color3.r;
                partsColor3Green[i] = other.bodyParts[i].color3.g;
                partsColor3Blue[i] = other.bodyParts[i].color3.b;
                partsColor3Alpha[i] = other.bodyParts[i].color3.a;

                partsSelected[i] = other.bodyParts[i].selected;
            }

            gender = (int)other.gender;
        }
    }
}