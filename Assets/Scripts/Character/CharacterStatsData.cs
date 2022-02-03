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

    public enum StatFields
    {
        Level, CurrentXp, XpToNextLevel,
        MaxHealth, Health, HealthRegen, HealthPerLevel,
        MaxEnergy, Energy, EnergyRegen, EnergyPerLevel,
        AttackSpeed, AttackRate, Resistnce, BlockAmount, MoveSpeed, JumpForce,
        PunchDamage, UnlockPoints, ThisEnumLength
    }

    [SerializeField] private int _level;
    [SerializeField] private float _currentXp;
    [SerializeField] private float _xpToNextLevel;

    [field: SerializeField] public float MaxHealth { get; private set;}
    [field: SerializeField] public float Health { get; private set; }
    [field: SerializeField] public float HealthRegen { get; private set; }

    [field: SerializeField] private float _healthPerLevel = 1;

    [field: SerializeField] public float MaxEnergy { get; private set; }
    [field: SerializeField] public float Energy { get; private set; }
    [field: SerializeField] public float EnergyRegen { get; private set; }

    [field: SerializeField] private float _energyPerLevel = 1;

    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRate { get; private set; }
    [field: SerializeField] public float Resistnce { get; private set; }
    [field: SerializeField] public float BlockAmount { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }

    [field: SerializeField] public float PunchDamage { get; set; }

    private Dictionary<StatFields, float> addBuffs = new Dictionary<StatFields, float>();

    private Dictionary<StatFields, float> multiplyBuff = new Dictionary<StatFields, float>();

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
        Resistnce = 0f;
        MoveSpeed = 30;
        JumpForce = 100f;

        PunchDamage = 5;
        UnlockPoints = 0;
    }

    public CharacterStatsData(CharacterStatsData otherStatsData)
    {
        CopyStats(otherStatsData);
    }

    public float GetStat(StatFields statsField, bool buffed = true){
        if(buffed) return ApplyBuff(statsField);
        switch (statsField){
            case StatFields.Level: return _level;
            case StatFields.CurrentXp: return _currentXp;
            case StatFields.XpToNextLevel: return _xpToNextLevel;

            case StatFields.MaxEnergy: return MaxEnergy;
            case StatFields.MaxHealth: return MaxHealth;

            case StatFields.Energy: return Energy;
            case StatFields.Health: return Health;

            case StatFields.HealthRegen: return HealthRegen;
            case StatFields.EnergyRegen: return EnergyRegen;

            case StatFields.AttackSpeed: return AttackSpeed;
            case StatFields.AttackRate: return AttackRate;
            case StatFields.Resistnce: return Resistnce;
            case StatFields.BlockAmount: return BlockAmount;
            case StatFields.MoveSpeed: return MoveSpeed;
            case StatFields.JumpForce: return JumpForce;

            case StatFields.PunchDamage: return PunchDamage;
            case StatFields.UnlockPoints: return UnlockPoints;
            default: return 0;
        }
    }

    private void SetStat(StatFields statField, float value){
        switch (statField){
            case StatFields.Level: _level = (int)value; break;
            case StatFields.CurrentXp: _currentXp = value; break;
            case StatFields.XpToNextLevel: _xpToNextLevel = value; break;

            case StatFields.MaxEnergy: MaxEnergy = value; break;
            case StatFields.MaxHealth: MaxHealth = value; break;

            case StatFields.Energy: Energy = value; break;
            case StatFields.Health: Health = value; break;

            case StatFields.HealthRegen: HealthRegen = value; break;
            case StatFields.EnergyRegen: EnergyRegen = value; break;

            case StatFields.AttackSpeed: AttackSpeed = value; break;
            case StatFields.AttackRate: AttackRate = value; break;

            case StatFields.Resistnce: Resistnce = value; break;
            case StatFields.BlockAmount: BlockAmount = value; break;

            case StatFields.MoveSpeed: MoveSpeed = value; break;
            case StatFields.JumpForce: JumpForce = value; break;

            case StatFields.PunchDamage: PunchDamage = value; break;
            case StatFields.UnlockPoints: UnlockPoints = value; break;
        }
    }

    private void StatsFromArray(float[] statsArray){
        if (statsArray.Length < (int)StatFields.ThisEnumLength)
        {
            return;
        }

        for(int i=0; i < statsArray.Length; i++){
            SetStat((StatFields)i, statsArray[i]);
        }
    }

    public float[] GetFieldsArray()
    {
        float[] fieldsArray = new float[(int)StatFields.ThisEnumLength];
        for(int i=0; i < fieldsArray.Length; i++){
            fieldsArray[i] = GetStat((StatFields)i, false);
        }

        return fieldsArray;
    }

    public CharacterStatsData(float[] statsArray)
    {
        StatsFromArray(statsArray);
    }

    public void CopyStats(CharacterStatsData otherStatsData)
    {
        StatsFromArray(otherStatsData.GetFieldsArray());

        OnHealthChange?.Invoke(this, EventArgs.Empty);
        OnEnergyChange?.Invoke(this, EventArgs.Empty);
        OnLevelChange?.Invoke(this, EventArgs.Empty);
        OnXpChange?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(float amount, bool ignoreResistance = false){
        var resistnce = ApplyBuff(StatFields.Resistnce);
        if(ignoreResistance) ChangeHpBy(amount);
        else ChangeHpBy(amount * (1 - resistnce));
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

    private float ApplyBuff(StatFields statField){
        float stat = GetStat(statField, buffed: false);
        stat = stat + (addBuffs.ContainsKey(statField) ? addBuffs[statField] : 0);
        stat = stat * (multiplyBuff.ContainsKey(statField) ? multiplyBuff[statField] : 1);
        return stat;
    }

    public void SetMultBuff(StatFields statField, float buffAmout, bool remove = false){
        if(!multiplyBuff.ContainsKey(statField)){
            if (remove) return;
            multiplyBuff[statField] = 1;
        }
        if(remove){
            if(buffAmout == 0) buffAmout = multiplyBuff[statField];
            multiplyBuff[statField] /= buffAmout;
        } else {
            multiplyBuff[statField] *= buffAmout;
        }
    }

    public void SetAddBuff(StatFields statField, float buffAmout, bool remove = false){
        if(!addBuffs.ContainsKey(statField)){
            if (remove) return;
            addBuffs[statField] = 0;
        }
        if (remove) addBuffs[statField] -= buffAmout;
        else addBuffs[statField] += buffAmout;
    }
}
