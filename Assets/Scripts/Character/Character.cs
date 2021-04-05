using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    private CharacterAnimator animator;
    public CharacterAnimator Animator { get => animator; }
    public bool IsMoving { get; private set; }
    private void Awake()
    {
        animator = GetComponentInChildren<CharacterAnimator>();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);
        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos)) yield break;

        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.Instance.UnwalkableLayer | GameLayers.Instance.InteractableLayer | GameLayers.Instance.PlayerLayer))
        {
            return false;
        }
        return true;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.4f, GameLayers.Instance.UnwalkableLayer | GameLayers.Instance.InteractableLayer))
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);
        if (yDiff == 0 | xDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
            Debug.Log("Dir error!");
    }
}
