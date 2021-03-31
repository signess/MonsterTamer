using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Monsters/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] MonsterType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

    public string Name { get => name; }
    public string Description { get => description; }
    public MonsterType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public bool AlwaysHits { get => alwaysHits; }
    public int PP { get => pp; }
    public int Priority { get => priority; }
    public MoveCategory MoveCategory { get => moveCategory; }
    public MoveEffects Effects { get => effects; }
    public List<SecondaryEffects> SecondaryEffects { get => secondaryEffects; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoosts> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoosts> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class StatBoosts
{
    public Stat Stat;
    public int Boost;
}

[System.Serializable]
public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}