using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHUD;

    [Header("Enemy")]
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHUD;


    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerUnit.Setup();
        playerHUD.SetData(playerUnit.Monster);

        enemyUnit.Setup();
        enemyHUD.SetData(enemyUnit.Monster);
    }
}
