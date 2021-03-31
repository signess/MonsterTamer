using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    private NPCState state;
    private float idleTimer = 0f;
    private int currentPattern = 0;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (DialogManager.Instance.IsShowing) return;

        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    public void Interact()
    {
        if (state == NPCState.Idle)
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;
        yield return character.Move(movementPattern[currentPattern]);
        currentPattern = (currentPattern + 1) % movementPattern.Count;
        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking }
