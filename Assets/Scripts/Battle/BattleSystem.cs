using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public enum BattleState
{
    Start, PlayerTurn, PlayerMove, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;

    [Header("Enemy")]
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;

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

    private InputManager inputManager;
    private BattleState state;
    [SerializeField] private int currentAction = -1;
    [SerializeField] private int currentMove = -1;


    private void Start()
    {
        inputManager = InputManager.Instance;
        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        if (state == BattleState.PlayerTurn)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHUD.SetData(playerUnit.Monster);
        SetMoveNamesAndDetails(playerUnit.Monster.Moves);

        enemyUnit.Setup();
        enemyHUD.SetData(enemyUnit.Monster);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared.");
        yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    private void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        dialogBox.SetDialog("Choose an action");
        EnableActionSelector(true);
    }

    public void PlayerMove()
    {
        state = BattleState.PlayerMove;
        EnableMoveSelector(true);
    }

    private void HandleActionSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentAction <= 1)
                currentAction += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentAction >= 2)
                currentAction -= 2;

        }
        else if (inputManager.GetBattleMoveInput().x < 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentAction % 2 != 0) //SE O NUMERO ATUAL E IMPAR
                currentAction--; //SUBTRAIR 1
        }
        else if (inputManager.GetBattleMoveInput().x > 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentAction % 2 == 0) // SE O NUMERO É PAR
                currentAction++; // ADCIONAR 1
        }

        UpdateActionSelection(currentAction);

        if (inputManager.GetBattleConfirmInput())
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
            else if (currentAction == 1)
            {

            }
            else if (currentAction == 2)
            {

            }
            else if (currentAction == 3)
            {

            }
        }
    }

    private void HandleMoveSelection()
    {
        if (inputManager.GetBattleMoveInput().y < 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentMove <= 1)
                currentMove += 2;

        }
        else if (inputManager.GetBattleMoveInput().y > 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentMove >= 2)
                currentMove -= 2;

        }
        else if (inputManager.GetBattleMoveInput().x < 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentMove % 2 != 0) //SE O NUMERO ATUAL E IMPAR
                currentMove--; //SUBTRAIR 1
        }
        else if (inputManager.GetBattleMoveInput().x > 0)
        {
            if (EnableKeyNavigation())
                return;

            if (currentMove % 2 == 0) // SE O NUMERO É PAR
                currentMove++; // ADCIONAR 1
        }

        UpdateMoveSelection(currentMove);

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
                moveTexts[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().color = highlightedColor;
            }
            else
            {
                moveTexts[i].transform.DOScale(Vector3.one, 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
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
                ppTexts[i].text = $"{moves[i].Base.PP}/{moves[i].Base.PP} PP";
                typeTexts[i].text = moves[i].Base.Type.ToString();
            }
            else
            {
                moveTexts[i].GetComponentInChildren<TextMeshProUGUI>().text = "-";
                ppTexts[i].text = "";
                typeTexts[i].text = "";
            }
        }
    }
}
