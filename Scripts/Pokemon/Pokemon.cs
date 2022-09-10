using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveEffects;

[System.Serializable]

//Classe usada para definir um Objeto Pok�mon
public class Pokemon
{

    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public int PKLevel { get { return level; } }

    int GetStat(PokemonStat stat)
    {
        int statValue = PokemonDefaultStat[stat];

        // TODO: Apply stat modifier

        int boost = StatBoosts[stat];
        var boostTier = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * boostTier[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / boostTier[-boost]);

        return statValue;
    }

    public void Init()
    {
        this.SetConditionStatus(ConditionID.None);
        DefineDefaultStats();
        HP = MaxHp;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in PKBase.LearnableMoves)
        {
            if (move.Level <= PKLevel)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }

        ResetStatBoost();
        //Status = null;
        VolatileStatus = null;
    }

    public void ResetStatBoost()
    {
        StatBoosts = new Dictionary<PokemonStat, int>()
        {
            {PokemonStat.Attack, 0 },
            {PokemonStat.Defense, 0 },
            {PokemonStat.SpAttack, 0 },
            {PokemonStat.SpDefense, 0 },
            {PokemonStat.Speed, 0 },
            {PokemonStat.Accuracy, 0 },
            {PokemonStat.Evasion, 0 },
        };
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChange = true;
    }

    void DefineDefaultStats()
    {
        PokemonDefaultStat = new Dictionary<PokemonStat, int>();
        PokemonDefaultStat.Add(PokemonStat.Attack, Mathf.FloorToInt(((2 * PKBase.Attack) * PKLevel) / 100f) + 5);
        PokemonDefaultStat.Add(PokemonStat.SpAttack, Mathf.FloorToInt(((2 * PKBase.SpAttack) * PKLevel) / 100f) + 5);
        PokemonDefaultStat.Add(PokemonStat.Defense, Mathf.FloorToInt(((2 * PKBase.Defense) * PKLevel) / 100f) + 5);
        PokemonDefaultStat.Add(PokemonStat.SpDefense, Mathf.FloorToInt(((2 * PKBase.SpDefense) * PKLevel) / 100f) + 5);
        PokemonDefaultStat.Add(PokemonStat.Speed, Mathf.FloorToInt(((2 * PKBase.Speed) * PKLevel) / 100f) + 5);

        MaxHp = Mathf.FloorToInt(((2 * PKBase.MaxHp) * PKLevel) / 100f) + PKLevel + 10;
    }

    public bool CheckPokemonType(Pokemon pokemon, ConditionID condition)
    {
        var pokemonType1 = pokemon.PKBase.Type1.ToString();
        var pokemonType2 = pokemon.PKBase.Type2.ToString();
        var conditionType = CheckConditionType(condition);

        if (((pokemonType1 == conditionType) || (pokemonType2 == conditionType)))
            return true;
        else
            return false;
    }

    public string CheckConditionType(ConditionID condition)
    {
        
        switch (condition)
        {
            case (ConditionID.Poison):
                return "Poison";

            case (ConditionID.Burn):
                return "Fire";

            case (ConditionID.Freeze):
                return "Ice";

            case (ConditionID.Paralysis):
                return "Electric";

            default: return "Typeless";
        }
    }

    public void DisplayImmuneToCondition(ConditionID condition)
    {
        var conditionType = CheckConditionType(condition);

        switch (conditionType)
        {
            case ("Poison"):
                DisplayStatChanges.Enqueue($"{PKBase.Name} is immune to poison!");
                break;

            case ("Fire"):
                DisplayStatChanges.Enqueue($"{PKBase.Name} is immune to burn!");
                break;

            case ("Ice"):
                DisplayStatChanges.Enqueue($"{PKBase.Name} is immune to freeze!");
                break;

            case ("Electric"):
                DisplayStatChanges.Enqueue($"{PKBase.Name} is immune to paralysis!");
                break;

            default:
                break;
        }
    }

    public void SetConditionStatus(ConditionID conditionID)
    {
        if (Status == null) Status = ConditionDataBase.Conditions[ConditionID.None];

        //Check if the target can be given the status condition
        if(CheckPokemonType(this, conditionID))
        {
            DisplayImmuneToCondition(conditionID);
            return;
        }
        else
        {
            //Set condition
            Status = ConditionDataBase.Conditions[conditionID];
            Status?.OnStart?.Invoke(this);

            if (Status.Name != "None")
            {
                DisplayStatChanges.Enqueue($"{this.PKBase.Name} {Status.StartMessage}");
                OnStatusChange?.Invoke();
            }
        }
    }

    public void CureStatus()
    {
        Status = ConditionDataBase.Conditions[ConditionID.None];
        OnStatusChange?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus == null) Status = ConditionDataBase.Conditions[ConditionID.None];

        //Check if the target can be given the status condition
        if (CheckPokemonType(this, conditionID))
        {
            DisplayImmuneToCondition(conditionID);
            return;
        }
        else
        {
            //Set condition
            VolatileStatus = ConditionDataBase.Conditions[conditionID];
            VolatileStatus?.OnStart?.Invoke(this);

            if (VolatileStatus.Name != "None")
            {
                DisplayStatChanges.Enqueue($"{this.PKBase.Name} {VolatileStatus.StartMessage}");
            }
        }
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public event System.Action OnStatusChange;

    public void EndTurnCheck()
    {
        Status?.TurnEndStatusCondition?.Invoke(this);
        VolatileStatus?.TurnEndStatusCondition?.Invoke(this);
    }

    public bool StartTurnCheck()
    {
        bool canPerformMove = true;

        if (Status?.TurnStartStatusCondition != null)
        {
            if (!Status.TurnStartStatusCondition(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.TurnStartStatusCondition != null)
        {
            if (!VolatileStatus.TurnStartStatusCondition(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public class DamageDetails
    {
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }
    }

    public PokemonBase PKBase { get { return _base; } }

    public Dictionary<PokemonStat, int> PokemonDefaultStat { get; private set; }

    public Dictionary<PokemonStat, int> StatBoosts { get; private set; }

    public Condition Status { get; private set; }

    public Condition VolatileStatus { get; private set; }

    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }

    public List<Move> Moves { get; set; }

    public Queue<string> DisplayStatChanges { get; private set; } = new Queue<string>();

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {

        //Modifiers Area
        float rand = Random.Range(0.85f, 1f);
        float crit = 1f;
        float STAB;

        //EffectivenessVerifier
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.PKBase.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.PKBase.Type2);

        //Critical verifier
        if (Random.value * 100f <= 6.25f)
            crit = 2f;

        //Stab verifier
        if (move.Base.Type == attacker.PKBase.Type1 || move.Base.Type == attacker.PKBase.Type2)
        {
            STAB = 1.5f;
        }
        else STAB = 1f;

        //DamageDetails
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = crit,
            Fainted = false
        };

        float a = (2 * attacker.PKLevel + 10) / 250f;
        float d;

        float attack = (move.Base.Category == MoveCategory.Physical) ? attacker.Attack : attacker.SpAttack;
        float defense = (move.Base.Category == MoveCategory.Physical) ? attacker.Defense : attacker.SpDefense;

        float modifiers = rand * STAB * type * crit;

        d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        if (move.Base.Category == MoveCategory.Status)
        {
            d = 0;
            damage = Mathf.FloorToInt(d * modifiers);
        }

        if (((move.Base.Category == MoveCategory.Physical) && (attacker.Status.Name == ("Burn"))))
        {
            damage = Mathf.FloorToInt(d / 2);
            Debug.Log("Damage reduced by burn");
            Debug.Log($"{attacker.PKBase.Name} dealt {damage} dmg");
        }
        else
        {
            Debug.Log($"{attacker.PKBase.Name} dealt {damage} dmg");
        }

        UpdateHP(damage);

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public void StatChange(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var boostedStat = statBoost.StatAffected;
            var boostValue = statBoost.EffectLevel;        

            switch (StatBoosts[boostedStat])
            {
                case 6 when boostValue > 0:
                    //Debug.Log($"{PKBase.Name} {boostedStat} won't go any higher!");
                    DisplayStatChanges.Enqueue($"{PKBase.Name} {boostedStat} won't go any higher!");
                    break;
                case -6 when boostValue < 0:
                    DisplayStatChanges.Enqueue($"{PKBase.Name} {boostedStat} won't go any lower!");
                    break;
                default:
                {
                    StatBoosts[boostedStat] = Mathf.Clamp(StatBoosts[boostedStat] + boostValue, -6, 6);

                    string oneStageChange = (boostValue >= 1 && boostValue <= 2) ? ("rose!") : ("fell!");

                    string twoStageChange = (boostValue >= 2 && boostValue <= 3) ? ("rose sharply!") : ("harshly fell!");

                    string statChangeDisplay = ((boostValue == 1 || boostValue == -1) || (boostValue == 2 && boostValue == -2)) ? (oneStageChange) : (twoStageChange);

                    if (boostValue > 0)
                        DisplayStatChanges.Enqueue($"{PKBase.Name} {boostedStat} {statChangeDisplay}");
                    else
                        DisplayStatChanges.Enqueue($"{PKBase.Name} {boostedStat} {statChangeDisplay}");
                    break;
                }
            }

            /*Consulta
             * foreach (var stat in StatBoosts) {
                Debug.Log($"{boostedStat} = {stat.Value}");
            }*/

            /*int count = 0;
            foreach (var stat in StatBoosts)
            {
                if(count == 0){
                    Debug.Log($"-------[{this.PKBase.Name}]-------");
                    }
                count++;    
                Debug.Log($"{stat.ToString()}");
                if (count == 5)
                    count = 0;
            }*/
        }
    }

    //Properties
    
    public bool HpChange { get; set; }

    public int HP { get; set; }

    public int MaxHp { get; private set; }

    public int Attack
    {
        get { return GetStat(PokemonStat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(PokemonStat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(PokemonStat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(PokemonStat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(PokemonStat.Speed); }
    }
}
