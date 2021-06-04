using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;
 
namespace Completed
{
    public class SwitchPart : MonoBehaviour
    {
        public CharacterAppearance player;
        [SerializeField] BodyParts[] bodyParts;
        // [SerializeField] int[] labels;
 
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < bodyParts.Length; i++)
            {
                // Debug.Log(labels[i]);
                bodyParts[i].Init(player);
            }
        }
    }
 
    [System.Serializable]
    public class BodyParts
    {
        private CharacterAppearance _appearance;
        [SerializeField] Button button;
        [SerializeField] UnityEngine.U2D.Animation.SpriteResolver[] spriteResolver;
        public int id;
        public int numberOfOptions;

        public bool canBeFemale = false;
 
        public UnityEngine.U2D.Animation.SpriteResolver[] SpriteResolver { get => spriteResolver; }
 
        //method to init the button callback
        public void Init(CharacterAppearance appearance) {
            id = 0;
            button.onClick.AddListener(delegate { SwitchParts(numberOfOptions); });
            _appearance = appearance;
        }
 
        //method that are going to be triggered by the button, and it will switch the sprites of each resolver list.
        public void SwitchParts(int numberOfOptions) {
            string label;
            id++;
            id = id % numberOfOptions;

            label = (id + 1).ToString();
            if (canBeFemale && _appearance.IsFemale()) {
                label += "f";
            }
 
            foreach (var item in spriteResolver)
            {
                item.SetCategoryAndLabel(item.GetCategory(), label);
                _appearance.SelectPart(item.GetCategory(), id);
            }
        }
    }
}