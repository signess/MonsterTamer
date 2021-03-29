using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int level;
    public MonsterBase Base { get => _base; }
    public int Level { get => level; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public void Init()
    {
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

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = 0f;
        float defense = 0f;
        if (move.MoveType == MoveType.PHYSICAL)
        {
            attack = attacker.Attack;
            defense = Defense;
        }
        else if (move.MoveType == MoveType.SPECIAL)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;
        }

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
