using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public float state_change_time;
    public float attack_time_confirmation;
    private float state_timer;
    private float attack_timer;
    private Quaternion rotation;

    private Command AI_move;
    private Command AI_aim;
    private Command AI_disableAim;
    private Command AI_attack;

    public Vector2 aim_direction;
    private Vector2 initial_position;
    public float move_speed;

    public override void Start()
    {
        base.Start();
        healthPool = 1;
        state_change_time = 3;
        state_timer = 0;

        aim_direction = new Vector2(0, 1);
        initial_position = new Vector2(transform.position.x, transform.position.y);
        move_speed = 2;

        AI_move = new MoveCommand(new Vector2(0, 0));
        AI_aim = new AimCommand(aim_direction);
        AI_disableAim = new DisableAimCommand();
        AI_attack = new AttackCommand();

        alertness = State.GREEN;
    }

    public override void Update()
    {
        base.Update();
        /*
        green_alertness();
        yellow_alertness();
        red_alertness();
        */
    }
    

    public override void attack()
    {
        if (isAiming)
        {
            Debug.Log("Enemy Shoots");
            if(aimTarget!=null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
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

    protected override void runVisionDetection()
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        GameActor tempLookTarget = null;
        foreach (GameObject actorObject in ActorObjects)
        {
            if (actorObject != this.gameObject) // ignore myself
            {
                Vector2 worldVector = actorObject.transform.position - transform.position;
                worldVector.Normalize();
                Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
                if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, sightDistance);
                    IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance); // sorted by ascending by default
                    foreach (RaycastHit2D hitinfo in hits)
                    {
                        GameObject hitObj = hitinfo.collider.gameObject;
                        if (hitObj.tag != "GameActor")
                            // obstruction in front, ignore the rest of the ray
                            break;
                        else if (hitObj.GetComponent<GameActor>() is PlayerActor)
                        {
                            // PlayerActor
                            tempLookTarget = hitObj.GetComponent<GameActor>();
                            Debug.DrawRay(transform.position, worldVector * sightDistance, Color.magenta);
                            break;
                        }
                        // else the next obj in the ray line is an AIActor, just ignore it and keep moving down the ray
                    }

                }
            }

            if (tempLookTarget != null)
                break; // found player, no need to keep looping
        }
        if (tempLookTarget == null)
            lookTarget = null;
        else
            lookTarget = tempLookTarget;
    }


    private void run_state(State color){
        alertness = color;
    }


    public void green_alertness(){
        if(alertness == State.GREEN){
            Debug.Log(initial_position);
            if(lookTarget != null){
                state_timer += Time.deltaTime;
                faceDir = new Vector2(lookTarget.gameObject.transform.position.x, lookTarget.gameObject.transform.position.y);
                aimDir = new Vector2(lookTarget.gameObject.transform.position.x, lookTarget.gameObject.transform.position.y);;
                if(state_timer > state_change_time){
                    state_timer = 0;
                    run_state(State.YELLOW);
                }
            }
            else{
                state_timer = 0;
            }
        }
    }

    public void yellow_alertness(){
        if(alertness == State.YELLOW){
            if(lookTarget == null){
                state_timer += Time.deltaTime;
                if(state_timer > state_change_time){
                    state_timer = 0;
                    //teleport to original position
                    run_state(State.GREEN);
                }
            }
            if(lookTarget != null){
                AI_move.updateCommandData(lookTarget.transform.position - transform.position);
                AI_move.execute(this);
                state_timer += Time.deltaTime;
                if(state_timer > state_change_time){
                    state_timer = 0;
                    run_state(State.RED);
                }
            }
        }
    }

    public void red_alertness(){
        if(alertness == State.RED){
            AI_aim.execute(this);
            attack_timer += Time.deltaTime;
            if(attack_timer > attack_time_confirmation && lookTarget != null){
                AI_attack.execute(this);
            }
            if(lookTarget == null){
                state_timer += Time.deltaTime;
                if(state_timer > state_change_time){
                    state_timer = 0;
                    AI_disableAim.execute(this);
                    run_state(State.YELLOW);
                }
            }
        }
    }


}
