using System.Collections;
using UnityEngine;

namespace Battle
{
    public abstract class State
    {

        protected BattleSystem BattleSystem;

        public State(BattleSystem battleSystem)
        {
            BattleSystem = battleSystem;
        }
        
        public virtual IEnumerator StartBattle()
        {
            yield break;
        }
        public virtual IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move moveUsed)
        {
            Debug.Log("eita pitombas2");
            yield break;
        }
        public virtual IEnumerator RunMoveEffects()
        {
            yield break;
        }
    }
    
    
    
    
    
    
}
