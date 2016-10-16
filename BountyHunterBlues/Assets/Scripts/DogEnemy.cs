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
		base.Update();

        _stateManager.update_state(closestAttackable, sound_heard(), is_alert());
        if(current_state.get_state() != _stateManager.get_state()){
            current_state.on_exit();
            if(_stateManager.get_state() == State.NEUTRAL)
                current_state = new NeutralDog(this);
            if(_stateManager.get_state() == State.ALERT)
                current_state = new AlertDog(this);
            if(_stateManager.get_state() == State.AGGRESIVE)
                current_state = new AggresiveDog(this);

            current_state.on_enter();
        }
		current_state.execute();

        if(isMoving)
        {
            if (!audioManager.isPlaying("Feet"))
            {
                if (isPatrolling)
                    audioManager.Play("Feet", "Patrol");
                else
                    audioManager.Play("Feet", "Chase");
            }

        }
        else
        {
            audioManager.Stop("Feet");
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
        else
            Debug.Log("AI can't attack other AI");
    }

    public override void meleeAttack(){
        throw new NotImplementedException();
    }

    public override void interact(){
        throw new NotImplementedException();
    }

    public override void die()
    {
        //StartCoroutine(DeathCleanUp());
        Destroy(gameObject);
        
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
        SpriteRenderer bBase = GetComponentInChildren<BarrelBase>().GetComponent<SpriteRenderer>();
        SpriteRenderer barrel = GetComponentInChildren<BarrelRotation>().GetComponent<SpriteRenderer>();
        SpriteRenderer laser = GetComponentInChildren<Laser>().GetComponent<SpriteRenderer>();
        SpriteRenderer reactionUI = transform.FindChild("Reactions").GetComponent<SpriteRenderer>();

        laser.sortingOrder = mySprite.sortingOrder + 1;
        barrel.sortingOrder = laser.sortingOrder + 1;
        bBase.sortingOrder = barrel.sortingOrder + 1;
        reactionUI.sortingOrder = bBase.sortingOrder + 1;


        
    }
    /*
    private IEnumerator DeathCleanUp()
    {
        audioManager.Play("Death");
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
    */
}
