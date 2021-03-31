using System;
using System.Collections.Generic;
public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn, new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp/8);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} hurt itself due to poison.");
                }
            }
        },
        {
             ConditionID.brn, new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned.",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp/16);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} hurt itself due to burn.");
                }
            }
        },
        {
             ConditionID.par, new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed.",
                OnBeforeMove = (Monster monster) =>
                {
                    if(UnityEngine.Random.Range(1,5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.Base.Name}'s paralyzed and can't move.");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
             ConditionID.frz, new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen.",
                OnBeforeMove = (Monster monster) =>
                {
                    if(UnityEngine.Random.Range(1,5) == 1)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is not frozen anymore.");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
             ConditionID.slp, new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep.",
                OnStart = (Monster monster) =>
                {
                    //Sleep for 1-3 turns
                    monster.StatusTime = UnityEngine.Random.Range(1,4);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.StatusTime <= 0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} woke up!");
                        return true;
                    }
                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is sleeping.");
                    return false;
                }
            }
        },
        //VOLATILE STATUS
        {
             ConditionID.confusion, new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused.",
                OnStart = (Monster monster) =>
                {
                    //Confused for 1-4 turns
                    monster.VolatileStatusTime = UnityEngine.Random.Range(1,5);
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} snapped out of confusion!");
                        return true;
                    }
                    monster.VolatileStatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is confused.");

                    if(UnityEngine.Random.Range(1,3) == 1)
                        return true;

                    monster.UpdateHP(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue("It hurt itself due to confusion.");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
