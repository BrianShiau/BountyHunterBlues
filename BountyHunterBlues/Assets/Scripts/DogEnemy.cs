using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class DogEnemy : EnemyActor {


	public override void Start(){
		base.Start();
        current_state = new NeutralDog(this);
        audioManager.setLoop("Feet", true);
	}

	public override void Update(){
		if (health <= 0)
			return;
		
		base.Update ();

		is_confused();
		_stateManager.update_state (closestAttackable, sound_heard (), is_alert ());
		if (current_state.get_state () != _stateManager.get_state ()) {
			current_state.on_exit ();
			if (_stateManager.get_state () == State.NEUTRAL)
				current_state = new NeutralDog (this);
			if (_stateManager.get_state () == State.ALERT)
				current_state = new AlertDog (this);
            if (_stateManager.get_state() == State.AGGRESIVE)
            {
                current_state = new AggresiveDog(this);
                audioManager.Play("Alert");
            }

			current_state.on_enter ();
		}
		current_state.execute ();

		if (isMoving) {
			if (!audioManager.isPlaying ("Feet")) {
				if (isPatrolling)
					audioManager.Play ("Feet", "Patrol");
				else
					audioManager.Play ("Feet", "Chase");
			}

		} else {
			audioManager.Stop ("Feet");
		}
	}

	public override void rangedAttack(){
        if (closestAttackable is PlayerActor){
            hasAttacked = true;
            closestAttackable.takeDamage();
            if (!closestAttackable.isAlive())
                closestAttackable = null;
            if (audioManager.isPlaying("Gun"))
                audioManager.Stop("Gun");
            audioManager.Play("Gun");
        }
        //else
        //    Debug.Log("AI can't attack other AI");
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
		base.die ();
		gameActorAnimator.SetBool ("isHit", true);
		transform.FindChild ("Base").gameObject.SetActive(false);
		transform.FindChild ("Reactions").gameObject.SetActive(false);
		transform.FindChild ("Feet_Collider").gameObject.SetActive(false);
		isMoving = false;
        StartCoroutine(DeathCleanUp());
    }

    public override void runAnimation()
    {
        base.runAnimation();
        BarrelBase bBase = GetComponentInChildren<BarrelBase>();
        bBase.facing = currDirection;
        Animator EnemyBarrelAnimator = bBase.GetComponentInChildren<Animator>();
        EnemyBarrelAnimator.SetInteger("Direction", (int)currDirection);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();

		BarrelBase bBaseObject = GetComponentInChildren<BarrelBase> ();
		if (bBaseObject) {
			SpriteRenderer bBase = bBaseObject.GetComponent<SpriteRenderer> ();
			bBase.sortingOrder = mySprite.sortingOrder + 3;
		}

		BarrelRotation barrelObject = GetComponentInChildren<BarrelRotation> ();
		if (barrelObject) {
			SpriteRenderer barrel = barrelObject.GetComponent<SpriteRenderer>();
			barrel.sortingOrder = mySprite.sortingOrder + 2;
		}

		Laser laserObject = GetComponentInChildren<Laser> ();
		if (laserObject) {
			SpriteRenderer laser =laserObject.GetComponent<SpriteRenderer>();
			laser.sortingOrder = mySprite.sortingOrder + 1;
		}

		Transform reactionsObject = transform.FindChild ("Reactions");
		if (reactionsObject) {
			SpriteRenderer reactionUI = reactionsObject.GetComponent<SpriteRenderer>();
			reactionUI.sortingOrder = mySprite.sortingOrder + 4;
		}
    }
    
    private IEnumerator DeathCleanUp()
    {
        //audioManager.Play("Death");
        yield return new WaitForSeconds(.1f);
		gameActorAnimator.SetBool ("isDead", true);
		yield return new WaitForSeconds(1);
        base.die();
        Destroy(gameObject);
    }
    
}
