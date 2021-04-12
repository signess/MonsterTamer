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

    public CinemachineVirtualCamera OverworldCamera { get => overworldCamera; }
    public CinemachineVirtualCamera BattleMainCamera { get => battleMainCamera; }
    public CinemachineVirtualCamera BattleEnemyCamera { get => battleEnemyCamera; }
    public CinemachineVirtualCamera BattlePlayerCamera { get => battlePlayerCamera; }

    private CinemachineVirtualCamera actualCamera;


    private void Awake()
    {
        Instance = this;
        actualCamera = overworldCamera;
    }

    public void SwitchPriority(CinemachineVirtualCamera from, CinemachineVirtualCamera to)
    {
        from.Priority = 0;
        to.Priority = 1;
    }
}
