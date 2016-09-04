using UnityEngine;
using System.Collections;
using System;

public abstract class Command : MonoBehaviour {

    public abstract void execute();

}

public class MoveCommand : Command
{
    public override void execute()
    {

    }
}

public class MeleeAttackCommand : Command
{
    public override void execute()
    {

    }
}

public abstract class RangedAttackCommand : Command
{
}

public class RangedShotCommand : Command
{
    public override void execute()
    {
        throw new NotImplementedException();
    }
}

public class RangedSpecialCommand : Command
{
    public override void execute()
    {
        throw new NotImplementedException();
    }
}

public class InteractCommand : Command
{
    public override void execute()
    {
        
    }
}

public class AimCommand : Command
{
    public override void execute()
    {
        
    }
}
