using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] private Image battleSprite;
    Vector3 originalPosition;
    Color originalColor;

    public Monster Monster { get; set; }

    private void Awake()
    {
        battleSprite = GetComponent<Image>();
        originalPosition = battleSprite.transform.localPosition;
        originalColor = battleSprite.color;
    }
    public void Setup(Monster monster)
    {
        Monster = monster;

        if (isPlayerUnit)
            battleSprite.sprite = Monster.Base.BackSprite;
        else
            battleSprite.sprite = Monster.Base.FrontSprite;

        battleSprite.color = originalColor;
        PlayerEnterAnimation();
    }

    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            battleSprite.transform.localPosition = new Vector3(-500f, originalPosition.y);
        else
            battleSprite.transform.localPosition = new Vector3(500, originalPosition.y);

        battleSprite.transform.DOLocalMoveX(originalPosition.x, 1f);
    }

    public IEnumerator PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x + 50f, 0.25f));
        else
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x - 50f, 0.25f));

        sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x, 0.25f));
        yield return sequence.Play().WaitForCompletion();
    }

    public IEnumerator PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(battleSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(battleSprite.DOColor(originalColor, 0.1f));
        sequence.Append(battleSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(battleSprite.DOColor(originalColor, 0.1f));
        sequence.Append(battleSprite.DOColor(Color.gray, 0.1f));
        sequence.Append(battleSprite.DOColor(originalColor, 0.1f));
        yield return sequence.Play().WaitForCompletion();
    }

    public IEnumerator PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(battleSprite.transform.DOLocalMoveY(originalPosition.y - 150f, 0.5f));
        sequence.Join(battleSprite.DOFade(0f, 0.5f));
        yield return sequence.Play().WaitForCompletion();
    }
}
