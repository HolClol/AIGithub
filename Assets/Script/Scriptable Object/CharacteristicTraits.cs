using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class ValueChanger
{
    public Stat Stats;
    public float Value;
    [Range(-1f, 1f)] public float PercentageValue;
}

[CreateAssetMenu(fileName = "CharacteristicTrait", menuName = "Scriptable Object/Characteristic Traits", order = 0)]
public class CharacteristicTraits : ScriptableObject
{
    public string TraitName;
    public List<ValueChanger> Stats;
    public CharacteristicTraits OppositeTrait; //This is set so if the game random the opposite trait it will reroll
}
