using System.Net.NetworkInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum BattleState
{
    Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver
}

public class BattleSystem : MonoBehaviour
{
    public event Action<bool> OnBattleOver;

    [Header("Player")]
    [SerializeField] BattleUnit playerUnit;

    [Header("Enemy")]
    [SerializeField] BattleUnit enemyUnit;

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
    private int currentAction = -1;
    private int lastAction = -1;
    private int currentMove = -1;
    private int currentMember = -1;

    MonsterParty playerParty;
    Monster wildMonster;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void StartBattle(MonsterParty monsterParty, Monster wildMonster)
    {
        this.playerParty = monsterParty;
        this.wildMonster = wildMonster;
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
        playerUnit.Setup(playerParty.GetHealthyMonster());
        SetMoveNamesAndDetails(playerUnit.Monster.Moves);

        enemyUnit.Setup(wildMonster);

        partyScreen.Init();

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared.");

        ChooseFirstTurn();
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        EnableActionSelector(true);
    }

    private void ChooseFirstTurn()
    {
        if (playerUnit.Monster.Speed >= enemyUnit.Monster.Speed)
        {
            ActionSelection();
        }
        else
            StartCoroutine(PerformEnemyMove());
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
                OpenPartyScreen();
                //MONSTERS
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
            DisableSelectors();
            StartCoroutine(PerformPlayerMove());
        }
        else if (inputManager.GetBattleCancelInput())
        {
            EnableActionSelector(true);
        }
    }

    public void MoveSeletected(int selectedMove)
    {
        currentMove = selectedMove;
        DisableSelectors();
        StartCoroutine(PerformPlayerMove());
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
        state = BattleState.Busy;
        DisableSelectors();
        StartCoroutine(SwitchMonster(selectedMember));
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
                moveTexts[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
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

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Monster.Moves[currentMove];
        yield return PerformMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
            StartCoroutine(PerformEnemyMove());
    }

    private IEnumerator PerformEnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Monster.GetRandomMove();
        yield return PerformMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    private IEnumerator PerformMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");
        yield return sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(.5f);
        yield return targetUnit.PlayHitAnimation();

        if (move.Base.MoveCategory == MoveCategory.Status)
        {
            yield return RunMoveEffect(move, sourceUnit.Monster, targetUnit.Monster);
        }
        else
        {
            var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
            yield return targetUnit.HUD.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }


        if (targetUnit.Monster.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} fainted!");
            yield return targetUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(1.5f);
            CheckForBattleOver(targetUnit);
        }

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

    private IEnumerator RunMoveEffect(Move move, Monster sourceUnit, Monster targetUnit)
    {
        var effects = move.Base.Effects;
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                sourceUnit.ApplyBoost(effects.Boosts);
            else
                targetUnit.ApplyBoost(effects.Boosts);
        }
        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
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

    private IEnumerator SendNextMonster(Monster nextMonster)
    {
        playerUnit.Setup(nextMonster);
        SetMoveNamesAndDetails(nextMonster.Moves);
        currentMove = -1;
        currentAction = -1;

        yield return dialogBox.TypeDialog($"Go {nextMonster.Base.Name}!");
    }

    private IEnumerator SwitchMonster(Monster newMonster)
    {
        bool currentMonsterFainted = true;
        if (playerUnit.Monster.HP > 0)
        {
            currentMonsterFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}.");
            yield return playerUnit.PlayFaintAnimation();
        }

        yield return SendNextMonster(newMonster);
        if (currentMonsterFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(PerformEnemyMove());
    }

    public void CancelButton()
    {
        state = BattleState.ActionSelection;
        EnableActionSelector(true);
        partyScreen.gameObject.SetActive(false);
        DisableKeyNavigation();
    }
}
