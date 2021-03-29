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
    [SerializeField] MoveType moveType;

    public string Name { get => name; }
    public string Description { get => description; }
    public MonsterType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int PP { get => pp; }
    public MoveType MoveType { get => moveType; }
}

[System.Serializable]
public enum MoveType
{
    PHYSICAL, SPECIAL, STATUS
}