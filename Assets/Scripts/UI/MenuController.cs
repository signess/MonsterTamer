using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    public event Action<int> OnMenuSelected;
    [SerializeField] GameObject menu;
    public void ToogleMenu()
    {
        StartCoroutine(MenuAnimation());
    }

    IEnumerator MenuAnimation()
    {
        if(!menu.activeInHierarchy)
        {
            GameController.Instance.PauseGame(true, GameState.Menu);
            menu.SetActive(true);
            yield return menu.transform.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        }
        else
        {
            yield return menu.transform.DOLocalMoveX(700, .5f).SetEase(Ease.OutSine).WaitForCompletion();
            menu.gameObject.SetActive(false);
            GameController.Instance.PauseGame(false);
        }
    }

    public void MenuButtonSelected(int i)
    {
        OnMenuSelected?.Invoke(i);
    }
}
