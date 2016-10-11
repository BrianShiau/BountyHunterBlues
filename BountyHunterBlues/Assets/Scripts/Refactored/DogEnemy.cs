using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class DogEnemy : EnemyActor {

	public override void Start(){
		base.Start();
	}

	public override void Update(){
		base.Update();
        //current_state = new AlertDog();
        //current_state = new AggresiveDog();
        //execute_neutral();
        if(_stateManager.get_state(closestAttackable, false) == StateManager.State.NEUTRAL)
            current_state = new NeutralDog(closestAttackable, this);
        if(_stateManager.get_state(closestAttackable, false) == StateManager.State.ALERT)
            execute_alert();
        if(_stateManager.get_state(closestAttackable, false) == StateManager.State.AGGRESIVE)
            execute_aggresive();


		current_state.execute(this.gameObject);
	}


    //public void execute_neutral(){
    //    stopMove();
    //    Vector2 worldFaceDir = closestAttackable.gameObject.transform.position - transform.position;
    //    worldFaceDir.Normalize();
//
    //    Vector2 localFaceDir = transform.InverseTransformDirection(worldFaceDir);
    //    Vector2 dir = Vector2.MoveTowards(faceDir, localFaceDir, rotation_speed * Time.deltaTime);
    //    dir.Normalize();
    //    faceDir = dir;
    //}

    public void execute_alert(){}
    public void execute_aggresive(){}

	public override void rangedAttack(){
        if (closestAttackable is PlayerActor){
            hasAttacked = true;
            closestAttackable.takeDamage();
            if (!closestAttackable.isAlive())
                closestAttackable = null;
        }
        else
            Debug.Log("AI can't attack other AI");
    }

    public override void meleeAttack(){
        throw new NotImplementedException();
    }

    public override void interact(){
        throw new NotImplementedException();
    }

    public override void runAnimation(){
    	base.runAnimation();
    	BarrelBase bBase = GetComponentInChildren<BarrelBase>();
    	Animator EnemyBarrelAnimator = bBase.GetComponentInChildren<Animator>();
    	if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
        {
            if (faceDir.y > 0)
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.UP);
                bBase.facing = Direction.UP;
            }
            else
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.DOWN);
                bBase.facing = Direction.DOWN;
            }
        }

        else
        {
            if (faceDir.x > 0)
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.RIGHT);
                bBase.facing = Direction.RIGHT;
            }
            else
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.LEFT);
                bBase.facing = Direction.LEFT;
            }
        }
    }
}
