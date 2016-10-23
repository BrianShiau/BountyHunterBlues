using UnityEngine;
using System.Collections;
using System;

public abstract class MissileState : AIState
{
    protected Command move;
    protected Command stopMove;
    protected Command rangedAttack;
    protected MissileEnemy enemy;
    protected MissileState(MissileEnemy enemy)
    {
        this.enemy = enemy;
        move = new MoveCommand(new Vector2(Int32.MaxValue, Int32.MaxValue));
        stopMove = new MoveStopCommand(); ;
        rangedAttack = new RangedAttackCommand();
    }

    public abstract void on_enter();
    public abstract void on_exit();
    public abstract void execute();
    public abstract string name();
    public abstract State get_state();
}

public class NeutralMissile : MissileState
{
    public NeutralMissile(MissileEnemy enemy) : base(enemy) {}

    public override void on_enter()
    {
        enemy.set_shortest_path_calculated(false);
        enemy.calc_shortest_path(enemy.transform.position, enemy.get_neutral_position());
    }

    public override void on_exit()
    {
        enemy.set_shortest_path_calculated(false);
    }

    public override void execute()
    {
        if (enemy.getClosestAttackable() != null)
        {
            Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
            worldFaceDir.Normalize();

            Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
            Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
            dir.Normalize();
            enemy.faceDir = dir;
            stopMove.execute(enemy);
        }
        else if (enemy.get_path_index() < enemy.path_length())
        {
            Node current_node = enemy.path.get_node(enemy.get_path_index());
            float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);

            Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            worldFace.Normalize();
            enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);
            enemy.setIsPatrolling(false);
            move.updateCommandData(enemy.faceDir);
            move.execute(enemy);
            if (distance_from_node < enemy.get_node_transition_threshold())
            {
                enemy.inc_path_index();
            }
        }
        else if (enemy.patrolManager.get_patrol_length() > 0)
        {
            enemy.set_neutral_position(enemy.patrolManager.get_next_patrol_point());
            if (enemy.get_neutral_position().x == Int32.MaxValue && enemy.get_neutral_position().y == Int32.MaxValue)
            {
                stopMove.execute(enemy);
            }
            else
            {
                Vector2 worldFace = enemy.get_neutral_position() - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                worldFace.Normalize();
                enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);
                enemy.setIsPatrolling(true);
                move.updateCommandData(enemy.faceDir);
                move.execute(enemy);
            }
        }
        else
        {
            enemy.path.clear();
            enemy.reset_path_index();
            Vector2 temp = Vector2.MoveTowards(enemy.faceDir, enemy.get_initial_faceDir(), enemy.rotation_speed * Time.deltaTime);
            temp.Normalize();
            enemy.faceDir = temp;
            stopMove.execute(enemy);
        }
    }

    public override string name()
    {
        return "NEUTRAL";
    }

    public override State get_state()
    {
        return State.NEUTRAL;
    }
}

public class AlertMissile : MissileState
{
    public AlertMissile(MissileEnemy enemy) : base(enemy) {}

    public override void on_enter()
    {
        enemy.reset_path_index();
        enemy.set_alert(true);
    }

    public override void on_exit()
    {
        //enemy.set_alert(false);
        enemy.set_last_seen(new Vector2(Int32.MaxValue, Int32.MaxValue));
        enemy.set_audio_location(new Vector2(Int32.MaxValue, Int32.MaxValue));
        enemy.path.clear();
        enemy.reset_path_index();
        enemy.set_chasing(false);
    }

    public override void execute()
    {
        if (enemy.getClosestAttackable() != null)
        {
            enemy.set_alert(true);
            enemy.set_chasing(false);
            Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
            worldFaceDir.Normalize();

            Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
            Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
            dir.Normalize();
            enemy.faceDir = dir;
            move.updateCommandData(localFaceDir);
            move.execute(enemy);
            enemy.set_last_seen(new Vector2(enemy.getClosestAttackable().gameObject.transform.position.x, enemy.getClosestAttackable().gameObject.transform.position.y));
            enemy.path.clear();
            enemy.reset_path_index();
            enemy.set_shortest_path_calculated(false);
        }
        else if (enemy.getClosestAttackable() == null && enemy.get_last_seen().x != Int32.MaxValue && enemy.get_last_seen().y != Int32.MaxValue)
        {
            enemy.set_alert(true);
            enemy.set_chasing(true);
            enemy.set_audio_location(new Vector2(Int32.MaxValue, Int32.MaxValue));
            enemy.calc_shortest_path(enemy.transform.position, enemy.get_last_seen());
            if (enemy.get_path_index() < enemy.path_length())
            {
                Node current_node = enemy.path.get_node(enemy.get_path_index());
                float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);

                Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                worldFace.Normalize();
                enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);

                move.updateCommandData(enemy.faceDir);
                move.execute(enemy);
                if (distance_from_node < enemy.get_node_transition_threshold())
                {
                    enemy.inc_path_index();
                }
            }
            else
            {
                enemy.path.clear();
                enemy.reset_path_index();
                enemy.set_shortest_path_calculated(false);
                enemy.set_last_seen(new Vector2(Int32.MaxValue, Int32.MaxValue));
            }
        }
        else if (enemy.getClosestAttackable() == null && enemy.get_audio_location().x != Int32.MaxValue && enemy.get_audio_location().y != Int32.MaxValue)
        {
            enemy.set_chasing(true);
            enemy.set_last_seen(new Vector2(Int32.MaxValue, Int32.MaxValue));
            if (Vector2.Distance(enemy.get_audio_location(), enemy.gameObject.transform.position) <= enemy.audio_distance)
            {
                if (enemy.get_path_index() < enemy.path_length())
                {
                    Node current_node = enemy.path.get_node(enemy.get_path_index());
                    float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);

                    Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
                    worldFace.Normalize();
                    enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);

                    move.updateCommandData(enemy.faceDir);
                    move.execute(enemy);
                    if (distance_from_node < enemy.get_node_transition_threshold())
                    {
                        enemy.inc_path_index();
                    }
                }
                else
                {
                    enemy.path.clear();
                    enemy.reset_path_index();
                    enemy.set_shortest_path_calculated(false);
                    enemy.set_audio_location(new Vector2(Int32.MaxValue, Int32.MaxValue));
                }
            }
        }
        else
        {
            stopMove.execute(enemy);
            enemy.set_alert(false);
        }
    }

    public override string name()
    {
        return "ALERT";
    }

    public override State get_state()
    {
        return State.ALERT;
    }
}

public class AggresiveMissile : MissileState
{
    public AggresiveMissile(MissileEnemy enemy) : base(enemy) { }

    private float shoot_timer = 0;
    private float shoot_timer_threshold = 1;

    public override void on_enter()
    {
        stopMove.execute(enemy);
    }

    public override void on_exit()
    {
        enemy.set_shortest_path_calculated(false);
        //enemy.set_confused_state(false);
    }

    public override void execute()
    {
        if (enemy.getClosestAttackable() != null)
        {
            Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
            worldFaceDir.Normalize();

            Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
            Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
            dir.Normalize();
            enemy.faceDir = dir;

            shoot_timer += Time.deltaTime;
            if (shoot_timer > shoot_timer_threshold)
            {
                //enemy.set_confused_state(true);
                rangedAttack.execute(enemy);
                shoot_timer = 0;
            }
            enemy.set_last_seen(new Vector2(enemy.getClosestAttackable().gameObject.transform.position.x, enemy.getClosestAttackable().gameObject.transform.position.y));
        }
    }

    public override string name()
    {
        return "AGGRESIVE";
    }

    public override State get_state()
    {
        return State.AGGRESIVE;
    }
}
