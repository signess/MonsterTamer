using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;

    Monster _monster;

    public void Init(Monster monster)
    {
        _monster = monster;
        UpdateData();

        _monster.OnHPChanged += UpdateData;
    }

    private void UpdateData()
    {
        icon.sprite = _monster.Base.Icon;
        nameText.text = _monster.Base.Name;
        levelText.text = "Lvl " + _monster.Level;
        hpBar.SetHP((float)_monster.HP / _monster.MaxHp);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = GlobalSettings.Instance.HighlightedColor;
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
        }
        else
        {
            nameText.color = Color.white;
            transform.DOScale(Vector3.one, 0.5f * Time.deltaTime).SetEase(Ease.OutSine);
        }
    }
}
