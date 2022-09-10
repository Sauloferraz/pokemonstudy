using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public ConditionID ID { get; set; }

    public string Name { get; set; }
    public string Affliction { get; set; }
    public string ConditionType { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Func<Pokemon, bool> TurnStartStatusCondition { get; set; }

    public Action<Pokemon> OnStart { get; set; }

    public Action<Pokemon> TurnEndStatusCondition { get; set; }

 }
