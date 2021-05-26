using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance { get; private set; }

    [SerializeField] Color highlightedColor;

    public Color HighlightedColor => highlightedColor;

    private void Awake()
    {
        Instance = this;
    }
}
