using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters;

    private void Start()
    {
        foreach (var monster in monsters)
        {
            monster.Init();
        }
    }

    public Monster GetHealthyMonster()
    {
        return monsters.Where(x => x.HP > 0).FirstOrDefault();
    }
}
