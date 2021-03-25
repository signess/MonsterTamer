using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monsters/Create New Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] MonsterType type1;
    [SerializeField] MonsterType type2;

    [Header("Base Stats")]
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] int baseXp;
    [SerializeField] int levelUpMultiplier;
    [SerializeField] float catchRate;

    [SerializeField] List<LearnableMove> learnableMoves;

    //Properties
    public string Name
    {
        get => name;
    }

    public string Description
    {
        get => description;
    }

    public Sprite FrontSprite
    {
        get => frontSprite;
    }

    public Sprite BackSprite
    {
        get => backSprite;
    }

    public MonsterType Type1
    {
        get => type1;
    }

    public MonsterType Type2
    {
        get => type2;
    }

    public int MaxHp
    {
        get => maxHp;
    }

    public int Attack
    {
        get => attack;
    }

    public int Defense
    {
        get => defense;
    }

    public int SpAttack
    {
        get => spAttack;
    }

    public int SpDefense
    {
        get => spDefense;
    }

    public int Speed
    {
        get => speed;
    }

    public int BaseXp
    {
        get => baseXp;
    }

    public int LevelUpMultiplier
    {
        get => levelUpMultiplier;
    }

    public float CatchRate
    {
        get => catchRate;
    }

    public List<LearnableMove> LearnableMoves { get => learnableMoves; }

}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base { get => moveBase; }
    public int Level { get => level; }
}

public enum MonsterType
{
    None, Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic,
    Bug, Rock, Ghost, Dragon, Steel, Dark, Fairy
}
