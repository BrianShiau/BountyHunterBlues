using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class MeleeEnemy : EnemyActor {

	private bool dash_cr_running;
	private bool spin_cr_running;


	public override void Start(){
		base.Start();
        current_state = new NeutralMelee(this);
        dash_cr_running = false;
        spin_cr_running = false;
        //audioManager.setLoop("Feet", true);
	}

	public override void Update(){
		if (health <= 0)
			return;
		
		base.Update ();

		is_confused();
		_stateManager.update_state (player_is_cloaked(), closestAttackable, sound_heard (), is_alert ());
		if (current_state.get_state () != _stateManager.get_state ()) {
			current_state.on_exit ();
			if (_stateManager.get_state () == State.NEUTRAL)
				current_state = new NeutralMelee (this);
			if (_stateManager.get_state () == State.ALERT)
				current_state = new AlertMelee (this);
            if (_stateManager.get_state() == State.AGGRESIVE){
                current_state = new AggresiveMelee(this);
                //audioManager.Play("Alert");
            }

			current_state.on_enter ();
		}
		current_state.execute ();

		//if (isMoving) {
		//	if (!audioManager.isPlaying ("Feet")) {
		//		if (isPatrolling)
		//			audioManager.Play ("Feet", "Patrol");
		//		else
		//			audioManager.Play ("Feet", "Chase");
		//	}
//
		//} 
		//else {
		//	audioManager.Stop ("Feet");
		//}
	}

	public bool is_dashing(){
		return dash_cr_running;
	}

	public bool is_spinning(){
		return spin_cr_running;
	}

    public override void rangedAttack()
    {
    	if (closestAttackable is PlayerActor){
        	StartCoroutine(DashAttack());
        }
    }

    private IEnumerator DashAttack(){
    	dash_cr_running = true;
        while(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), get_last_seen()) > 0.01f){
        	Vector2 temp = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), get_last_seen(), 0.5f * Time.deltaTime);
        	transform.position = temp;

        	yield return null;
        }
    	dash_cr_running = false;

        yield return new WaitForSeconds(1f);

        spin_cr_running = true;

        

        spin_cr_running = false;
        //print("melee attack"); 
    }

    public override void meleeAttack(){
        throw new NotImplementedException();
    }

    public override void interact(){
        throw new NotImplementedException();
    }

	public override void EndInteract(){
		throw new NotImplementedException();
	}

    public override void die()
    {
        base.die();
        Destroy(gameObject);
    }
    
}
