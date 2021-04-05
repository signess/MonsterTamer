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

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () => { idleTimer = 0f; state = NPCState.Idle; }));
        }
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;
        var oldPos = transform.position;
        yield return character.Move(movementPattern[currentPattern]);
        if (transform.position != oldPos)
        {
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        }
        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog }
