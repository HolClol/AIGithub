using System;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    MoveSpeed,
    WorkSpeed,
    WorkQuality,
    WorkFailChance,
    WorkAbandonChance,
    WorkBonusChance,
    WanderOffChance,
    AssistChance,
}

[Serializable] public class AiStatsClass
{
    public float MoveSpeed, WorkSpeed, WorkQuality;
    [Range(0f, 1f)] public float WorkFailChance, WorkAbandonChance, WorkBonusChance, WanderOffChance, AssistChance;

    public void TraitReconfigStats(List<ValueChanger> traits)
    {
        foreach (ValueChanger trait in traits)
        {
            switch (trait.Stats)
            {
                case Stat.MoveSpeed:
                    MoveSpeed += trait.Value;
                    break;
                case Stat.WorkSpeed:
                    WorkSpeed += trait.Value;
                    break;
                case Stat.WorkQuality:
                    WorkQuality += trait.Value;
                    break;
                case Stat.WorkFailChance:
                    WorkFailChance += trait.PercentageValue;
                    break;
                case Stat.WorkAbandonChance:
                    WorkAbandonChance += trait.PercentageValue;
                    break;
                case Stat.WorkBonusChance:
                    WorkBonusChance += trait.PercentageValue;
                    break;
                case Stat.WanderOffChance:
                    WanderOffChance += trait.PercentageValue;
                    break;
                case Stat.AssistChance:
                    AssistChance += trait.PercentageValue;
                    break;
            }    
        }
    }

    // These stats are reconfigured from base
    public void ReconfigStat(AiStatsClass basestats, float moodMulti)
    {
        // Flat Values
        MoveSpeed = basestats.MoveSpeed * moodMulti;
        WorkSpeed = basestats.WorkSpeed * moodMulti;
        WorkQuality = basestats.WorkQuality * moodMulti;

        // Percentage Values
        // => Negative
        WanderOffChance = basestats.WanderOffChance * (2f - moodMulti);
        WorkAbandonChance = basestats.WorkAbandonChance * (2f - moodMulti);
        WorkFailChance = basestats.WorkFailChance * (2f - moodMulti);
        // => Positive
        AssistChance = basestats.AssistChance * moodMulti;
        WorkBonusChance = basestats.WorkBonusChance * moodMulti;
    }

    public GeneralData GetGeneralData()
    {
        GeneralData data = new GeneralData();
        data.MoveSpeed = MoveSpeed;
        data.WanderOffChance = WanderOffChance;
        data.AssistChance = AssistChance;   

        return data;
    }

    public WorkData GetWorkData()
    {
        WorkData data = new WorkData();
        data.WorkSpeed = WorkSpeed;
        data.WorkQuality = WorkQuality;
        data.WorkFailChance = WorkFailChance;
        data.WorkAbandonChance = WorkAbandonChance;    
        data.WorkBonusChance = WorkBonusChance;

        return data;
    }
}

public class GeneralData
{
    public float MoveSpeed;
    [Range(0f, 1f)] public float WanderOffChance, AssistChance;
}

public class WorkData
{
    public float WorkSpeed, WorkQuality;
    [Range(0f, 1f)] public float WorkFailChance, WorkAbandonChance, WorkBonusChance;
}
