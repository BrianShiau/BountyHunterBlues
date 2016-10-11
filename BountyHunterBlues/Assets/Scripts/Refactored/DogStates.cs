using UnityEngine;
using System.Collections;
using System;

public abstract class DogState : AIState
{
    protected Command move;
    protected Command stopMove;
    protected Command rangedAttack;
    protected DogEnemy enemy;
    protected DogState(DogEnemy enemy)
    {
        this.enemy = enemy;
        move = new MoveCommand(new Vector2(0,0));
        stopMove = new MoveStopCommand();
        rangedAttack = new RangedAttackCommand();
    }
    public abstract void on_enter();
    public abstract void on_exit();
    public abstract void execute();
    public abstract string name();
    public abstract State getState();
}

public class NeutralDog: DogState {


	public NeutralDog(DogEnemy enemy) : base(enemy) {}

	public override void on_enter(){ Debug.Log("enter neutral"); }

	public override void on_exit(){ Debug.Log("exit neutral"); }

	public override void execute(){
        enemy.calc_shortest_path(enemy.transform.position, enemy.get_neutral_position());
    	if(enemy.getClosestAttackable() != null){
			Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
	        worldFaceDir.Normalize();

	        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
	        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
	        dir.Normalize();
	        enemy.faceDir = dir;
    	}
    	else if(enemy.get_path_index() < enemy.path_length()){
        	Node current_node = enemy.path.get_node(enemy.get_path_index());
        	float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);
                
            Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            worldFace.Normalize();
            enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);
            move.updateCommandData(enemy.faceDir);
            move.execute(enemy);
            if(distance_from_node < enemy.get_node_transition_threshold()){
                enemy.inc_path_index();   
            }
        }
    	else{
    		enemy.path.clear();
    		enemy.reset_path_index();
			Vector2 temp = Vector2.MoveTowards(enemy.faceDir, enemy.get_initial_faceDir(), enemy.rotation_speed * Time.deltaTime);
            temp.Normalize();
            enemy.faceDir = temp;
            stopMove.execute(enemy);
    	}
	}


	public override string name(){
		return "NEUTRAL";
	}

    public override State getState()
    {
        return State.NEUTRAL;
    }
}

public class AlertDog: DogState {

    public AlertDog(DogEnemy enemy) : base(enemy) { }
	public override void on_enter(){ Debug.Log("enter alert"); }

	public override void on_exit(){ Debug.Log("exit alert"); }

	public override void execute(){
		Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
        worldFaceDir.Normalize();

        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
        dir.Normalize();
        enemy.faceDir = dir;

        move.updateCommandData(localFaceDir);
        move.execute(enemy);
	}

	public override string name(){
		return "ALERT";
	}

    public override State getState()
    {
        return State.ALERT;
    }
}

public class AggresiveDog: DogState
{
    public AggresiveDog(DogEnemy enemy) : base(enemy) { }

	public override void on_enter(){}

	public override void on_exit(){}

	public override void execute(){
		Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
        worldFaceDir.Normalize();

        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
        dir.Normalize();
        enemy.faceDir = dir;
	}

	public override string name(){
		return "AGGRESIVE";
	}

    public override State getState()
    {
        return State.AGGRESIVE;
    }
}
