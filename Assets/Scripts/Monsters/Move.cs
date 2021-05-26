public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = Base.PP;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.Name,
            pp = PP
        };
        return saveData;
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}
