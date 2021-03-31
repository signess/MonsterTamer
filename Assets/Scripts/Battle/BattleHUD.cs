using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] HPBar hpBar;

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
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHP((float)monster.HP / monster.MaxHp);

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

    public IEnumerator UpdateHP()
    {
        if (_monster.HPChanged)
        {
            yield return hpBar.LerpHP((float)_monster.HP / _monster.MaxHp);
            _monster.HPChanged = false;
        }
    }
}
