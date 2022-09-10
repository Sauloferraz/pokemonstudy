using System.Collections;
using UnityEngine;

namespace Battle
{
    public class SetupBattle : State
    {
        public SetupBattle(BattleSystem battleSystem) : base(battleSystem)
        {
            
        }

        public override IEnumerator StartBattle()
        {
            Debug.Log("ai pai para");
            BattleSystem.playerUnit.Setup(BattleSystem.playerParty.GetFirstPokemon());
            BattleSystem.enemyUnit.Setup(BattleSystem.wildPokemon);

            BattleSystem.partyScreen.Init();

            BattleSystem.dialogBox.SetMoveNames(BattleSystem.playerUnit.Pokemon.Moves);

            yield return BattleSystem.StartCoroutine(BattleSystem.dialogBox.TypeDialog($"A wild {BattleSystem.enemyUnit.Pokemon.PKBase.Name} appeared!"));
            yield return new WaitForSeconds(0.7f);
            yield return BattleSystem.StartCoroutine(BattleSystem.dialogBox.TypeDialog($"Saulo sent out {BattleSystem.playerUnit.Pokemon.PKBase.Name}!"));

            BattleSystem.ActionSelection();
        }
    }
}