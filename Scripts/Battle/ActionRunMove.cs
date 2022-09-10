using System.Collections;
using UnityEngine;

namespace Battle
{
    public class ActionRunMove : State
    {
        public ActionRunMove(BattleSystem battleSystem) : base(battleSystem)
        {
            
        }

        public override IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move moveUsed)
        {
            
            
            
            
            yield break;
        }
    }
}