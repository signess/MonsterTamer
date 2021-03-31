using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;

    private InputManager inputManager;
    private Character character;

    [SerializeField] private Vector2 input;
    [SerializeField] private bool interact;

    private void Start()
    {
        inputManager = InputManager.Instance;
        character = GetComponent<Character>();
    }

    private void Update()
    {
        MovePlayer(Time.deltaTime);
    }

    public void GetInput()
    {
        input.x = inputManager.GetPlayerMovement().x;
        input.y = inputManager.GetPlayerMovement().y;
        interact = inputManager.GetPlayerInteract();
    }

    private void MovePlayer(float deltaTime)
    {
        if (!character.IsMoving)
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
                StartCoroutine(character.Move(input, CheckForEncounters));
            }
        }

        character.HandleUpdate();

        if (interact)
            Interact();
    }

    private void Interact()
    {
        interact = false;
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.Instance.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<IInteractable>()?.Interact();
        }
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                input = Vector2.zero;
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

}
