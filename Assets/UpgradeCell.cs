using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCell : MonoBehaviour
{
    [SerializeField] private Button unlockButton;
    [SerializeField] private Image abilityIcon;

    private UpgradeData upgradeData;

    private bool isLocked = true;

    private AbilityStatic.AbilityEnum _abilityId;

    private int _upgradeIndex = 0;

    private CharacterPowerManager _powersManager;

    private CharacterStatsData _statsData;

    private void Awake() {
        var playerObj = GameObject.FindWithTag("Player");
        _powersManager = playerObj.GetComponent<CharacterPowerManager>();
        _statsData = playerObj.GetComponent<CharacterStats>().GetCharacterStats();
        unlockButton.onClick.AddListener(delegate {
            if(!_powersManager.IsAbilityUnlocked(this._abilityId) || _statsData.UnlockPoints <= 0){
                    return;
            }
            _statsData.UnlockPoints--;
            this._powersManager.UpgradeAbility(_abilityId, _upgradeIndex);
            this.isLocked = false;
            unlockButton.gameObject.SetActive(false);
        });
    }

    public void SetData(UpgradeData upgradeData, bool isUnlocked, AbilityStatic.AbilityEnum abilityId, int upgradeIndex){
        this._abilityId = abilityId;
        this._upgradeIndex = upgradeIndex;
        this.abilityIcon.sprite = upgradeData.icon;
        this.upgradeData = upgradeData;
        isLocked = !isUnlocked;
        Debug.Log($"Is Upgrade Locked: {isLocked}");
        this.unlockButton.gameObject.SetActive(isLocked);
    }
}
