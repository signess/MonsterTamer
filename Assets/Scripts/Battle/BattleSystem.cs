using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, Bag, PartyScreen, AboutToUse, ForgetMove, BattleOver }

public enum BattleAction { Move, SwitchMonster, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [Header("Player")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] SpriteRenderer playerSprite;

    [Header("Enemy")]
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] SpriteRenderer tamerSprite;

    [Header("HUDs")]
    [SerializeField] CanvasGroup hudCanvasGroup;

    [Header("Dialog Box")]
    [SerializeField] BattleDialogBox dialogBox;

    [Header("Selector Buttons")]
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject choiceBox;
    [SerializeField] NewMoveSelectionUI newMoveSelectionUI;

    [Header("Texts")]
    [SerializeField] List<GameObject> actionTexts;
    [SerializeField] List<GameObject> moveTexts;
    [SerializeField] List<TextMeshProUGUI> ppTexts;
    [SerializeField] List<TextMeshProUGUI> typeTexts;

    [Header("Party Screen")]
    [SerializeField] PartyScreen partyScreen;

    [Header("Inventory Screen")]
    [SerializeField] InventoryUI inventoryUI;

    private InputManager inputManager;
    private BattleState state;
    
    private int currentAction = -1;
    private int lastAction = -1;
    private int currentMove = -1;
    private bool aboutToUseChoice = true;

    MonsterParty playerParty;
    MonsterParty tamerParty;
    Monster wildMonster;

    bool isTamerBattle = false;
    PlayerController player;
    TamerController tamer;

    int escapeAttempts;
    MoveBase moveToLearn;

    [SerializeField] GameObject pokeballSprite;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        isTamerBattle = false;
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetupBattle());
    }

    public void StartTamerBattle(MonsterParty playerParty, MonsterParty tamerParty)
    {
        isTamerBattle = true;
        this.playerParty = playerParty;
        this.tamerParty = tamerParty;

        player = playerParty.GetComponent<PlayerController>();
        tamer = tamerParty.GetComponent<TamerController>();

        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if(state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.CloseInventoryUI();
                state = BattleState.ActionSelection;
            };

            Action onItemUsed = () =>
            {
                state = BattleState.Busy;
                inventoryUI.CloseInventoryUI();
                StartCoroutine(RunTurns(BattleAction.UseItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.ForgetMove)
        {
            // Action<int> onMoveSelected = (moveIndex) =>
            // {
            //     newMoveSelectionUI.gameObject.SetActive(false);
            //     if (moveIndex == 4)
            //     {
            //         StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} did not learn {moveToLearn.Name}!"));
            //     }
            //     else
            //     {
            //         var selectedMove = playerUnit.Monster.Moves[moveIndex].Base;
            //         StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} forgot {selectedMove.Name}."));
            //         StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} learned {selectedMove.Name}."));
            //         playerUnit.Monster.Moves[moveIndex] = new Move(moveToLearn);
            //     }
            //     moveToLearn = null;
            //     state = BattleState.RunningTurn;
            // };
            // newMoveSelectionUI.HandleMoveSelection(onMoveSelected);
        }

    }


    public IEnumerator SetupBattle()
    {
        ToogleHUD(false);
        playerUnit.Clear();
        enemyUnit.Clear();
        if (!isTamerBattle)
        {
            playerUnit.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(true);
            playerSprite.sprite = player.Sprite;
            //Wild Battle
            enemyUnit.Setup(wildMonster, true);
            yield return Fader.Instance.FadeOut(.5f);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared.");
        }
        else
        {
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerSprite.gameObject.SetActive(true);
            tamerSprite.gameObject.SetActive(true);
            playerSprite.sprite = player.Sprite;
            tamerSprite.sprite = tamer.Sprite;

            yield return Fader.Instance.FadeOut(1f);
            yield return dialogBox.TypeDialog($"{tamer.Name} wants to battle!");

            //Send out first pokemon of the trainer
            tamerSprite.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyMonster = tamerParty.GetHealthyMonster();
            yield return dialogBox.TypeDialog($"{tamer.Name} send out {enemyMonster.Base.Name}.");
            enemyUnit.Setup(enemyMonster);
        }
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattlePlayerCharacterCamera);
        yield return new WaitForSeconds(1f);
        //Send out first pokemon of the player
        var playerMonster = playerParty.GetHealthyMonster();
        playerSprite.gameObject.SetActive(false);
        playerUnit.gameObject.SetActive(true);
        playerUnit.Setup(playerMonster);
        yield return dialogBox.TypeDialog($"Go {playerMonster.Base.Name}!");
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleMainCamera);
        yield return new WaitForSeconds(1f);
        ToogleHUD(true);
        SetMoveNamesAndDetails(playerUnit.Monster.Moves);

        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        EnableActionSelector(true);
    }


    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Monsters.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    public void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OpenPartyScreen();
    }

    public void MovesButton()
    {
        state = BattleState.MoveSelection;
        SetMoveNamesAndDetails(playerUnit.Monster.Moves);
        EnableMoveSelector(true);
    }

    private IEnumerator AboutToUse(Monster newMonster)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{tamer.Name} is about to send {newMonster.Base.Name}. Do you want to change Monsters?");

        state = BattleState.AboutToUse;
        EnableChoiceBox(true);
    }

    private IEnumerator ChooseMoveToForget(Monster monster, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Select a move to be replaced by {newMove.Name}: ");
        newMoveSelectionUI.gameObject.SetActive(true);
        newMoveSelectionUI.SetMoveData(monster.Moves.Select(x => x.Base).ToList());
        moveToLearn = newMove;
        state = BattleState.ForgetMove;
    }

    public void SelectedMoveToForget(int selectedMove)
    {
        Debug.Log($"Click on button {selectedMove}");
        StartCoroutine(ForgetMove(selectedMove));
    }

    private IEnumerator ForgetMove(int selectedMove)
    {
        newMoveSelectionUI.gameObject.SetActive(false);
        if (selectedMove == 4)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} did not learn {moveToLearn.Name}!");
        }
        else
        {
            var oldMove = playerUnit.Monster.Moves[selectedMove].Base;
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} forgot {oldMove.Name}.");
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} learned {moveToLearn.Name}.");
            playerUnit.Monster.Moves[selectedMove] = new Move(moveToLearn);
        }
        moveToLearn = null;
        state = BattleState.RunningTurn;
    }

    private void HandleActionSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            if (lastAction != -1)
                currentAction = lastAction;
            else
                currentAction += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            //if (currentAction >= 2)
            lastAction = currentAction;
            currentAction = 0;

        }
        else if (inputManager.GetBattleMoveInput().x < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            if (currentAction >= 2)
                currentAction--;
        }
        else if (inputManager.GetBattleMoveInput().x > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            if (currentAction >= 1)
                currentAction++;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        //UpdateActionSelection(currentAction);

        if (inputManager.GetBattleConfirmInput() && currentAction != -1)
        {
            if (currentAction == 0)
            {
                MovesButton();
            }
            else if (currentAction == 1)
            {
                //BAG
                BagButton();
            }
            else if (currentAction == 2)
            {
                //RUN
                EscapeButton();
            }
            else if (currentAction == 3)
            {
                //CHANGE PARTY
                PartyScreenButton();
            }
        }
    }

    public void PartyScreenButton()
    {
        OpenPartyScreen();
    }

    private void HandleMoveSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            currentMove += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            currentMove -= 2;

        }
        else if (inputManager.GetBattleMoveInput().x < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            --currentMove;
        }
        else if (inputManager.GetBattleMoveInput().x > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;

            ++currentMove;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);

        //UpdateMoveSelection(currentMove);

        if (inputManager.GetBattleConfirmInput() && currentMove != -1)
        {
            MoveSelected(currentMove);
        }
        else if (inputManager.GetBattleCancelInput())
        {
            CancelButton();
        }
    }

    public void MoveSelected(int selectedMove)
    {
        currentMove = selectedMove;
        var move = playerUnit.Monster.Moves[currentMove];
        if (move.PP == 0)
        {
            dialogBox.SetDialog("Not enough PP!");
            return;
        }

        DisableSelectors();
        StartCoroutine(RunTurns(BattleAction.Move));
    }

    public void BagButton()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OpenInventoryUI();
    }

    
    private void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            MonsterSelected();
        };

        Action onBack = () =>
        {
            CancelButton();
        };

        partyScreen.HandleUpdate(onSelected, onBack);

    }

    public void MonsterSelected()
    {
        var selectedMember = partyScreen.SelectedMember;
        if (selectedMember.HP <= 0)
        {
            partyScreen.SetMessageText("You can't send out a fainted Monster.");
            return;
        }
        if (selectedMember == playerUnit.Monster)
        {
            partyScreen.SetMessageText("You can't switch with the same Monster.");
            return;
        }
        partyScreen.gameObject.SetActive(false);
        DisableSelectors();
        if (partyScreen.CalledFrom == BattleState.ActionSelection)
        {
            StartCoroutine(RunTurns(BattleAction.SwitchMonster));
        }
        else
        {
            state = BattleState.Busy;
            bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
            StartCoroutine(SwitchMonster(selectedMember, isTrainerAboutToUse));
        }
        partyScreen.CalledFrom = null;
    }

    public void CancelButton()
    {
        if (playerUnit.Monster.HP <= 0)
        {
            dialogBox.SetDialog("You have to choose a Monster to continue!");
            return;
        }


        partyScreen.ClosePartyScreen();
        if (partyScreen.CalledFrom == BattleState.AboutToUse)
        {
            StartCoroutine(SendNextTamerMonster());
        }
        else
            ActionSelection();

        partyScreen.CalledFrom = null;
    }


    public void HandleAboutToUse()
    {
        if (inputManager.GetBattleMoveInput().y > 0 || inputManager.GetBattleMoveInput().y < 0)
            aboutToUseChoice = !aboutToUseChoice;
        if (inputManager.GetBattleConfirmInput())
        {
            AboutToUseSelected(aboutToUseChoice);
        }
        else if (inputManager.GetBattleCancelInput())
        {
            AboutToUseSelected(false);
        }
    }

    public void AboutToUseSelected(bool selected)
    {
        EnableChoiceBox(false);
        if (selected)
        {
            OpenPartyScreen();
        }
        else
        {
            StartCoroutine(SendNextTamerMonster());
        }
    }


    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
        moveSelector.SetActive(!enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        actionSelector.SetActive(!enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void DisableSelectors()
    {
        moveSelector.SetActive(false);
        actionSelector.SetActive(false);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                actionTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = GlobalSettings.Instance.HighlightedColor;
            }
            else
            {
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
        }
    }

    private void SetMoveNamesAndDetails(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().text = moves[i].Base.Name;
                moveTexts[i].GetComponent<Button>().enabled = true;
                typeTexts[i].text = moves[i].Base.Type.ToString();
                ppTexts[i].text = $"{moves[i].PP}/{moves[i].Base.PP} PP";
                if (moves[i].PP > 0)
                {
                    ppTexts[i].color = Color.black;
                }
                else
                {
                    ppTexts[i].color = Color.red;
                }
            }
            else
            {
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().text = "-";
                moveTexts[i].GetComponent<Button>().enabled = false;
                ppTexts[i].text = "";
                typeTexts[i].text = "";
            }
        }
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMove];
            enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

            int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;

            //Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondMonster = secondUnit.Monster;

            // First Turn
            yield return PerformMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondMonster.HP > 0)
            {
                // Second Turn
                yield return PerformMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchMonster)
            {
                var selectedMonster = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMonster);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                //This is handled from item screen, so do nothing and skip to enemy move
                DisableSelectors();
            }
            else if (playerAction == BattleAction.Run)
            {
                DisableSelectors();
                yield return TryToEscape();
            }

            //EnemyTurn
            var enemyMove = enemyUnit.Monster.GetRandomMove();
            yield return PerformMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator PerformMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canPerformMove = sourceUnit.Monster.OnBeforeMove();
        if (!canPerformMove)
        {
            yield return ShowStatusChanges(sourceUnit.Monster);
            yield return sourceUnit.HUD.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");

        if (CheckMoveAccuracy(move, sourceUnit.Monster, targetUnit.Monster))
        {
            ToogleHUD(false);

            if (sourceUnit.IsPlayerUnit)
                CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattlePlayerCamera);
            else
                CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
            yield return new WaitForSeconds(.5f);

            yield return sourceUnit.PlayAttackAnimation();

            yield return new WaitForSeconds(.5f);

            if (sourceUnit.IsPlayerUnit)
                CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleMainCamera);
            else
                CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleMainCamera);
            yield return new WaitForSeconds(.5f);
            ToogleHUD(true);

            yield return targetUnit.PlayHitAnimation();

            if (move.Base.MoveCategory == MoveCategory.Status)
            {
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.HUD.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Monster.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffect(secondary, sourceUnit.Monster, targetUnit.Monster, secondary.Target);
                }
            }


            if (targetUnit.Monster.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name}'s attack missed!");
        }
    }

    private IEnumerator RunMoveEffect(MoveEffects effects, Monster sourceUnit, Monster targetUnit, MoveTarget moveTarget)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                sourceUnit.ApplyBoost(effects.Boosts);
            else
                targetUnit.ApplyBoost(effects.Boosts);
        }
        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Statuses like burn or psn will hurt the pokemo after the turn!
        sourceUnit.Monster.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Monster);
        yield return sourceUnit.HUD.WaitForHPUpdate();
        if (sourceUnit.Monster.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    private bool CheckMoveAccuracy(Move move, Monster source, Monster target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

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

    private IEnumerator ShowStatusChanges(Monster monster)
    {
        while (monster.StatusChanges.Count > 0)
        {
            var message = monster.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Monster.Base.Name} fainted!");
        yield return faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            //Exp gain
            int expYield = faintedUnit.Monster.Base.BaseXp;
            int enemyLevel = faintedUnit.Monster.Level;
            float tamerBonus = (isTamerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * tamerBonus) / 7);
            playerUnit.Monster.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} gained {expGain} experience points.");
            yield return playerUnit.HUD.SetExpSmooth();

            //Check level up
            while (playerUnit.Monster.CheckForLevelUp())
            {
                playerUnit.HUD.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} grew to level {playerUnit.Monster.Level}!");

                //Try to learn a new move
                var newMove = playerUnit.Monster.GetLearnableMoveWhenLevelUp();
                if (newMove != null)
                {
                    if (playerUnit.Monster.Moves.Count < 4)
                    {
                        playerUnit.Monster.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} learned {newMove.Base.Name}.");
                        SetMoveNamesAndDetails(playerUnit.Monster.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} is trying to learn {newMove.Base.Name}.");
                        yield return dialogBox.TypeDialog($"But it cannot learn more than four moves!");
                        yield return ChooseMoveToForget(playerUnit.Monster, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.ForgetMove);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.HUD.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextMonster = playerParty.GetHealthyMonster();
            if (nextMonster != null)
            {
                OpenPartyScreen();
            }
            else
                BattleOver(false);
        }
        else
        {
            if (!isTamerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextMonster = tamerParty.GetHealthyMonster();
                if (nextMonster != null)
                    StartCoroutine(AboutToUse(nextMonster));
                else
                    BattleOver(true);
            }
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    private IEnumerator SwitchMonster(Monster newMonster, bool isTrainerAboutToUse = false)
    {
        if (playerUnit.Monster.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}.");
            yield return playerUnit.PlayFaintAnimation();
        }

        yield return SendNextMonster(newMonster, isTrainerAboutToUse);
    }
    private IEnumerator SendNextMonster(Monster nextMonster, bool isTrainerAboutToUse = false)
    {
        playerUnit.Setup(nextMonster);
        SetMoveNamesAndDetails(nextMonster.Moves);

        yield return dialogBox.TypeDialog($"Go {nextMonster.Base.Name}!");

        if(isTrainerAboutToUse)
            StartCoroutine(SendNextTamerMonster());
        else
            state = BattleState.RunningTurn;

    }

    private IEnumerator SendNextTamerMonster()
    {
        state = BattleState.Busy;

        var nextMonster = tamerParty.GetHealthyMonster();
        enemyUnit.Setup(nextMonster);
        yield return dialogBox.TypeDialog($"{tamer.Name} send out {nextMonster.Base.Name}!");

        state = BattleState.RunningTurn;
    }

   

    private IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTamerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal other tamer's Monsters!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used Pokeball!");
        var pokeballObject = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObject.GetComponent<SpriteRenderer>();

        //Animations
        Sequence throwSequence = DOTween.Sequence();
        throwSequence.Append(pokeball.transform.DOJump(enemyUnit.transform.position, 1.5f, 1, .8f));
        throwSequence.Join(pokeball.transform.DORotate(new Vector3(0, 0, -720), .8f, RotateMode.FastBeyond360));

        throwSequence.Append(pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 1), 1f, 1, 0.5f));
        yield return throwSequence.WaitForCompletion();
        //suga suga
        yield return enemyUnit.PlayCaptureAnimation();
        yield return new WaitForSeconds(1f);

        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
        Sequence captureSequence = DOTween.Sequence();
        captureSequence.Append(pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1f, .5f)).SetEase(Ease.OutBounce);

        int shakeCount = TryToCatchMonster(enemyUnit.Monster);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(1f);
            captureSequence.Append(pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f));
        }
        yield return captureSequence.WaitForCompletion();
        if (shakeCount == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} was caught!");
            CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleMainCamera);

            playerParty.AddMonster(enemyUnit.Monster);
            yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} has been added to your party!");
            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleMainCamera);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog($"Almost caught it!");
            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    private int TryToCatchMonster(Monster monster)
    {
        float a = (3 * monster.MaxHp - 2 * monster.HP) * monster.Base.CatchRate * ConditionsDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);
        if (a >= 255)
            return 4;
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            shakeCount++;
        }

        return shakeCount;
    }

    public void EscapeButton()
    {
        StartCoroutine(RunTurns(BattleAction.Run));
    }

    private IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        if (isTamerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from tamer battle!");
            state = BattleState.RunningTurn;
            yield break;
        }

        escapeAttempts++;

        int playerSpeed = playerUnit.Monster.Speed;
        int enemySpeed = enemyUnit.Monster.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Run away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Run away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape!");
                state = BattleState.RunningTurn;
            }
        }
    }

    private void ToogleHUD(bool on)
    {
        if (on)
            hudCanvasGroup.alpha = 1;
        else
            hudCanvasGroup.alpha = 0;
    }
}
