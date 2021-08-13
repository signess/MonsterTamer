using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] private List<Monster> monsters;

    public event Action OnUpdated;

    public List<Monster> Monsters
    {
        get => monsters;
        set
        {
            monsters = value;
            OnUpdated?.Invoke();
        }
    }

    public static MonsterParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<MonsterParty>();
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
        {
            monsters.Add(newMonster);
            OnUpdated?.Invoke();
        }
        else
        {
            //ADD TO PC
        }
    }
}