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

    public void SetData(Monster monster)
    {
        _monster = monster;
        icon.sprite = monster.Base.Icon;
        nameText.text = monster.Base.Name;
        levelText.text = "Lvl " + monster.Level;
        hpBar.SetHP((float)monster.HP / monster.MaxHp);
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
