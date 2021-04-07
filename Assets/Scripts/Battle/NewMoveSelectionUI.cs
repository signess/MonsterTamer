using System;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewMoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] List<TextMeshProUGUI> ppTexts;
    [SerializeField] List<TextMeshProUGUI> typeTexts;

    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].Name;
            typeTexts[i].text = currentMoves[i].Type.ToString();
            ppTexts[i].text = $"{currentMoves[i].PP}/{currentMoves[i].PP} PP";
        }

    }

    public void SelectedMove(int selectedMove, Action<int> onSelected)
    {
        currentSelection = selectedMove;
        onSelected?.Invoke(currentSelection);
    }
}
