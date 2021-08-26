using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image health;

    public bool IsUpdating { get; private set; }

    public void SetHP(float hpNormalized)
    {
        health.fillAmount = hpNormalized;
    }

    public IEnumerator SetHPAsync(float newHP)
    {
        IsUpdating = true;
        yield return health.DOFillAmount(newHP, 1.5f).WaitForCompletion();
        health.fillAmount = newHP;
        IsUpdating = false;

        // float currentHP = health.fillAmount;
        // float changeAmount = currentHP - newHP;
        // while (currentHP - newHP > Mathf.Epsilon)
        // {
        //     currentHP -= changeAmount * Time.deltaTime;
        //     health.fillAmount = changeAmount;
        //     yield return null;
        // }
        // health.fillAmount = newHP;
    }
}
