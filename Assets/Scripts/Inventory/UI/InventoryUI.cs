using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private Action backAction;
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    private int selectedItem = 0;

    private List<ItemSlotUI> slotUIList;
    private Inventory inventory;
    private RectTransform itemListRect;

    private const int ITEMS_IN_VIEWPORT = 6;
    private const float HEIGHT_OFFSET = 40f;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
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
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }
        for (int i = 0; i < slotUIList.Count; i++)
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
        for (int i = 0; i < slotUIList.Count; i++)
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

        HandleScrolling();
    }

    private void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - ITEMS_IN_VIEWPORT / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        ShowArrows();
    }

    public void ShowArrows()
    {
        bool showUpArrow = (selectedItem > ITEMS_IN_VIEWPORT / 2) && (itemListRect.localPosition.y >= slotUIList[0].Height - HEIGHT_OFFSET);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = (selectedItem + ITEMS_IN_VIEWPORT / 2 < slotUIList.Count) && (itemListRect.localPosition.y <= (slotUIList[0].Height * (slotUIList.Count - ITEMS_IN_VIEWPORT)) - HEIGHT_OFFSET);
        downArrow.gameObject.SetActive(showDownArrow);
    }
}