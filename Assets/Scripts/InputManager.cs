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
}
