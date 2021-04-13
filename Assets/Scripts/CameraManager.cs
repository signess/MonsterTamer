using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] CinemachineVirtualCamera overworldCamera;
    [SerializeField] CinemachineVirtualCamera battleMainCamera;
    [SerializeField] CinemachineVirtualCamera battleEnemyCamera;
    [SerializeField] CinemachineVirtualCamera battlePlayerCamera;
    [SerializeField] CinemachineVirtualCamera battlePlayerCharacterCamera;

    public CinemachineVirtualCamera OverworldCamera { get => overworldCamera; }
    public CinemachineVirtualCamera BattleMainCamera { get => battleMainCamera; }
    public CinemachineVirtualCamera BattleEnemyCamera { get => battleEnemyCamera; }
    public CinemachineVirtualCamera BattlePlayerCamera { get => battlePlayerCamera; }
    public CinemachineVirtualCamera BattlePlayerCharacterCamera { get => battlePlayerCharacterCamera; }

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchPriority(CinemachineVirtualCamera to)
    {
        overworldCamera.Priority = 0;
        battleEnemyCamera.Priority = 0;
        battleMainCamera.Priority = 0;
        battlePlayerCamera.Priority = 0;
        battlePlayerCharacterCamera.Priority = 0;
        to.Priority = 1;
    }
}
