using UnityEngine;
using System.Collections;
using System;

public class MissileEnemy : EnemyActor {

    public float timeBetweenEachMissile;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        current_state = new NeutralMissile(this);

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        is_confused();
        _stateManager.update_state(closestAttackable, sound_heard(), is_alert());
        if (current_state.get_state() != _stateManager.get_state())
        {
            current_state.on_exit();
            if (_stateManager.get_state() == State.NEUTRAL)
                current_state = new NeutralMissile(this);
            if (_stateManager.get_state() == State.ALERT)
                current_state = new AlertMissile(this);
            if (_stateManager.get_state() == State.AGGRESIVE)
                current_state = new AggresiveMissile(this);

            current_state.on_enter();
        }
        current_state.execute();

        Debug.Log("Missile Enemy in state " + current_state.name());
    }

    public override void die()
    {
        base.die();
        Destroy(gameObject);
    }

    public override void interact()
    {
        throw new NotImplementedException();
    }

    public override void meleeAttack()
    {
        throw new NotImplementedException();
    }

    public override void rangedAttack()
    {
        // shoot projectile
        Debug.Log("Missile Enemy has fired!!!!");
    }

    
}
