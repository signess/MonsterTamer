using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    public MonsterBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Monster(MonsterBase monsterBase, int monsterLevel)
    {
        Base = monsterBase;
        Level = monsterLevel;
        HP = MaxHp;

        //Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4)
                    break;
            }
        }
    }

    public int MaxHp
    {
        get => Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
    }

    public int Attack
    {
        get => Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5;
    }

    public int Defense
    {
        get => Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5;
    }

    public int SpAttack
    {
        get => Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5;
    }

    public int SpDefense
    {
        get => Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5;
    }

    public int Speed
    {
        get => Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5;
    }
}
