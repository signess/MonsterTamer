using System.Collections.Generic;
public class ConditionsDB
{
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
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
