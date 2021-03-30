using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    GameState state;

    private void Start()
    {
        playerMovement.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerMovement.GetInput();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
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

        var playerParty = playerMovement.GetComponent<MonsterParty>();
        var wildMonster = FindObjectOfType<WildArea>().GetComponent<WildArea>().GetRandomWildMonster();

        battleSystem.StartBattle(playerParty, wildMonster);

    }
}
