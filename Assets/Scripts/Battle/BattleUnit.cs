using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    public bool IsPlayerUnit
    { get => isPlayerUnit; }
    [SerializeField] BattleHUD hud;
    public BattleHUD HUD { get => hud; }
    [SerializeField] private SpriteRenderer battleSprite;
    Vector3 originalPosition;
    Color originalColor;

    public Monster Monster { get; set; }

    private void Awake()
    {
        battleSprite = GetComponent<SpriteRenderer>();
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

        hud.gameObject.SetActive(true);
        hud.SetData(monster);

        ResetSprite();

        PlayerEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void ResetSprite()
    {
        transform.localScale = Vector3.one;
        battleSprite.color = originalColor;
    }

    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            battleSprite.transform.localPosition = new Vector3(-4, originalPosition.y);
        else
            battleSprite.transform.localPosition = new Vector3(4, originalPosition.y);

        battleSprite.transform.DOLocalMoveX(originalPosition.x, 1f);
    }

    public IEnumerator PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x + 3, 0.25f));
        else
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x - 3, 0.25f));

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
        sequence.Append(battleSprite.transform.DOLocalMoveY(originalPosition.y - 4f, 0.5f));
        sequence.Join(battleSprite.DOFade(0f, 0.5f));
        yield return sequence.Play().WaitForCompletion();
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(battleSprite.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y + 3f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(battleSprite.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}
