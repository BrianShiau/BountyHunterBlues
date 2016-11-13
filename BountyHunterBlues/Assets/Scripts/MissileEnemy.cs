using UnityEngine;
using System.Collections;
using System;

public class MissileEnemy : EnemyActor {

    public GameObject MissileObject;
    public float timeBetweenEachMissile;
    public int numMissilesToFire;

    private float shoot_timer = 1;
    private float shoot_timer_threshold = 1;
    public bool readyToFire { get; private set; }
    private bool readyToTime = true;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        current_state = new NeutralMissile(this);

    }

    // Update is called once per frame
    public override void Update()
    {
		if (health <= 0)
			return;

        base.Update();

        is_confused();
        _stateManager.update_state(player_is_cloaked(), closestAttackable, sound_heard(), is_alert());
        if (current_state.get_state() != _stateManager.get_state())
        {
            current_state.on_exit();
            if (_stateManager.get_state() == State.NEUTRAL)
                current_state = new NeutralMissile(this);
            if (_stateManager.get_state() == State.ALERT)
                current_state = new AlertMissile(this);
            if (_stateManager.get_state() == State.AGGRESIVE)
            {
                current_state = new AggresiveMissile(this);
                audioManager.Play("Alert");
            }

            current_state.on_enter();
        }
        current_state.execute();

        if (readyToTime && shoot_timer < shoot_timer_threshold)
            shoot_timer += Time.deltaTime;

        if (shoot_timer >= shoot_timer_threshold)
            readyToFire = true; 
    }

    public override void die()
    {
        base.die();
		//isMoving = false;
		transform.FindChild ("Laser").gameObject.SetActive(false);
		StartCoroutine(DeathCleanUp());
    }

    public override void interact()
    {
        throw new NotImplementedException();
    }

	public override void EndInteract(){
		throw new NotImplementedException();
	}

    public override void meleeAttack()
    {
        throw new NotImplementedException();
    }

    public override void rangedAttack()
    {
        // shoot projectile
        StartCoroutine(FireMissiles());
    }

    private IEnumerator FireMissiles()
    {
        readyToFire = false;
        readyToTime = false;
        shoot_timer = 0;
        for (int i = 0; i < numMissilesToFire; ++i)
        {
			if (health > 0) {
                Vector3 source = transform.position;
                if (raySource != null)
                    source = raySource.position;
				MissileProjectile missile = MissileProjectile.Create (MissileObject, source, GetComponentInChildren<Laser> ().transform.eulerAngles);
				missile.setInitialDir (transform.TransformDirection (faceDir));
				missile.setOwner (this);
			}
            yield return new WaitForSeconds(timeBetweenEachMissile);
        }
        readyToTime = true;

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
