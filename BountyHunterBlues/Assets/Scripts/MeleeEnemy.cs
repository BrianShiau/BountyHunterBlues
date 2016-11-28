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
    public Animator shield_animation;
	private int shieldStack;

	public override void Start(){
		base.Start();
        current_state = new NeutralMelee(this);
        dash_cr_running = false;
        spin_cr_running = false;
        //audioManager.setLoop("Feet", true);
        foreach(Animator anim in GetComponentsInChildren(typeof(Animator))){
            if(anim.name == "ShieldHit"){
                shield_animation = anim;
            }
        }
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
            this.gameObject.transform.GetChild(6).gameObject.active = true;
            meleeAttack();
            yield return null;
        }
        this.gameObject.transform.GetChild(6).gameObject.active = false;
        gameActorAnimator.SetBool("isAttack", false);
        set_attacking(false);
        spin_cr_running = false;
        spin_time = 0;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(spin_cr_running){
            if(collision.gameObject.name == "0_Player"){
                get_player_actor().takeDamage();
            }
        }
    }

    public override void meleeAttack(){
        
    }

    public override void interact(){
        throw new NotImplementedException();
    }

	public override void EndInteract(){
		throw new NotImplementedException();
	}

    public override void die(){
        base.die();
		transform.FindChild ("Laser").gameObject.SetActive(false);
		StartCoroutine(DeathCleanUp());
    }

    public override void takeDamage(int damage = 1){
        if((int)currDirection == 2 && get_player_actor().get_current_direction() == 0){
            shield_animation.SetBool("Hit", true);
			Invoke ("resetShieldAnim", .01f);
			shieldStack++;
            //blocksound here
        }
        else if((int)currDirection == 0 && get_player_actor().get_current_direction() == 2){
            shield_animation.SetBool("Hit", true);
			Invoke ("resetShieldAnim", .01f);
			shieldStack++;
            //blocksound here
        }
        else if((int)currDirection == 3 && get_player_actor().get_current_direction() == 1){
            shield_animation.SetBool("Hit", true);
			Invoke ("resetShieldAnim", .01f);
			shieldStack++;
            //blocksound here
        }
        else if((int)currDirection == 1 && get_player_actor().get_current_direction() == 3){
            shield_animation.SetBool("Hit", true);
			Invoke ("resetShieldAnim", .01f);
			shieldStack++;
            //blocksound here
        }
        else if(closestAttackable != null){
            shield_animation.SetBool("Hit", true);
			Invoke ("resetShieldAnim", .01f);
			shieldStack++;
            //blocksound here
        }
        else{
            base.takeDamage(damage);
        }

    }

	private void resetShieldAnim(){
		if (--shieldStack == 0) {
			shield_animation.SetBool ("Hit", false);
		}
	}

	private IEnumerator DeathCleanUp()
	{
		//audioManager.Play("Death");
		yield return new WaitForSeconds(.1f);
		gameActorAnimator.SetBool ("isDead", true);
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
