using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Monster _monster;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Monster monster)
    {
        _monster = monster;

        nameText.text = monster.Base.Name;
        SetLevel();
        hpBar.SetHP((float)monster.HP / monster.MaxHp);
        SetExp();

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor}, {ConditionID.brn, brnColor}, {ConditionID.slp, slpColor},
            {ConditionID.par, parColor}, {ConditionID.frz, frzColor}
        };

        SetStatusText();
        _monster.OnStatusChanged += SetStatusText;
    }

    public void SetStatusText()
    {
        if (_monster.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _monster.Status.ID.ToString().ToUpper();
            statusText.color = statusColors[_monster.Status.ID];
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _monster.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();
        expBar.fillAmount = normalizedExp;
    }

    public IEnumerator SetExpSmooth(bool reset = false)
    {
        if (expBar == null) yield break;
        if (reset)
        {
            expBar.fillAmount = 0;
        }
        float normalizedExp = GetNormalizedExp();
        yield return expBar.DOFillAmount(normalizedExp, 1.5f).WaitForCompletion();
    }

    private float GetNormalizedExp()
    {
        int currLevelExp = _monster.Base.GetExpForLevel(_monster.Level);
        int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Level + 1);

        float normalizedExp = (float)(_monster.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator UpdateHP()
    {
        if (_monster.HPChanged)
        {
            yield return hpBar.LerpHP((float)_monster.HP / _monster.MaxHp);
            _monster.HPChanged = false;
        }
    }
}
