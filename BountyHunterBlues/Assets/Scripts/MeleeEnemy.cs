using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;



public class SpinExplosion : Explosion {

    public override void Start()
    {
        base.Start();
        OnExplosionEnd();
    }

    public static SpinExplosion Create(GameObject prefab, Vector3 position)
    {
        GameObject explosionObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        return explosionObj.GetComponent<SpinExplosion>();
    }

    protected override bool isValidHit(GameActor hitActor)
    {
        return hitActor is PlayerActor;
    }

    protected override void explosionHit(GameActor hitActor)
    {
        hitActor.takeDamage();
    }
}

public class MeleeEnemy : EnemyActor {

	private bool dash_cr_running;
	private bool spin_cr_running;
	private float spin_time;
	public float spin_time_threshold; 
	public GameObject MissileObject;
	public float dash_speed;


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
		_stateManager.update_state (is_attacking(), player_is_cloaked(), closestAttackable, sound_heard (), is_alert ());
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
        //Debug.Log(is_attacking());
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

    public override void rangedAttack(){
        StartCoroutine(DashAttack());  
    }

    public void reset_spin_timer(){
        spin_time = 0;
    }

    private IEnumerator DashAttack(){
        set_attacking(true);
        dash_cr_running = true;
        gameActorAnimator.SetBool("isAttack", true);
        while(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), get_last_seen()) > 0.3f){
            Vector2 temp = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), get_last_seen(), dash_speed * Time.deltaTime);
            transform.position = new Vector3(temp.x, temp.y, transform.position.z);

            if(Vector2.Distance(get_player_object().transform.position, transform.position) < 2f){
                get_player_actor().takeDamage();
            }
            yield return null;
        }
        dash_cr_running = false;

        spin_cr_running = true;
        
        spin_time += Time.deltaTime;    
        while(spin_time < spin_time_threshold){
            meleeAttack();
            yield return null;
        }
        gameActorAnimator.SetBool("isAttack", false);
        set_attacking(false);
        spin_cr_running = false;
        spin_time = 0;
    }

    public override void meleeAttack(){
        SpinExplosion obj = SpinExplosion.Create(MissileObject, transform.position);
    }

    public override void interact(){
        throw new NotImplementedException();
    }

	public override void EndInteract(){
		throw new NotImplementedException();
	}

    public override void die(){
        base.die();
        Destroy(gameObject);
    }

    public override void takeDamage(int damage = 1){
        if((int)currDirection == 2 && get_player_actor().get_current_direction() == 0){
            //animation here
        }
        else if((int)currDirection == 0 && get_player_actor().get_current_direction() == 2){
            //animation here
        }
        else if((int)currDirection == 3 && get_player_actor().get_current_direction() == 1){
            //animation here
        }
        else if((int)currDirection == 1 && get_player_actor().get_current_direction() == 3){
            //animation here
        }
        else{
            base.takeDamage(damage);
        }

    }
}
