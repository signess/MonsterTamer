public class Move
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }
    public MoveType MoveType { get; set; }

    public Move(MoveBase moveBase)
    {
        Base = moveBase;
        PP = Base.PP;
        MoveType = Base.MoveType;
    }
}
