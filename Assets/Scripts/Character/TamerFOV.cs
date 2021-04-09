using UnityEngine;

public class TamerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.OnEnterTamersView(GetComponentInParent<TamerController>());
    }
}
