using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPower : MonoBehaviour
{
    [System.Serializable]
    public class SuperPowerButton
    {
        public string name;
        public Button powerButton;
        public GameObject superPower;
        public Sprite powerSprite;
    }
    
    [System.Serializable]
    public class PowerIcon
    {
        public int index;
        public Button selectButton;
        public Image setPowerIcon;
        public Transform transform;

        
    }
    
    [SerializeField] CharacterPowers powersPrefb;
    [SerializeField] private Transform selecterMarker;
    [SerializeField] private PowerIcon[] powerIcons;

    [SerializeField] private SuperPowerButton[] superPowerButtons;

    private int _powerIndex;
    private Image _selectedIconImage;

    public void AssignPower(GameObject power) {
        powersPrefb.SelectPower(power, _powerIndex);
    }
    
    public void InitSuperPowerButton(SuperPowerButton superPowerButton) {
        superPowerButton.powerButton.onClick.AddListener(delegate { SelectSuperPower(superPowerButton); });
    }
    
    public void InitPowerIcon(PowerIcon powerIcon) {
        powerIcon.selectButton.onClick.AddListener(delegate { SelectIcon(powerIcon); });
    }
    
    public void SelectIcon(PowerIcon powerIcon) {
        _powerIndex = powerIcon.index;
        selecterMarker.transform.position = powerIcon.transform.position;
        _selectedIconImage = powerIcon.setPowerIcon;

    }
    
    public void SelectSuperPower(SuperPowerButton superPowerButton) {
        if(superPowerButton==null) return;
        
        powersPrefb.SelectPower(superPowerButton.superPower, _powerIndex);
        powersPrefb.SetPowerAsActive(_powerIndex, _powerIndex+1);
        if (!_selectedIconImage) return;
        _selectedIconImage.sprite = superPowerButton.powerSprite;
        _selectedIconImage.enabled = true;
    }

    

    private void Start() {
        foreach (var icon in powerIcons) {
            InitPowerIcon(icon);
        }

        foreach (var superPowerButton in superPowerButtons) {
            InitSuperPowerButton(superPowerButton);
        }
    }
}
