using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    private PartyMemberUI[] memberSlots;
    private List<Monster> monsters;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> monsters)
    {
        this.monsters = monsters;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < monsters.Count)
                memberSlots[i].SetData(monsters[i]);
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
