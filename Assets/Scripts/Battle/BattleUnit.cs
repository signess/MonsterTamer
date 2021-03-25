using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Monster Monster { get; set; }

    public void Setup()
    {
        new Monster(_base, level);
        if (isPlayerUnit)
            GetComponent<Image>().sprite = Monster.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Monster.Base.FrontSprite;
    }
}
