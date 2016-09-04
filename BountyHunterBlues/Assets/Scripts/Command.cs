using UnityEngine;
using System.Collections;
using System;

public abstract class Command : MonoBehaviour {

    public abstract void execute(GameActor actor);

}

public class MoveCommand : Command
{
    public override void execute(GameActor actor)
    {

    }
}

public class MeleeAttackCommand : Command
{
    public override void execute(GameActor actor)
    {

    }
}

public abstract class RangedAttackCommand : Command
{
}

public class RangedShotCommand : Command
{
    public override void execute(GameActor actor)
    {
        throw new NotImplementedException();
    }
}

public class RangedSpecialCommand : Command
{
    public override void execute(GameActor actor)
    {
        throw new NotImplementedException();
    }
}

public class InteractCommand : Command
{
    public override void execute(GameActor actor)
    {
        
    }
}

public class AimCommand : Command
{
    public override void execute(GameActor actor)
    {
        
    }
}
