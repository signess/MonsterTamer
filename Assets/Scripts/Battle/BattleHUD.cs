using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private Image expBar;

    [SerializeField] private Color psnColor;
    [SerializeField] private Color brnColor;
    [SerializeField] private Color slpColor;
    [SerializeField] private Color parColor;
    [SerializeField] private Color frzColor;

    private Monster _monster;
    private Dictionary<ConditionID, Color> statusColors;

    public void SetData(Monster monster)
    {
        if(_monster != null)
        {
            _monster.OnHPChanged -= UpdateHP;
            _monster.OnStatusChanged -= SetStatusText;
        }
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
        _monster.OnHPChanged += UpdateHP;
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

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPAsync((float)_monster.HP / _monster.MaxHp);
    }

    public IEnumerable WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }
}