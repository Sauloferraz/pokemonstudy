using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable Object para criar um novo Pokémon na Database

[CreateAssetMenu(fileName = "New Pokemon", menuName = "Pokemon/Create New Pokemon")]
public class PokemonBase : ScriptableObject
{

    [Header("Text Properties")]
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;

    [Header("Sprites")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite menuSprite;

    [Header("Types")]
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [Header("Base Stats")]
    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [Header("Learnset Level-Up")]
    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    
    public Sprite MenuSprite
    {
        get { return menuSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

}


[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum PokemonStat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    
    //Not Base Stats, used only in combat
    Accuracy,
    Evasion
}


public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public class TypeChart
{
    static float[][] chart =
    {
        //                      NOR   FIR   WAT   ELE   GRA   ICE   FIG   POI   GRO   FLY   PSY   BUG   ROC   GHO   DRA    DAR   STE    FAI                            
        /*Nor   */ new float[] { 1f,  1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0f,   1f,   1f,   0.5f,   1f   },
        /*Fir   */ new float[] { 1f,  0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f,   1f,   2f,     1f   },
        /*Wat   */ new float[] { 1f,  2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f,   1f   },
        /*Elec  */ new float[] { 1f,  1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,     1f   },
        /*Gra   */ new float[] { 1f,  0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 1f,   0.5f,   1f   },
        /*Ice   */ new float[] { 1f,  0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f,   1f   },
        /*Fig   */ new float[] { 2f,  1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f,   2f,   2f,     0.5f },
        /*POI   */ new float[] { 1f,  1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   0f,     2f   },
        /*GRO   */ new float[] { 1f,  2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f,   1f,   2f,     1f   },
        /*FLY   */ new float[] { 1f,  1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f,   1f   },
        /*PSY   */ new float[] { 1f,  1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f,   0f,   0.5f,   1f   },
        /*BUG   */ new float[] { 1f,  0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 0.5f, 1f,   2f,   0.5f,   0.5f },
        /*ROC   */ new float[] { 1f,  2f,   1f,   0.5f, 2f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   0.5f,   1f   },
        /*GHO   */ new float[] { 0f,  1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f,   1f,   1f   },
        /*DRA   */ new float[] { 1f,  1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f,   0f   },
        /*DAR   */ new float[] { 1f,  1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f,   1f,   0.5f },
        /*STE   */ new float[] { 1f,  0.5f, 0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   0.5f,   2f   },
        /*FAI   */ new float[] { 1f,  0.5f, 1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   0.5f,   1f   }

    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}

