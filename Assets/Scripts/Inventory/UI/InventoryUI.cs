using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Action backAction;
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] int selectedItem = 0;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    private void Start()
    {
        UpdateItemList();
    }
    public void HandleUpdate(Action onBack)
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

        if (backAction != onBack)
            backAction = onBack;
    }


    public void CloseInventory()
    {
        backAction?.Invoke();
    }

    private void UpdateItemList()
    {
        // Clear all the existing items
        foreach(Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }
        for(int i = 0; i < slotUIList.Count; i++)
        {
            int closureIndex = i;
            slotUIList[closureIndex].GetComponent<Button>().onClick.AddListener(() => ButtonSelected(closureIndex));
        }

        UpdateItemSelection();
    }

    private void ButtonSelected(int selectedItem)
    {
        this.selectedItem = selectedItem;
        UpdateItemSelection();
    }

    private void UpdateItemSelection()
    {
        for(int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].NameText.color = GlobalSettings.Instance.HighlightedColor;
            }
            else
                slotUIList[i].NameText.color = Color.black;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;
    }
}