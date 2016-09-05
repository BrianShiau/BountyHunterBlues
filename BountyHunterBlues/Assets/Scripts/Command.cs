using UnityEngine;
using System.Collections;
using System;

public abstract class Command {

    public abstract void execute(GameActor actor);
    public virtual void updateCommandData(Vector3 data)
    {
        // by default, simply ignore the data
    }

}

public class MoveCommand : Command
{

    Vector3 dir;

    public MoveCommand(Vector3 dir)
    {
        this.dir = dir;
    }
    
    public override void execute(GameActor actor)
    {
        actor.move(dir);
    }

    public override void updateCommandData(Vector3 data)
    {
        dir = data;
    }
}

public class AttackCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.attack();
    }
}

public class InteractCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.interact();
    }
}

public class AimCommand : Command
{
    Vector3 dir;

    public AimCommand(Vector3 dir)
    {
        this.dir = dir;
    }

    public override void execute(GameActor actor)
    {
        actor.aim(dir);
    }

    public override void updateCommandData(Vector3 data)
    {
        dir = data;
    }
}

public class DisableAimCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.disableAim();
    }
}
