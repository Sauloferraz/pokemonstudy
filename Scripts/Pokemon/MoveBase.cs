using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable Object para criar uma novo Move na Database

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create New Move")]
public class MoveBase : ScriptableObject
{
    [Header("Text Properties")]
    [SerializeField] string name;
    [TextArea] [SerializeField] string description;
    [SerializeField] PokemonType type;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveTarget target;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] MoveEffects effects;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }
    public int PP
    {
        get { return pp; }
    }

    public bool AlwaysHits { get { return alwaysHits; } }

    public MoveTarget Target
    {
        get { return target;  }
    }

    public MoveEffects Effects
    {
        get { return effects;  }
    }

    public virtual void Execute()
    {
        
    }

}

public enum MoveCategory
{
    Status,
    Physical,
    Special
}

public enum MoveTarget
{
    Enemy,
    Self
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> StatEffect;
    [SerializeField] ConditionID StatusConditionEffect;
    [SerializeField] ConditionID VolatileConditionEffect;

    public List<StatBoost> Boosts { get { return StatEffect; } }

    public ConditionID StatusCondition { get { return StatusConditionEffect; } }

    public ConditionID VolatileCondition { get { return VolatileConditionEffect; } } 
}

[System.Serializable]
public class StatBoost
{
    public PokemonStat StatAffected;
    public int EffectLevel;
}
