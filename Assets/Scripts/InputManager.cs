using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get => _instance;
    }
    private PlayerControls playerControls;
    private void Awake()
    {
        playerControls = new PlayerControls();
        if (_instance == null)
        {
            _instance = FindObjectOfType<InputManager>();
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public bool GetPlayerInteract()
    {
        return playerControls.Player.Interaction.triggered;
    }

    public Vector2 GetBattleMoveInput()
    {
        return playerControls.Battle.Move.ReadValue<Vector2>();
    }

    public bool GetBattleConfirmInput()
    {
        return playerControls.Battle.Confirm.triggered;
    }

    public bool GetBattleCancelInput()
    {
        return playerControls.Battle.Cancel.triggered;
    }
}
