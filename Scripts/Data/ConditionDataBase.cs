using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionID
{
    None,
    //Persistent Conditions
    Poison, 
    Burn, 
    Sleep, 
    Paralysis, 
    Freeze, 
    Frostbite,
    Drowsy, 
    BadPoison,
    Fainted,
    //Volatile Conditions
    Confusion,
    Flinch

}
public class ConditionDataBase : MonoBehaviour
{
    public static void Init()
    {
        foreach (var key in Conditions)
        {
            var conditionID = key.Key;
            var condition = key.Value;

            condition.ID = conditionID;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            // Poison Status
            ConditionID.Poison, 
            new Condition()
            {
                ConditionType = "Poison",
                Name = "Poison",
                Affliction = "Poisoned",
                StartMessage = "has been poisoned!",
                TurnEndStatusCondition = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} was hurt by poison!");
                    
                }
            }   
        },

        {
            // Burn Status
            ConditionID.Burn,
            new Condition()
            {
                ConditionType = "Fire",
                Name = "Burn",
                Affliction = "Burned",
                StartMessage = "has been burned!",
                TurnEndStatusCondition = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp);
                    pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} was hurt by its burn!");
                    //implementar redu��o do dano
                }
            }
        },
                {
            // None Status
            ConditionID.None,
            new Condition()
            {
                ConditionType = null,
                Name = "None",
                Affliction = "None",
                StartMessage = null
            }
        },                
                
                {
            // Paralyze Status
            ConditionID.Paralysis,
            new Condition()
            {
                ConditionType = "Electric",
                Name = "Paralysis",
                Affliction = "Paralyzed",
                StartMessage = "has been paralyzed!",
                TurnStartStatusCondition = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} is paralyzed! It can't move!");
                        return false;
                    }
                    
                    return true;
                }
            }
        }, 
        
        {
            // Freeze Status
            ConditionID.Fainted,
            new Condition()
            {
                Name = "Faint",
                Affliction = "Fainted",
                StartMessage = null,
            }
        },

            {
            // Freeze Status
            ConditionID.Freeze,
            new Condition()
            {
                ConditionType = "Ice",
                Name = "Freeze",
                Affliction = "Frozen",
                StartMessage = "was frozen solid!",
                TurnStartStatusCondition = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 10) <= 2)
                    {
                        pokemon.CureStatus();
                        pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} has thawed out!");
                        return true;
                    }

                    pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} is frozen solid! It can't move!");
                    return false;
                }
            }
        },

            {
            // Sleep Status
            ConditionID.Sleep,
            new Condition()
            {
                Name = "Sleep",
                Affliction = "Asleep",
                StartMessage = "has fallen asleep!",
               OnStart = (Pokemon pokemon) =>
               {
                   pokemon.StatusTime = Random.Range(1, 4);
                   Debug.Log($"vai mimir por {pokemon.StatusTime}");
               },
               TurnStartStatusCondition = (Pokemon pokemon) =>
               {
                   if(pokemon.StatusTime <= 0)
                   {
                       pokemon.CureStatus();
                       pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} woke up!");
                       return true;
                   }

                   pokemon.StatusTime--;
                   pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} is fast asleep!");
                   return false;
               }
               
            }
        },
            {
            // Confusion Status
            ConditionID.Confusion,
            new Condition()
            {
                Name = "Confusion",
                Affliction = "Confused",
                StartMessage = "became confused!",
               OnStart = (Pokemon pokemon) =>
               {
                   pokemon.VolatileStatusTime = Random.Range(2, 5);
                   Debug.Log($"vai ficar confuso por {pokemon.VolatileStatusTime}");
               },
               TurnStartStatusCondition = (Pokemon pokemon) =>
               {
                   if(pokemon.VolatileStatusTime <= 0)
                   {
                       pokemon.CureVolatileStatus();
                       pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} snapped out of its confusion!");
                       return true;
                   }
                   pokemon.VolatileStatusTime--;

                   // 66% chance do to a move
                   if(Random.Range(1, 3) >= 2)
                       return true;

                   //Hurt by confusion
                   pokemon.DisplayStatChanges.Enqueue($"{pokemon.PKBase.Name} is confused!");
                   pokemon.UpdateHP(pokemon.MaxHp / 8);
                   //pokemon.TakeDamage(BasePower 40, pokemon);
                   pokemon.DisplayStatChanges.Enqueue($"It hurt itself in its to confusion!");
                   return false;
               }

            }
        },
            {
            // Flinch Status
            ConditionID.Flinch,
            new Condition()
            {
                Name = "Flinch",
                Affliction = "Flinched",
                StartMessage = "flinched and couldn't move!",
                TurnStartStatusCondition = (Pokemon pokemon) =>
                {
                   pokemon.CureVolatileStatus();
                   return false;
                },
            }
        },

    };

}
