using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CharacterStatsData
{
    public event EventHandler OnXpChange;
    public event EventHandler OnLevelChange;
    public event EventHandler OnHealthChange;
    public event EventHandler OnEnergyChange;

    public event EventHandler OnUnlockPoitnsChange;

    public event EventHandler<float> OnDamageTaken;

    private enum FieldToArrayIndex
    {
        Level, CurrentXp, XpToNextLevel,
        MaxHealth, Health, HealthRegen, HealthPerLevel,
        MaxEnergy, Energy, EnergyRegen, EnergyPerLevel,
        AttackSpeed, AttackRate, MoveSpeed, JumpForce,
        PunchDamage, UnlockPoints, ThisEnumLength
    }

    [SerializeField] private int _level;
    [SerializeField] private float _currentXp;
    [SerializeField] private float _xpToNextLevel;

    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    [field: SerializeField] public float HealthRegen { get; private set; }

    [field: SerializeField] private float _healthPerLevel = 1;

    [field: SerializeField] public float MaxEnergy { get; private set; }
    [field: SerializeField] public float Energy { get; private set; }
    [field: SerializeField] public float EnergyRegen { get; private set; }

    [field: SerializeField] private float _energyPerLevel = 1;

    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRate { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    [field: SerializeField] public float PunchDamage { get; set; }

    private float _UnlockPoints = 0;
    [SerializeField] public float UnlockPoints
    {
        get { return _UnlockPoints; }
        set
        {
            _UnlockPoints = value;
            OnUnlockPoitnsChange?.Invoke(this, EventArgs.Empty);
        }
    }


    public CharacterStatsData()
    {
        _level = 1;
        _currentXp = 0;
        _xpToNextLevel = 100;

        MaxEnergy = 100;
        MaxHealth = 100;

        Energy = MaxEnergy;
        Health = MaxHealth;

        HealthRegen = 0.01f;
        EnergyRegen = 1f;

        AttackSpeed = 1;
        AttackRate = 1;
        MoveSpeed = 30;
        JumpForce = 100f;

        PunchDamage = 5;
        UnlockPoints = 0;
    }

    public CharacterStatsData(CharacterStatsData otherStatsData)
    {
        CopyStats(otherStatsData);
    }

    public CharacterStatsData(float[] statsArray)
    {
        if (statsArray.Length < (int)FieldToArrayIndex.ThisEnumLength)
        {
            return;
        }

        _level = (int)statsArray[(int)FieldToArrayIndex.Level];
        _currentXp = statsArray[(int)FieldToArrayIndex.CurrentXp];
        _xpToNextLevel = statsArray[(int)FieldToArrayIndex.XpToNextLevel];

        MaxEnergy = statsArray[(int)FieldToArrayIndex.MaxEnergy];
        MaxHealth = statsArray[(int)FieldToArrayIndex.MaxHealth];

        Energy = statsArray[(int)FieldToArrayIndex.Energy];
        Health = statsArray[(int)FieldToArrayIndex.Health];

        HealthRegen = statsArray[(int)FieldToArrayIndex.HealthRegen];
        EnergyRegen = statsArray[(int)FieldToArrayIndex.EnergyRegen];

        AttackSpeed = statsArray[(int)FieldToArrayIndex.AttackSpeed];
        AttackRate = statsArray[(int)FieldToArrayIndex.AttackRate];
        MoveSpeed = statsArray[(int)FieldToArrayIndex.MoveSpeed];
        JumpForce = statsArray[(int)FieldToArrayIndex.JumpForce];

        PunchDamage = statsArray[(int)FieldToArrayIndex.PunchDamage];
        UnlockPoints = statsArray[(int)FieldToArrayIndex.UnlockPoints];
    }

    public float[] GetFieldsArray()
    {
        float[] fieldsArray = new float[(int)FieldToArrayIndex.ThisEnumLength];
        fieldsArray[(int)FieldToArrayIndex.Level] = _level;
        fieldsArray[(int)FieldToArrayIndex.CurrentXp] = _currentXp;
        fieldsArray[(int)FieldToArrayIndex.XpToNextLevel] = _xpToNextLevel;

        fieldsArray[(int)FieldToArrayIndex.MaxEnergy] = MaxEnergy;
        fieldsArray[(int)FieldToArrayIndex.MaxHealth] = MaxHealth;

        fieldsArray[(int)FieldToArrayIndex.Energy] = Energy;
        fieldsArray[(int)FieldToArrayIndex.Health] = Health;

        fieldsArray[(int)FieldToArrayIndex.HealthRegen] = HealthRegen;
        fieldsArray[(int)FieldToArrayIndex.EnergyRegen] = EnergyRegen;

        fieldsArray[(int)FieldToArrayIndex.AttackSpeed] = AttackSpeed;
        fieldsArray[(int)FieldToArrayIndex.AttackRate] = AttackRate;
        fieldsArray[(int)FieldToArrayIndex.MoveSpeed] = MoveSpeed;
        fieldsArray[(int)FieldToArrayIndex.JumpForce] = JumpForce;

        fieldsArray[(int)FieldToArrayIndex.PunchDamage] = PunchDamage;
        fieldsArray[(int)FieldToArrayIndex.UnlockPoints] = UnlockPoints;

        return fieldsArray;
    }

    public void CopyStats(CharacterStatsData otherStatsData)
    {
        _level = otherStatsData._level;
        _currentXp = otherStatsData._currentXp;
        _xpToNextLevel = otherStatsData._xpToNextLevel;

        MaxEnergy = otherStatsData.MaxEnergy;
        MaxHealth = otherStatsData.MaxHealth;

        Energy = otherStatsData.Energy;
        Health = otherStatsData.Health;

        HealthRegen = otherStatsData.HealthRegen;
        EnergyRegen = otherStatsData.EnergyRegen;

        AttackSpeed = otherStatsData.AttackSpeed;
        AttackRate = otherStatsData.AttackRate;
        MoveSpeed = otherStatsData.MoveSpeed;
        JumpForce = otherStatsData.JumpForce;

        PunchDamage = otherStatsData.PunchDamage;
        UnlockPoints = otherStatsData.UnlockPoints;


        OnHealthChange?.Invoke(this, EventArgs.Empty);
        OnEnergyChange?.Invoke(this, EventArgs.Empty);
        OnLevelChange?.Invoke(this, EventArgs.Empty);
        OnXpChange?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeHpBy(float amount)
    {
        if (amount < 0) OnDamageTaken?.Invoke(this, -amount);

        if (amount > 0 && Health >= MaxHealth) return;
        if (amount < 0 && Health <= 0) return;

        Health += amount;
        if (Health < 0) Health = 0;
        if (Health > MaxHealth) Health = MaxHealth;

        OnHealthChange?.Invoke(this, EventArgs.Empty);
    }

    public void RegenerateHealth(float multiplier = 1)
    {
        ChangeHpBy(HealthRegen * multiplier);
    }

    public void SetHealthToMax()
    {
        Health = MaxHealth;
        OnHealthChange?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeEnergyBy(float amount)
    {

        if (amount > 0 && Energy >= MaxEnergy) return;
        if (amount < 0 && Energy <= 0) return;

        Energy += amount;
        if (Energy < 0) Energy = 0;
        if (Energy > MaxEnergy) Energy = MaxEnergy;

        OnEnergyChange?.Invoke(this, EventArgs.Empty);
    }

    public void RegenerateEnergy(float multiplier = 1)
    {
        ChangeEnergyBy(EnergyRegen * multiplier);
    }

    public void SetEnergyToMax()
    {
        Energy = MaxEnergy;
        OnEnergyChange?.Invoke(this, EventArgs.Empty);
    }

    public void AddXp(float xpToAdd)
    {
        _currentXp += xpToAdd;
        while (_currentXp >= _xpToNextLevel)
        {
            _currentXp -= _xpToNextLevel;
            LevelUp();
        }
        OnXpChange?.Invoke(this, EventArgs.Empty);
    }

    public void LevelUp()
    {
        _level++;

        MaxHealth += _healthPerLevel;
        Health += _healthPerLevel;

        MaxEnergy += _energyPerLevel;
        Energy += _energyPerLevel;

        UnlockPoints++;

        OnLevelChange?.Invoke(this, EventArgs.Empty);
    }

    public void ReduceXp(float xpToReduce)
    {
        if (_currentXp > 0)
        {
            _currentXp = Math.Max(0, _currentXp - xpToReduce);
            OnXpChange?.Invoke(this, EventArgs.Empty);
        }
    }

    public int GetLevel()
    {
        return _level;
    }

    public float GetXpNormalized()
    {
        return _currentXp / _xpToNextLevel;
    }
}
