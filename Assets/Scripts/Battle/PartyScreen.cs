using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    private PartyMemberUI[] memberSlots;
    private List<Monster> monsters;

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

        if (InputManager.Instance.GetBattleConfirmInput() && selection != -1)
        {
            onSelected?.Invoke();
        }
        else if (InputManager.Instance.GetBattleCancelInput())
        {
            onBack?.Invoke();
        }
    }

   

    public void SetPartyData(List<Monster> monsters)
    {
        this.monsters = monsters;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < monsters.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(monsters[i]);
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
}
