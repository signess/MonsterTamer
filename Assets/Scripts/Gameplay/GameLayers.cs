using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers Instance { get; private set; }

    [SerializeField] LayerMask unwalkableLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;

    public LayerMask UnwalkableLayer { get => unwalkableLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => grassLayer; }

    private void Awake()
    {
        Instance = this;
    }
}
