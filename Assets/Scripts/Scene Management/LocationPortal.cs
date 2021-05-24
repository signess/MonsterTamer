using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    public Transform SpawnPoint => spawnPoint;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return Fader.Instance.FadeIn(.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return Fader.Instance.FadeOut(.5f);
        GameController.Instance.PauseGame(false);
    }
}
