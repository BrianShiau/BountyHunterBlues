using UnityEngine;
using System.Collections;
using System;

public abstract class Command : MonoBehaviour {

    public abstract void execute(GameActor actor);

}

public class MoveCommand : Command
{

    Vector2 dir;

    public MoveCommand(Vector2 dir)
    {
        this.dir = dir;
    }
    
    public override void execute(GameActor actor)
    {
        actor.move(dir);
    }
}

public class AttackCommand : Command
{
    public override void execute(GameActor actor)
    {

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
    Vector2 dir;

    public AimCommand(Vector2 dir)
    {
        this.dir = dir;
    }
    public override void execute(GameActor actor)
    {
        actor.aim(dir);
    }
}

public class LowerAimCommand : Command
{
    public override void execute(GameActor actor)
    {
        
    }
}
