using System.Net;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;

    private InputManager inputManager;
    private Character character;

    [HideInInspector] public Vector2 Input;
    [SerializeField] private bool interact;

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
    public Character Character { get => character; }

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
        Input.x = inputManager.GetPlayerMovement().x;
        Input.y = inputManager.GetPlayerMovement().y;
        interact = inputManager.GetPlayerInteract();
    }

    private void MovePlayer(float deltaTime)
    {
        if (!character.IsMoving)
        {
            if (Input.x != 0f)
            {
                Input.y = 0;
                if (Input.x < 0)
                    Input.x = -1.0f;
                else
                    Input.x = 1.0f;
            }
            else if (Input.y != 0)
            {
                Input.x = 0;
                if (Input.y < 0)
                    Input.y = -1.0f;
                else
                    Input.y = 1.0f;
            }

            if (Input != Vector2.zero)
            {
                StartCoroutine(character.Move(Input, OnMoveOver));
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
            collider.GetComponent<IInteractable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, GameLayers.Instance.TriggerableLayers);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            monsters = GetComponent<MonsterParty>().Monsters.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var savedData = (PlayerSaveData)state;
        //Restore position
        var pos = savedData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        //Restore party
        GetComponent<MonsterParty>().Monsters = savedData.monsters.Select(s => new Monster(s)).ToList();
    }
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<MonsterSaveData> monsters;
}
