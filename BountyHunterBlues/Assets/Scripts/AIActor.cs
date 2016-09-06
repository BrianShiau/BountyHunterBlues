using UnityEngine;
using System.Collections;
using System;

public enum State
{
    GREEN, YELLOW, RED
}

public class AIActor : GameActor {

    public GameObject playerObject;

    
    private State alertness;

    /*
     * AI could be attached to AIActor like so
     * AIManager AI;
     */

    private Command move;

    private Command aim;
    private Command disableAim;

    private Command interacting;
    private Command attacking;

    public Vector3 aim_direction;

    public override void Start()
    {
        base.Start();
        healthPool = 1;

        aim_direction = new Vector2(0, 1);
        move = new MoveCommand(new Vector2(0, 0));
        aim = new AimCommand(aim_direction);
        disableAim = new DisableAimCommand();
        interacting = new InteractCommand();
        attacking = new AttackCommand();

        alertness = State.GREEN;
    }

    public override void attack()
    {
        if (isAiming)
        {
            Debug.Log("Enemy Shoots");
            if(aimTarget!=null && Vector3.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
            {
                if(aimTarget is PlayerActor)
                {
                    aimTarget.takeDamage();
                    if (!aimTarget.isAlive())
                        aimTarget = null;
                }

                // else for friendly fire with AI
            }
        }
        // no else since AI can't melee
    }

    public override void interact()
    {
        throw new NotImplementedException();
    }

    public void resetState()
    {
        alertness = State.GREEN;
    }

    public void updateState(State newAlertState)
    {

    }

	public override void die()
	{
		Destroy (gameObject);
	}

    IEnumerator ExecuteGreen(float time){
        yield return new WaitForSeconds(time);
        alertness = State.GREEN;
    }

    IEnumerator ExecuteYellow(float time){
        yield return new WaitForSeconds(time);
        alertness = State.YELLOW;
    }

    IEnumerator ExecuteRed(float time){
        yield return new WaitForSeconds(time);
        alertness = State.RED;
    }

    public void green_alertness(){
        if(alertness == State.GREEN){
            if(lookTarget != null){
                ExecuteGreen(3);
            }
        }
    }

    public void yellow_alertness(){
        if(alertness == State.YELLOW){
            
        }
    }

    public void red_alertness(){
        if(alertness == State.RED){
            
        }
    }

    void Update(){
        green_alertness();
        yellow_alertness();
        red_alertness();
    }
}
