using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    
    
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;

    protected CharacterStatsData CharacterStatsData;
    private float _energySliderTarget;
    private float _healthSliderTarget;
    private float _xpSliderTarget;
    private int _levelTarget;
    private int _level;

    public void SetStats(CharacterStatsData setStatsData) {
        CharacterStatsData = setStatsData;
        CharacterStatsData.OnHealthChange += StatsDataOnHealthChange;
        CharacterStatsData.OnEnergyChange += StatsDataOnEnergyChange;
        CharacterStatsData.OnXpChange += LevelSystemOnXpChange;
        CharacterStatsData.OnLevelChange += LevelSystemOnLevelChange;
        _level = CharacterStatsData.GetLevel();
        _levelTarget = _level;

        SetUpSliders();
    }

    private void SetUpSliders() {
        if (energySlider) {
            energySlider.maxValue = CharacterStatsData.MaxEnergy;
            energySlider.value = CharacterStatsData.Energy;
        }
        
        if (healthSlider) {
            healthSlider.maxValue = CharacterStatsData.MaxHealth;
            healthSlider.value = CharacterStatsData.Health;
        }

        if (levelText) {
            levelText.text = _level.ToString();
        }

        if (xpSlider) {
            _xpSliderTarget = CharacterStatsData.GetXpNormalized();
            xpSlider.value = _xpSliderTarget;
        }
    }

    private void Start() {
        if (characterStats == null) {
            characterStats = GameObject.FindWithTag("Player")?.GetComponent<CharacterStats>();
            SetStats(GameObject.FindWithTag("GameController")?.GetComponent<GameControler>().GetCharacterStats());
        }
        else if (CharacterStatsData == null && characterStats!=null) {
            SetStats(characterStats.GetCharacterStats());
        }
    }

    private void LevelSystemOnXpChange(object sender, EventArgs e) {
        if (!xpSlider || CharacterStatsData == null) return;
        _xpSliderTarget = CharacterStatsData.GetXpNormalized();
    }

    private void LevelSystemOnLevelChange(object sender, EventArgs e) {
        _levelTarget = CharacterStatsData.GetLevel();
        if (_levelTarget < _level && levelText && xpSlider) {
            levelText.text = _levelTarget.ToString();
            _level = _levelTarget;
            _xpSliderTarget = CharacterStatsData.GetXpNormalized();
            xpSlider.value = _xpSliderTarget;
            SetUpSliders();
        }
    }

    private void StatsDataOnEnergyChange(object sender, EventArgs e) {
        if (!energySlider) return;
        energySlider.value = CharacterStatsData.Energy;
    }

    private void StatsDataOnHealthChange(object sender, EventArgs e) {
        if (!healthSlider) return;
        healthSlider.value = CharacterStatsData.Health;
    }

    private void Update() {
        if (xpSlider && (xpSlider.value < _xpSliderTarget || _level < _levelTarget)) {
            // Debug.Log("Target Xp: "+_xpSliderTarget+", Target Level: " +_levelTarget);
            UpdateXpSliderGradualy();
        }
    }

    private void UpdateXpSliderGradualy() {
        if (_levelTarget > _level) {
            if (xpSlider.value + Time.deltaTime > 1) {
                xpSlider.value = 0;
                _level++;
                if (levelText) {
                    levelText.text = _level.ToString();
                }
            }
            else {
                xpSlider.value += Time.deltaTime;
            }
        }
        else {
            if (xpSlider.value + Time.deltaTime >= _xpSliderTarget) {
                xpSlider.value = _xpSliderTarget;
            }
            else {
                xpSlider.value += Time.deltaTime;
            }
        }
    }
}
