using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] private Image battleSprite;

    public Monster Monster { get; set; }

    private void Awake()
    {
        battleSprite = GetComponent<Image>();
    }
    public void Setup()
    {
        Monster = new Monster(_base, level);

        if (isPlayerUnit)
            battleSprite.sprite = Monster.Base.BackSprite;
        else
            battleSprite.sprite = Monster.Base.FrontSprite;
    }
}
