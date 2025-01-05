using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseSkill
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int CurrentLevel { get; set; }
    public int MaxLevel { get; set; }
    public float CostMultiplier { get; set; }

    public BaseSkill(string name, string description, float costMultiplier, int maxLevel = 20)
    {
        Name = name;
        Description = description;
        CostMultiplier = costMultiplier;
        MaxLevel = maxLevel;
        CurrentLevel = 0;
    }

    public int GetUpgradeCost()
    {
        return Mathf.CeilToInt((CurrentLevel + 1) * CostMultiplier);
    }

    public virtual void Upgrade()
    {
        if (CurrentLevel < MaxLevel)
        {
            CurrentLevel++;
        }
    }

    public abstract void ApplyEffect();
}

public class FireRateSkill : BaseSkill
{
    public float FireRateBonus { get; private set; }

    public FireRateSkill() : base("Fire Rate", "Increases the rate of fire for towers.", 1.8f) { }

    public override void ApplyEffect()
    {
        FireRateBonus = CurrentLevel * 0.1f; // Example: Each level adds a 10% bonus
        Debug.Log($"Fire rate increased by {FireRateBonus * 100}%.");
    }
    public float getEffect(){
        return FireRateBonus;
    }
}

public class MobSlowingSkill : BaseSkill
{
    public float SlowEffectDuration { get; private set; }

    public MobSlowingSkill() : base("Mob Slowing", "Increases the duration of slowing effects on enemies.", 1.8f) { }

    public override void ApplyEffect()
    {
        SlowEffectDuration = 1-(CurrentLevel * 0.03f);
        Debug.Log($"Slow effect duration increased to {SlowEffectDuration} seconds.");
    }
    public float getEffect(){
        return SlowEffectDuration;
    }
}

public class GoldEarnSkill : BaseSkill
{
    public float GoldMultiplier { get; private set; }

    public GoldEarnSkill() : base("Gold Earn", "Increases the gold earned per wave.", 3.0f) { }

    public override void ApplyEffect()
    {
        GoldMultiplier = 1 + (CurrentLevel * 0.005f); //Each level adds a 0.5% bonus
        Debug.Log($"Gold multiplier is now {GoldMultiplier}x.");
    }

    public float getEffect(){
        return GoldMultiplier;
    }
}

public class XpBoostSkill : BaseSkill
{
    public float XpMultiplier { get; private set; }

    public XpBoostSkill() : base("XP Boost", "Increases the XP earned per wave.", 4.0f) { }

    public override void ApplyEffect()
    {
        XpMultiplier = 1 + (CurrentLevel * 0.005f); // Example: Each level adds a 0.5% interest
        Debug.Log($"XP multiplier is now {XpMultiplier}x.");
    }
    public float getEffect(){
        return XpMultiplier;
    }
}

public class RegenPerWaveSkill : BaseSkill
{
    public float RegenAmount { get; private set; }

    public RegenPerWaveSkill() : base("Regen Per Wave", "Restores a percentage of HP after each wave.", 1.6f) { }

    public override void ApplyEffect()
    {
        RegenAmount = 1 + (CurrentLevel * 0.025f);
        Debug.Log($"Regen amount per wave is now {RegenAmount}%.");
    }
    public float getEffect(){
        return RegenAmount;
    }
}

public class TowerDamageSkill : BaseSkill
{
    public float DamageBonus { get; private set; }

    public TowerDamageSkill() : base("Tower Damage", "Increases tower damage.", 2.5f) { }

    public override void ApplyEffect()
    {
        DamageBonus = 1+(CurrentLevel * 0.015f);
        Debug.Log($"Tower damage increased by {DamageBonus}.");
    }
    public float getEffect()
    {
        return DamageBonus;
    }
}

public abstract class BaseActiveSkill : BaseSkill
{
    public float Cooldown { get; private set; }
    public bool IsAvailable { get; private set; } = true;

    public BaseActiveSkill(string name, string description, float costMultiplier, float cooldown, int maxLevel = 20)
        : base(name, description, costMultiplier, maxLevel)
    {
        Cooldown = cooldown;
    }

    public virtual void Activate()
    {
        if (IsAvailable)
        {
            IsAvailable = false;
            ApplyEffect();
            Debug.Log($"{Name} activated!");
            GameManager.Instance.StartCoroutine(CooldownRoutine());
        }
        else
        {
            Debug.Log($"{Name} is on cooldown.");
        }
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(Cooldown);
        IsAvailable = true;
        Debug.Log($"{Name} is available again!");
    }

    public override void ApplyEffect()
    {
        Debug.Log($"Effect of {Name} applied.");
    }
}

public class StopTimeSkill : BaseActiveSkill
{
    public float Duration { get; private set; }
    public bool IsActive { get; private set;} = false;

    public StopTimeSkill() : base("Stop Time", "Temporarily stops all enemies for a duration.", 4.0f, 40f) { }

    public override void ApplyEffect()
    {
        Duration = CurrentLevel * 0.4f; // Example: Each level adds 0.4 seconds to the duration
        IsActive = true;
        Debug.Log($"Stopping time for {Duration} seconds.");
        GameManager.Instance.StartCoroutine(StopTimeRoutine(Duration));
    }

    private IEnumerator StopTimeRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsActive = false;
        Debug.Log("Time resumed.");
    }
}

public class SkillTree : MonoBehaviour
{
    public List<BaseSkill> PassiveSkills;
    public List<BaseActiveSkill> ActiveSkills;
    public UI ui;
    public static SkillTree Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        PassiveSkills = new List<BaseSkill>
        {
            new FireRateSkill(),
            new MobSlowingSkill(),
            new GoldEarnSkill(),
            new XpBoostSkill(),
            new RegenPerWaveSkill(),
            new TowerDamageSkill()
        };

        ActiveSkills = new List<BaseActiveSkill>
        {
            new StopTimeSkill()
            //new MeteorShowerSkill()
        };

        foreach (var skill in PassiveSkills)
        {
            skill.ApplyEffect();
        }

        foreach (var skill in ActiveSkills)
        {
            Debug.Log($"Active skill {skill.Name} is ready.");
        }
        ui.UpdateSkillTreeUI(PassiveSkills, ActiveSkills); // Initial UI update
    }

    public T GetSkill<T>() where T : BaseSkill
    {
        return PassiveSkills.Find(skill => skill is T) as T;
    }

    public T GetActiveSkill<T>() where T : BaseSkill
    {
        return ActiveSkills.Find(skill => skill is T) as T;
    }

    public void LevelUpPassiveSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < PassiveSkills.Count)
        {
            var skill = PassiveSkills[skillIndex];
            if (skill.CurrentLevel < skill.MaxLevel && GameManager.Instance.experience >= skill.GetUpgradeCost())
            {
                GameManager.Instance.experience -= skill.GetUpgradeCost();
                skill.Upgrade();
                skill.ApplyEffect();
                ui.UpdateSkillTreeUI(PassiveSkills, ActiveSkills); // Notify UI
                ui.ShowToastMessage($"{skill.Name} upgraded to Level {skill.CurrentLevel}");
            }
            else
            {
                ui.ShowToastMessage("Not enough XP or max level reached.");
            }
        }
    }

        public void LevelUpActiveSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < ActiveSkills.Count)
        {
            var skill = ActiveSkills[skillIndex];
            if (skill.CurrentLevel < skill.MaxLevel && GameManager.Instance.experience >= skill.GetUpgradeCost())
            {
                GameManager.Instance.experience -= skill.GetUpgradeCost();
                skill.Upgrade();
                skill.ApplyEffect();
                ui.UpdateSkillTreeUI(PassiveSkills, ActiveSkills); // Notify UI
                ui.ShowToastMessage($"{skill.Name} upgraded to Level {skill.CurrentLevel}");
            }
            else
            {
                ui.ShowToastMessage("Not enough XP or max level reached.");
            }
        }
    }

    public void ActivateSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < ActiveSkills.Count)
        {
            var skill = ActiveSkills[skillIndex];
            if (skill.IsAvailable)
            {
                skill.Activate();
                StartCoroutine(UpdateCooldownUI(skill));
            }
            else
            {
                ui.ShowToastMessage($"{skill.Name} is on cooldown.");
            }
        }
    }

    private IEnumerator UpdateCooldownUI(BaseActiveSkill skill)
    {
        float remainingCooldown = skill.Cooldown;
        while (remainingCooldown > 0)
        {
            remainingCooldown -= Time.deltaTime;
            ui.UpdateActiveSkillCooldown(skill.Name, remainingCooldown);
            yield return null;
        }
        ui.UpdateActiveSkillCooldown(skill.Name, 0);
    }
}


