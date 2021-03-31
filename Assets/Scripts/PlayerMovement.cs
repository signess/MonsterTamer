using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event Action OnEncountered;

    private InputManager inputManager;
    private Animator animator;
    [SerializeField] float moveSpeed;
    [SerializeField] LayerMask unwalkableLayer;
    [SerializeField] LayerMask grassLayer;
    private bool isMoving;
    [SerializeField] private Vector2 input;
    private void Start()
    {
        inputManager = InputManager.Instance;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        MovePlayer(Time.deltaTime);
    }

    public void GetInput()
    {
        input.x = inputManager.GetPlayerMovement().x;
        input.y = inputManager.GetPlayerMovement().y;
    }

    private void MovePlayer(float deltaTime)
    {
        if (!isMoving)
        {
            if (input.x != 0f)
            {
                input.y = 0;
                if (input.x < 0)
                    input.x = -1.0f;
                else
                    input.x = 1.0f;
            }
            else if (input.y != 0)
            {
                input.x = 0;
                if (input.y < 0)
                    input.y = -1.0f;
                else
                    input.y = 1.0f;
            }

            if (input != Vector2.zero)
            {
                animator.SetFloat("horizontal", input.x);
                animator.SetFloat("vertical", input.y);
                Vector2 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos, input))
                    StartCoroutine(Move(targetPos, deltaTime));
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    private IEnumerator Move(Vector3 targetPos, float deltaTime)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPos - (Vector3)direction, direction, 1f, unwalkableLayer);
        Debug.DrawRay(targetPos - (Vector3)direction, direction, Color.red);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.ToString());
            return false;
        }
        return true;
        // if (Physics2D.OverlapCircle(targetPos, 0.2f, unwalkableLayer) != null)
        // {
        //     return false;
        // }
        // return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                input = Vector2.zero;
                OnEncountered();
            }
        }
    }

}
