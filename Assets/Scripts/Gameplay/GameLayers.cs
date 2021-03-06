using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers Instance { get; private set; }

    [SerializeField] LayerMask unwalkableLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;

    public LayerMask UnwalkableLayer { get => unwalkableLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => grassLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask FovLayer { get => fovLayer; }
    public LayerMask PortalLayer { get => portalLayer; }
    public LayerMask TriggerableLayers { get => grassLayer | fovLayer | portalLayer; }

    private void Awake()
    {
        Instance = this;
    }
}
