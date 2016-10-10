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
		if(_stateManager.get_state(this.gameObject, false) == StateManager.State.NEUTRAL)
			current_state = new NeutralDog();
		if(_stateManager.get_state(this.gameObject, false) == StateManager.State.ALERT)
			current_state = new AlertDog();
		if(_stateManager.get_state(this.gameObject, false) == StateManager.State.AGGRESIVE)
			current_state = new AggresiveDog();

		current_state.execute();
	}

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
