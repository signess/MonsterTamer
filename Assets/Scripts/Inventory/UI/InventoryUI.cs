using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy }

public class InventoryUI : MonoBehaviour
{
    private Action backAction;
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    [SerializeField] private PartyScreen partyScreen;

    private int selectedItem = 0;
    private InventoryUIState state;

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
        inventory.OnUpdated += UpdateItemList;
    }

    public void HandleUpdate(Action onBack)
    {
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

            if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (backAction != onBack)
                backAction = onBack;

            if (Input.GetKeyDown(KeyCode.Z))
                OpenPartyScreen();
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();

        }
        else if(state == InventoryUIState.PartySelection)
        {
            Action onSelectedPartyScreen = () =>
            {
                // Use the item on the selected pokemon
                StartCoroutine(UseItem());

            };
            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelectedPartyScreen, onBackPartyScreen);
        }
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
        if (state == InventoryUIState.ItemSelection)
        {
            if (this.selectedItem != selectedItem)
            {
                this.selectedItem = selectedItem;
                UpdateItemSelection();
            }
            else if (this.selectedItem == selectedItem)
                OpenPartyScreen();
        }
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
        if (slotUIList.Count <= ITEMS_IN_VIEWPORT) return;

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

    private void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OpenPartyScreen();
    }

    private void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.ClosePartyScreen();
    }

    private IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember);
        if(usedItem != null)
        {
            yield return DialogManager.Instance.ShowDialogText($"{usedItem.Name} was used!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("It won't have any effect!");
        }

        ClosePartyScreen();
    }
}