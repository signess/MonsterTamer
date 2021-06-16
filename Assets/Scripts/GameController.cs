using System.Collections.Generic;
using System.Collections;
using Cinemachine;
using UnityEngine;
using System;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused }

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] MenuController menuController;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    GameState state;
    GameState prevState;

    TamerController tamer;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    private void Awake()
    {
        Instance = this;
        MonstersDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;
        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () => { state = GameState.Dialog; };
        DialogManager.Instance.OnCloseDialog += () => { if (state == GameState.Dialog) state = GameState.FreeRoam; };

        menuController.OnMenuSelected += OnMenuSelected;
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
        else if(state == GameState.Menu)
        {
            //menu handle
        }
        else if(state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {
                //Go to sumary screen
            };

            Action onBack = () =>
            {
                PartyScreenBackButon();
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if(state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.Menu;
            };
            inventoryUI.HandleUpdate(onBack);
        }
    }

    public void PauseGame(bool pause, GameState nextState = GameState.Paused)
    {
        if (pause)
        {
            playerController.ForceStopMovement();
            prevState = state;
            state = nextState;
        }
        else
        {
            state = prevState;
        }
    }


    public void OnEnterTamersView(TamerController tamer)
    {
        state = GameState.Cutscene;
        playerController.ForceStopMovement();
        StartCoroutine(tamer.TriggerTrainerBattle(playerController));
    }

    public void StartWildBattle()
    {
        menuController.ToggleMenuButton();
        StartCoroutine(WildBattleTransition());
    }

    public void StartTamerBattle(TamerController tamer)
    {
        menuController.ToggleMenuButton();
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
        menuController.ToggleMenuButton();

        //worldCamera.gameObject.SetActive(true);
    }

    private IEnumerator TransitionEndBattle()
    {
       // SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.SimpleFadeTransition);
       // SceneTransitionController.Instance.ToogleTransition();
        yield return Fader.Instance.FadeIn(.5f);

        battleSystem.gameObject.SetActive(false);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.OverworldCamera);

        yield return Fader.Instance.FadeOut(.5f);
      //  SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.SimpleFadeTransition);
       // SceneTransitionController.Instance.ToogleTransition(2f);
     //   yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator WildBattleTransition()
    {
        state = GameState.Battle;

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = CurrentScene.GetComponent<WildArea>().GetRandomWildMonster();
        var wildMonsterCopy = new Monster(wildMonster.Base, wildMonster.Level);

        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.MosaicBattleTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(1.5f);
        yield return Fader.Instance.FadeIn(0.1f);

        battleSystem.gameObject.SetActive(true);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.VerticalFadeTransition);
        SceneTransitionController.Instance.ToogleTransition(100f);
        yield return new WaitForSeconds(0.5f);       

        battleSystem.StartBattle(playerParty, wildMonsterCopy);
    }

    private IEnumerator TamerBattleTransition(TamerController tamer)
    {
        state = GameState.Battle;
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.MosaicBattleTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(1.5f);
        yield return Fader.Instance.FadeIn(0.1f);

        battleSystem.gameObject.SetActive(true);
        CameraManager.Instance.SwitchPriority(CameraManager.Instance.BattleEnemyCamera);
        SceneTransitionController.Instance.ChangeTransition(SceneTransitionController.Instance.VerticalFadeTransition);
        SceneTransitionController.Instance.ToogleTransition();
        yield return new WaitForSeconds(0.5f);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var tamerParty = tamer.GetComponent<MonsterParty>();


        battleSystem.StartTamerBattle(playerParty, tamerParty);

    }

    public void SetCurrentScene(SceneDetails currentScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currentScene;
    }

    private void OnMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
        {

        }
        else if(selectedItem == 1)
        {
            //Party Screen
            partyScreen.gameObject.SetActive(true);
            partyScreen.SetPartyData(playerController.GetComponent<MonsterParty>().Monsters);
            state = GameState.PartyScreen;
        }
        else if(selectedItem == 2)
        {
            //Bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if(selectedItem == 3)
        {

        }
        else if (selectedItem == 4)
        {
            //Save
            SavingSystem.i.Save("saveSlot1");
        }
        else if(selectedItem == 5)
        {
            //Load
            SavingSystem.i.Load("saveSlot1");
        }


    }

    public void PartyScreenBackButon()
    {
        partyScreen.gameObject.SetActive(false);
        menuController.ToogleMenu();
    }
}
