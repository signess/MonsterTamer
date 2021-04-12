using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator animator;
    private bool overworldCamera = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SwitchState();
        }
    }

    private void SwitchState()
    {
        if (overworldCamera)
        {
            animator.Play("BattleCamera");
        }
        else
        {
            animator.Play("OverworldCamera");
        }
        overworldCamera = !overworldCamera;
    }
}
