using System.Collections.Generic;
using System.Collections;
using Cinemachine;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    TamerController tamer;

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }

    private void Start()
    {
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


    public void OnEnterTamersView(TamerController tamer)
    {
        state = GameState.Cutscene;
        StartCoroutine(tamer.TriggerTrainerBattle(playerController));
    }

    public void StartWildBattle()
    {
        StartCoroutine(WildBattleTransition());
    }

    public void StartTamerBattle(TamerController tamer)
    {
        this.tamer = tamer;
        StartCoroutine(TamerBattleTransition(tamer));
    }

    private void EndBattle(bool won)
    {
        if (tamer != null && won)
        {
            tamer.BattleLost();
            tamer = null;
        }

        StartCoroutine(TransitionEndBattle());
        state = GameState.FreeRoam;

        //worldCamera.gameObject.SetActive(true);
    }

    private IEnumerator TransitionEndBattle()
    {
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.SimpleFadeTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(1.5f);
        battleSystem.gameObject.SetActive(false);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.OverworldCamera);
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.SimpleFadeTransition);
        SceneTransitionController.Instance.ToogleTransition(2f);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator WildBattleTransition()
    {
        state = GameState.Battle;
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.MosaicBattleTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(1.5f);

        battleSystem.gameObject.SetActive(true);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.VerticalFadeTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(0.5f);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = FindObjectOfType<WildArea>().GetComponent<WildArea>().GetRandomWildMonster();
        var wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Level);

        battleSystem.StartBattle(playerParty, wildMonsterCopy);

    }

    private IEnumerator TamerBattleTransition(TamerController tamer)
    {
        state = GameState.Battle;
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.MosaicBattleTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(1.5f);

        battleSystem.gameObject.SetActive(true);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.VerticalFadeTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(0.5f);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var tamerParty = tamer.GetComponent<MonsterParty>();


        battleSystem.StartTamerBattle(playerParty, tamerParty);

    }
}
