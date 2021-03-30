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
    [SerializeField] int pp;
    [SerializeField] MoveCategory moveCategory;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

    public string Name { get => name; }
    public string Description { get => description; }
    public MonsterType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int PP { get => pp; }
    public MoveCategory MoveCategory { get => moveCategory; }
    public MoveEffects Effects { get => effects; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoosts> boosts;
    [SerializeField] ConditionID status;
    public List<StatBoosts> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
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