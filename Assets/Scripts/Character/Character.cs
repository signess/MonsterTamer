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

        if (!IsWalkable(targetPos, moveVector)) yield break;

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

    private bool IsWalkable(Vector3 targetPos, Vector2 direction)
    {
        // RaycastHit2D hit = Physics2D.Raycast(targetPos, direction, .5f, GameLayers.Instance.UnwalkableLayer | GameLayers.Instance.InteractableLayer);
        // Debug.DrawRay(targetPos, direction, Color.red);

        // if (hit.collider != null)
        // {
        //     return false;
        // }
        // return true;


        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.UnwalkableLayer | GameLayers.Instance.InteractableLayer))
        {
            return false;
        }
        return true;
    }
}
