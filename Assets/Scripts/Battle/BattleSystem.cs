using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }

public enum BattleAction { Move, SwitchMonster, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [Header("Player")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] Image playerSprite;

    [Header("Enemy")]
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] Image tamerSprite;

    [Header("Dialog Box")]
    [SerializeField] BattleDialogBox dialogBox;

    [Header("Selector Buttons")]
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;

    [SerializeField] List<GameObject> actionTexts;
    [SerializeField] List<GameObject> moveTexts;
    [SerializeField] List<TextMeshProUGUI> ppTexts;
    [SerializeField] List<TextMeshProUGUI> typeTexts;
    [SerializeField] Color highlightedColor;

    [Header("Party Screen")]
    [SerializeField] PartyScreen partyScreen;

    private InputManager inputManager;
    private BattleState state;
    private BattleState? prevState;
    private int currentAction = -1;
    private int lastAction = -1;
    private int currentMove = -1;
    private int currentMember = -1;

    MonsterParty playerParty;
    MonsterParty tamerParty;
    Monster wildMonster;

    bool isTamerBattle = false;
    PlayerController player;
    TamerController tamer;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        isTamerBattle = false;
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
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
    }

    public IEnumerator SetupBattle()
    {
        if (!isTamerBattle)
        {
            //Wild Battle
            playerUnit.Setup(playerParty.GetHealthyMonster());
            enemyUnit.Setup(wildMonster);

            SetMoveNamesAndDetails(playerUnit.Monster.Moves);
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

            yield return dialogBox.TypeDialog($"{tamer.Name} wants to battle!");
        }

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
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);

    }

    public void MoveSelection()
    {
        state = BattleState.MoveSelection;
        EnableMoveSelector(true);
    }

    private void HandleActionSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            if (lastAction != -1)
                currentAction = lastAction;
            else
                currentAction += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            //if (currentAction >= 2)
            lastAction = currentAction;
            currentAction = 0;

        }
        else if (inputManager.GetBattleMoveInput().x < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            if (currentAction >= 2)
                currentAction--;
        }
        else if (inputManager.GetBattleMoveInput().x > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            if (currentAction >= 1)
                currentAction++;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        UpdateActionSelection(currentAction);

        if (inputManager.GetBattleConfirmInput() && currentAction != -1)
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //BAG
            }
            else if (currentAction == 2)
            {
                //RUN
            }
            else if (currentAction == 3)
            {
                //CHANGE PARTY
                prevState = state;
                OpenPartyScreen();
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            currentMove += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            currentMove -= 2;

        }
        else if (inputManager.GetBattleMoveInput().x < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            --currentMove;
        }
        else if (inputManager.GetBattleMoveInput().x > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            ++currentMove;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);

        UpdateMoveSelection(currentMove);

        if (inputManager.GetBattleConfirmInput() && currentMove != -1)
        {
            var move = playerUnit.Monster.Moves[currentMove];
            if (move.PP == 0)
            {
                dialogBox.SetDialog("Not enough PP!");
                return;
            }

            DisableSelectors();
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (inputManager.GetBattleCancelInput())
        {
            EnableActionSelector(true);
        }
    }

    public void MoveSeletected(int selectedMove)
    {
        var move = playerUnit.Monster.Moves[currentMove];
        if (move.PP == 0)
        {
            dialogBox.SetDialog("Not enough PP!");
            return;
        }

        currentMove = selectedMove;
        DisableSelectors();
        StartCoroutine(RunTurns(BattleAction.Move));
    }

    private void HandlePartySelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            currentMember += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            currentMember -= 2;

        }
        else if (inputManager.GetBattleMoveInput().x < 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            --currentMember;
        }
        else if (inputManager.GetBattleMoveInput().x > 0 && !inputManager.HoldInput)
        {
            inputManager.HoldInput = true;
            if (EnableKeyNavigation())
                return;

            ++currentMember;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);
        partyScreen.UpdateMemberSelection(currentMember);

        if (inputManager.GetBattleConfirmInput() && currentMember != -1)
        {
            MonsterSelected(currentMember);
        }
        else if (inputManager.GetBattleCancelInput())
        {
            partyScreen.gameObject.SetActive(false);
            EnableActionSelector(true);
        }
    }

    public void MonsterSelected(int selected)
    {
        var selectedMember = playerParty.Monsters[selected];
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
        if (prevState == BattleState.ActionSelection)
        {
            prevState = null;
            StartCoroutine(RunTurns(BattleAction.SwitchMonster));
        }
        else
        {
            state = BattleState.Busy;
            StartCoroutine(SwitchMonster(selectedMember));
        }
        DisableSelectors();
    }

    private bool EnableKeyNavigation()
    {
        if (currentAction == -1 || currentMove == -1)
        {
            currentAction = 0;
            currentMove = 0;
            return true;
        }
        return false;
    }
    public void DisableKeyNavigation()
    {
        currentAction = -1;
        currentMove = -1;
        currentMember = -1;
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
                actionTexts[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
                actionTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = highlightedColor;
            }
            else
            {
                actionTexts[i].transform.DOScale(Vector3.one, 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
                actionTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = highlightedColor;
            }
            else
            {
                moveTexts[i].transform.DOScale(Vector3.one, 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
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
                ppTexts[i].text = $"{moves[i].Base.PP}/{moves[i].Base.PP} PP";
                typeTexts[i].text = moves[i].Base.Type.ToString();
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
                var selectedMonster = playerParty.Monsters[currentMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMonster);
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
            yield return sourceUnit.HUD.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Monster);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");

        if (CheckMoveAccuracy(move, sourceUnit.Monster, targetUnit.Monster))
        {
            yield return sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(.5f);
            yield return targetUnit.PlayHitAnimation();

            if (move.Base.MoveCategory == MoveCategory.Status)
            {
                yield return RunMoveEffect(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
                yield return targetUnit.HUD.UpdateHP();
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
                yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} fainted!");
                yield return targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(1.5f);
                CheckForBattleOver(targetUnit);
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
        yield return sourceUnit.HUD.UpdateHP();
        if (sourceUnit.Monster.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} fainted!");
            yield return sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);
            CheckForBattleOver(sourceUnit);
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
            BattleOver(true);
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

    private IEnumerator SwitchMonster(Monster newMonster)
    {
        if (playerUnit.Monster.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}.");
            yield return playerUnit.PlayFaintAnimation();
        }

        yield return SendNextMonster(newMonster);
    }
    private IEnumerator SendNextMonster(Monster nextMonster)
    {
        playerUnit.Setup(nextMonster);
        SetMoveNamesAndDetails(nextMonster.Moves);
        currentMove = -1;
        currentAction = -1;

        yield return dialogBox.TypeDialog($"Go {nextMonster.Base.Name}!");

        state = BattleState.RunningTurn;
    }


    public void CancelButton()
    {
        state = BattleState.ActionSelection;
        EnableActionSelector(true);
        partyScreen.gameObject.SetActive(false);
        DisableKeyNavigation();
    }
}
