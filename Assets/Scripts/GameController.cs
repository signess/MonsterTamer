using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;

    private void Awake()
    {
        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () => { state = GameState.Dialog; };
        DialogManager.Instance.OnCloseDialog += () => { if (state == GameState.Dialog) state = GameState.FreeRoam; };
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.GetInput();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    private void StartBattle()
    {
        StartCoroutine(BattleTransition());
    }

    private void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private IEnumerator BattleTransition()
    {
        state = GameState.Battle;
        worldCamera.GetComponent<SimpleBlit>().FadeIn();
        yield return new WaitForSeconds(1.5f);

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        worldCamera.GetComponent<SimpleBlit>().SetCutoffToZero();

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = FindObjectOfType<WildArea>().GetComponent<WildArea>().GetRandomWildMonster();

        battleSystem.StartBattle(playerParty, wildMonster);

    }
}
