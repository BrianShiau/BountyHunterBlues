using UnityEngine;
using System.Collections;
using System;

public abstract class Command
{

    public abstract void execute(GameActor actor);
    public virtual void updateCommandData(Vector2 data)
    {
        // by default, simply ignore the data
    }

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

    public override void updateCommandData(Vector2 data)
    {
        dir = data;
    }
}

public class MoveStopCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.stopMove();
    }
}

public class RangedAttackCommand : Command
{
    public override void execute(GameActor actor)
    {
		actor.rangedAttack();
    }
}

public class MeleeAttackCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.meleeAttack();
    }
}

public class InteractCommand : Command
{
    public override void execute(GameActor actor)
    {
        actor.interact();
    }
}

public class LookCommand : Command
{

    Vector2 worldPos;

    public override void updateCommandData(Vector2 data)
    {
        worldPos = data;
    }
    public override void execute(GameActor actor)
    {
        actor.lookAt(worldPos);
    }
}

