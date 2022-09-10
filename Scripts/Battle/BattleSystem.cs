using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

[System.Serializable]

//Define em que estado da batalha o jogador está
public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartyScreen,
    BattleOver
}

public class BattleSystem : StateMachine
{
    #region Referencias

    [SerializeField] public BattleUnit playerUnit;
    [SerializeField] public BattleUnit enemyUnit;
    [SerializeField] public BattleDialogBox dialogBox;
    [SerializeField] public PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    #endregion

    BattleUnit unit;
    
    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;
    int RunAttempts = 0;

    public PokemonParty playerParty;
    public Pokemon wildPokemon;
    PartySprite partySprite;

    //------------------------------------------------------------------------//
    //                             BATTLE INICIALIZATION                      //
    //------------------------------------------------------------------------//
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        state = BattleState.Start;
        SetState(new SetupBattle(this));
    }

    #region OldStateMachine
    /*public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetFirstPokemon());
        enemyUnit.Setup(wildPokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.PKBase.Name} appeared!"));
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(dialogBox.TypeDialog($"Saulo sent out {playerUnit.Pokemon.PKBase.Name}!"));
        
        ActionSelection();
    }*/
    #endregion
    
    //------------------------------------------------------------------------//
    //                              ACTION SELECTION                          //
    //------------------------------------------------------------------------//
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        } else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    internal void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog($"{playerUnit.Pokemon.PKBase.Name} is waiting for your directions.");
        dialogBox.EnableActionSelector(true);
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(playerUnit.Pokemon.HP);
            Debug.Log(playerUnit.Pokemon.Attack);
            Debug.Log(playerUnit.Pokemon.SpAttack);
            Debug.Log(playerUnit.Pokemon.Defense);
            Debug.Log(playerUnit.Pokemon.SpDefense);
            Debug.Log(playerUnit.Pokemon.Defense);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Bag

            }
            else if (currentAction == 2)
            {
                //Pokemon
                OpenPartyScreen();

            }
            else if (currentAction == 3)
            {
                //Run
                print("try to run");
                StartCoroutine(RunAway());
            }
        }
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Party);
        partyScreen.gameObject.SetActive(true);
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            
            var move = playerUnit.Pokemon.Moves[currentMove];
            
            StartCoroutine(PlayerTurn());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Party.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Party[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Pok�mon!");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("Pok�mon already in battle!");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            dialogBox.EnableActionSelector(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    //------------------------------------------------------------------------//
    //                              ACTION EXECUTIONS                         //
    //------------------------------------------------------------------------//

    IEnumerator PlayerTurn()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Pokemon.Moves[currentMove];

        yield return StartCoroutine(State.RunMove(playerUnit, enemyUnit, move));
    }

    IEnumerator EnemyTurn(bool SwitchIn)
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Pokemon.GetRandomMove();

        yield return StartCoroutine(State.RunMove(enemyUnit, playerUnit, move));

        if (SwitchIn)
        {
            if (state == BattleState.PerformMove)
                ActionSelection();
        }
    }

    bool CheckPP(BattleUnit battleUnit, Move moveUsed)
    {
        if (moveUsed.PP >= 1)
        {
            return true;
        }
        else return false;
    }

    MoveCategory CheckMoveCategory(Move move)
    {
        return move.Base.Category;
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move moveUsed)
    {
        bool canMove = sourceUnit.Pokemon.StartTurnCheck();

        if (!canMove)
        {
            yield return ShowStatChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatChanges(sourceUnit.Pokemon);

        moveUsed.PP--;

        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.PKBase.Name} used {moveUsed.Base.Name}!");

        if (AccuracyCheck(moveUsed, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            switch (CheckMoveCategory(moveUsed))
            {
                case MoveCategory.Status:
                    sourceUnit.PlayStatusMoveAnimation();
                    yield return new WaitForSeconds(1f);
                    yield return RunMoveEffects(sourceUnit.Pokemon, targetUnit.Pokemon, moveUsed);
                    break;

                default:

                    var damageDetails = targetUnit.Pokemon.TakeDamage(moveUsed, sourceUnit.Pokemon);

                    yield return StartCoroutine(DamageMoveAnimation(sourceUnit, targetUnit));
                    yield return targetUnit.Hud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);

                    if (targetUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.PKBase.Name} fainted!");
                        yield return FaintAnimation(targetUnit);

                        CheckForBattleOver(targetUnit);
                        targetUnit.Pokemon.SetConditionStatus(ConditionID.Fainted);
                    }

                    if (moveUsed.Base.Effects.Boosts != null || moveUsed.Base.Effects.StatusCondition != ConditionID.None || moveUsed.Base.Effects.VolatileCondition != ConditionID.None)
                    {
                        //sourceUnit.PlayStatusMoveAnimation();
                        yield return new WaitForSeconds(1f);
                        yield return RunMoveEffects(sourceUnit.Pokemon, targetUnit.Pokemon, moveUsed);
                    }
                    break;
            }
        } 
        else yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.PKBase.Name} attack missed!");
    }

    IEnumerator DamageMoveAnimation(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        sourceUnit.PlayMoveAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.PlayHitAnimation();
    }

    IEnumerator FaintAnimation(BattleUnit battleUnit)
    {
        battleUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1f);
    }

    IEnumerator EndTurn(BattleUnit battleUnit)
    {
        battleUnit.Pokemon.EndTurnCheck();

        yield return ShowStatChanges(battleUnit.Pokemon);
        yield return battleUnit.Hud.UpdateHP();


        if (battleUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{battleUnit.Pokemon.PKBase.Name} fainted!");
            yield return FaintAnimation(battleUnit);
            CheckForBattleOver(battleUnit);
            battleUnit.Pokemon.SetConditionStatus(ConditionID.Fainted);
        }
    }

    IEnumerator RunMoveEffects(Pokemon sourceUnit, Pokemon targetUnit, Move moveUsed)
    {
        var moveEffect = moveUsed.Base.Effects;

        // Stat Change
        if (moveUsed.Base.Effects.Boosts != null)
        {
            if (moveUsed.Base.Target == MoveTarget.Self)
                sourceUnit.StatChange(moveEffect.Boosts);
            else
                targetUnit.StatChange(moveEffect.Boosts);
        }

        // Status Condition
        if ((moveUsed.Base.Effects.StatusCondition != ConditionID.None) && (moveUsed.Base.Target == MoveTarget.Enemy))
        {
            if ((targetUnit.Status.Name != null) && (targetUnit.Status.Name != moveUsed.Base.Effects.StatusCondition.ToString()))
            {
                targetUnit.SetConditionStatus(moveEffect.StatusCondition);
            }
            else yield return dialogBox.TypeDialog($"{targetUnit.PKBase.Name} is already {targetUnit.Status.Affliction}!");
        }

        else if ((moveUsed.Base.Effects.StatusCondition != ConditionID.None) && (moveUsed.Base.Target == MoveTarget.Self))
        {
            if ((sourceUnit.Status.Name != null) && (sourceUnit.Status.Name != moveUsed.Base.Effects.StatusCondition.ToString()))
            {
                sourceUnit.SetConditionStatus(moveEffect.StatusCondition);
            }
            else yield return dialogBox.TypeDialog($"{sourceUnit.PKBase.Name} is already {sourceUnit.Status.Affliction}!");
        }

        // Volatile Condition
        if (moveEffect.VolatileCondition != ConditionID.None)
        {
            targetUnit.SetVolatileStatus(moveEffect.VolatileCondition);
        }

        yield return ShowStatChanges(sourceUnit);
        yield return ShowStatChanges(targetUnit);
    }

    IEnumerator ShowDamageDetails(Pokemon.DamageDetails damageDetails)
    {
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
            yield return new WaitForSeconds(0.7f);
        }
            
        if (damageDetails.TypeEffectiveness < 1f && damageDetails.TypeEffectiveness > 0)
        {
            yield return dialogBox.TypeDialog("It's not very effective!");
            yield return new WaitForSeconds(0.7f);
        }
            
        if (damageDetails.TypeEffectiveness <= 0)
        {
            yield return dialogBox.TypeDialog("No effect.");
            yield return new WaitForSeconds(0.7f);
        }

        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("Critical hit!");
            yield return new WaitForSeconds(0.7f);
        }
    }

    IEnumerator ShowStatChanges(Pokemon pokemon)
    {
        while (pokemon.DisplayStatChanges.Count > 0)
        {
            var message = pokemon.DisplayStatChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {

        bool FaintSwitchIn = true;

        if (playerUnit.Pokemon.HP > 0)
        {
            FaintSwitchIn = false;
            yield return dialogBox.TypeDialog($"Come back, {playerUnit.Pokemon.PKBase.Name}!");
            playerUnit.Pokemon.ResetStatBoost();
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(0.7f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"Go, {newPokemon.PKBase.Name}!");

        if (FaintSwitchIn)
        {
            Debug.Log("entrou");
            yield return new WaitForSeconds(1f);

            //if (state == BattleState.PerformMove)
                ActionSelection();
        }

        else if (!FaintSwitchIn)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(EnemyTurn(true));
        }
    }

    IEnumerator RunAway()
    {
        state = BattleState.Busy;
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableActionSelector(false);

        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
        {
            yield return dialogBox.TypeDialog("Got away safely!");
            OnBattleOver(false);
        }
        else
        {
            RunAttempts += 1;
            yield return EscapeAttempt(RunAttempts);
            if (EscapeAttempt(RunAttempts))
            {
                yield return dialogBox.TypeDialog("Got away safely!");
                OnBattleOver(false);
                RunAttempts = 0;
            }
            else
            {
                yield return dialogBox.TypeDialog("Couldn't escape!");
                StartCoroutine(EnemyTurn(true));
            }
        }
    }

    bool EscapeAttempt(int attempts)
    {
        int playerSpeed = playerUnit.Pokemon.Speed * 128;
        int wildSpeed = enemyUnit.Pokemon.Speed;

        int escapeOdds = Mathf.FloorToInt(((Mathf.FloorToInt(playerSpeed / wildSpeed)) + 30 * (attempts)) % 256);
       
        Debug.Log($"Attemps = {attempts}");
        Debug.Log($"EscapeOdds = {escapeOdds}");

        if (UnityEngine.Random.value * 255f < escapeOdds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Checks 

    IEnumerator CheckTurnOrder()
    {

        if ((playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed))
        {
            if (playerUnit.Pokemon.HP > 0)
                yield return StartCoroutine(PlayerTurn());
            if (enemyUnit.Pokemon.HP > 0)
                yield return StartCoroutine(EnemyTurn(false));

            yield return EndTurn(playerUnit);
            yield return EndTurn(enemyUnit);
        }
        else
        {
            if ((enemyUnit.Pokemon.HP > 0))
                yield return StartCoroutine(EnemyTurn(false));
            if (playerUnit.Pokemon.HP > 0)
                yield return StartCoroutine(PlayerTurn());

            yield return EndTurn(enemyUnit);
            yield return EndTurn(playerUnit);
        }

        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    bool AccuracyCheck(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;
        
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[PokemonStat.Accuracy];
        int evasion = source.StatBoosts[PokemonStat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];
        
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy; 
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetFirstPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
            BattleOver(true);
    }

    void BattleOver(bool result)
    {
        state = BattleState.BattleOver;
        playerParty.Party.ForEach(p => p.OnBattleOver());
        OnBattleOver(result);
    }

    #endregion
}
