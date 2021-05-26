using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters;
    public List<Monster> Monsters
    {
        get => monsters;
        set => monsters = value;
    }

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

    public void AddMonster(Monster newMonster)
    {
        if (monsters.Count < 6)
        { monsters.Add(newMonster); }
        else
        {
            //ADD TO PC
        }
    }
}
