using System.Collections.Generic;
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
    public void Setup(Monster monster, bool isWild = false)
    {
        Monster = monster;

        if (isPlayerUnit)
            battleSprite.sprite = Monster.Base.BackSprite;
        else
            battleSprite.sprite = Monster.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(monster);

        ResetSprite();

        if (isWild)
            StartCoroutine(PlayWildEnterAnimation());
        else
            StartCoroutine(CaptureDiskEnterAnimation());
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

    public IEnumerator PlayEnterAnimation()
    {
        if (isPlayerUnit)
            battleSprite.transform.localPosition = new Vector3(-4, originalPosition.y);
        else
            battleSprite.transform.localPosition = new Vector3(4, originalPosition.y);

        yield return battleSprite.transform.DOLocalMoveX(originalPosition.x, 1f).WaitForCompletion();
    }

    public IEnumerator CaptureDiskEnterAnimation()
    {
        battleSprite.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        battleSprite.transform.localPosition = originalPosition;
        var tmpColor = battleSprite.color;
        tmpColor = Color.black;
        tmpColor.a = 0f;
        battleSprite.color = tmpColor;

        Sequence enterSequence = DOTween.Sequence();
        enterSequence.Append(battleSprite.transform.DOScale(Vector3.one, 0.5f));
        enterSequence.Join(battleSprite.DOColor(originalColor, 0.5f));
        yield return enterSequence.Play().WaitForCompletion();
    }

    public IEnumerator PlayWildEnterAnimation()
    {
        yield return battleSprite.transform.DOPunchRotation(new Vector3(0f, 0f, 30f), 1.5f).WaitForCompletion();
    }

    public IEnumerator PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x + 1, 0.25f));
        else
            sequence.Append(battleSprite.transform.DOLocalMoveX(originalPosition.x - 1, 0.25f));

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
        sequence.Append(battleSprite.transform.DOLocalMoveY(originalPosition.y - 1.5f, 0.5f));
        sequence.Join(battleSprite.DOFade(0f, 0.5f));
        yield return sequence.Play().WaitForCompletion();
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(battleSprite.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPosition.y + 1f, 0.5f));
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
