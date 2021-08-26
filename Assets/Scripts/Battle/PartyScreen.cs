using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    private PartyMemberUI[] memberSlots;
    private List<Monster> monsters;
    private MonsterParty party;
    private Action selectedAction;
    private Action backAction;

    int selection = 0;
    public int Selection { get => selection; set => selection = value; }

    public Monster SelectedMember => monsters[selection];

    /// <summary>
    /// Party screen can be called from differente states like ActionSelection, RunningTurn, AboutToUse
    /// </summary>
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);

        party = MonsterParty.GetPlayerParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        if (InputManager.Instance.GetBattleMoveInput().y < 0 && !InputManager.Instance.HoldInput)
        {
            InputManager.Instance.HoldInput = true;
            selection += 2;
        }
        else if (InputManager.Instance.GetBattleMoveInput().y > 0 && !InputManager.Instance.HoldInput)
        {
            InputManager.Instance.HoldInput = true;
            selection -= 2;
        }
        else if (InputManager.Instance.GetBattleMoveInput().x < 0 && !InputManager.Instance.HoldInput)
        {
            InputManager.Instance.HoldInput = true;
            --selection;
        }
        else if (InputManager.Instance.GetBattleMoveInput().x > 0 && !InputManager.Instance.HoldInput)
        {
            InputManager.Instance.HoldInput = true;
            ++selection;
        }

        selection = Mathf.Clamp(selection, 0, monsters.Count - 1);
        //UpdateMemberSelection(currentMember);


        if (selectedAction != onSelected)
            selectedAction = onSelected;

        if (backAction != onBack)
            backAction = onBack;

        if (InputManager.Instance.GetBattleConfirmInput() && selection != -1)
        {
            onSelected?.Invoke();
        }
        else if (InputManager.Instance.GetBattleCancelInput())
        {
            onBack?.Invoke();
        }
    }

    public void PartyButton(int selected)
    {
        Selection = selected;
        selectedAction?.Invoke();
    }

   public void BackButton()
    {
        backAction?.Invoke();
    }

    public void SetPartyData()
    {
        monsters = party.Monsters;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(monsters[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }
        messageText.text = "Choose a Monster.";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    private IEnumerator PartyScreenFader(bool open)
    {
        if(open)
        {
            yield return Fader.Instance.FadeIn(.5f);
            canvasGroup.alpha = 1;
            yield return Fader.Instance.FadeOut(.5f);
        }
        else if(!open)
        {
            yield return Fader.Instance.FadeIn(.5f);
            canvasGroup.alpha = 0;
            yield return Fader.Instance.FadeOut(.5f);
            gameObject.SetActive(false);
            backAction = null;
            selectedAction = null;
        }
    }
    public void OpenPartyScreen()
    {
        StartCoroutine(PartyScreenFader(true));
    }

    public void ClosePartyScreen()
    {
        StartCoroutine(PartyScreenFader(false));
    }
}
