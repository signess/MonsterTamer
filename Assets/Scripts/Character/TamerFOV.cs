using UnityEngine;

public class TamerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterTamersView(GetComponentInParent<TamerController>());
    }
}
