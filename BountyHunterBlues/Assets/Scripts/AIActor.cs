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


    [System.Serializable]
    public class Patrol
    {
        public Transform point;
        public float wait_time;
    }

    public Patrol[] patrol_points;
    private int patrol_index;
    public bool is_patrol;
    public bool is_cycle;
    private bool patrol_forward;
    private bool patrol_backward;

    
    public GameObject playerObject;
    private State alertness;

    /*
     * AI could be attached to AIActor like so
     * AIManager AI;
     */

    public float state_change_time;
    public float attack_time_confirmation;
    private float inc_state_timer;
    private float dec_state_timer;
    private float attack_timer;
    private Quaternion rotation;

    private Command AI_move;
    private Command AI_aim;
    private Command AI_disableAim;
    private Command AI_attack;

    private Vector2 initial_position;
    private Vector2 initial_faceDir;
    private List<Vector2> positions = new List<Vector2>();
    private Vector2 transition_faceDir;
    public float move_speed;

	public Color[] stateColors = {
		Color.green,
		Color.yellow,
		Color.red
	};

    public override void Start()
    {
        base.Start();

        patrol_forward = true;
        patrol_backward = false;

        patrol_index = 0;
        inc_state_timer = 0;
        dec_state_timer = 0;

        initial_position = transform.position;
        initial_faceDir = faceDir;
        transition_faceDir = faceDir;

        move_speed = 2;

        AI_move = new MoveCommand(new Vector2(0, 0));
        AI_aim = new AimCommand(faceDir);
        AI_disableAim = new DisableAimCommand();
        AI_attack = new AttackCommand();

		run_state(State.GREEN);
    }

    public override void Update()
    {
        base.Update();

        //print(is_patrol);
        if(is_patrol){
            green_patrol();
        }
        else{
            green_alertness();
        }
        yellow_alertness();
        red_alertness();
        
    }
    

    public override void attack()
    {
        if (isAiming)
        {
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
		run_state(State.GREEN);
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
                    foreach (RaycastHit2D hitinfo in sortedHits)
                    {
                        GameObject hitObj = hitinfo.collider.gameObject;
                        if (hitObj.tag != "GameActor")
                            // obstruction in front, ignore the rest of the ray
                            break;
                        else if (hitObj.GetComponent<GameActor>() is PlayerActor)
                        {
                            // PlayerActor
                            tempLookTarget = hitObj.GetComponent<GameActor>();
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
        {
            lookTarget = tempLookTarget;
            Vector2 worldVector = lookTarget.gameObject.transform.position - transform.position;
            Debug.DrawRay(transform.position, worldVector * sightDistance, Color.magenta);
        }
    }


    private void run_state(State color){
        alertness = color;
        GetComponent<SpriteRenderer> ().color = stateColors [(int)color];
    }


    public void green_patrol(){
        if(alertness == State.GREEN && is_patrol){
            isMoving = false;
            if(lookTarget != null){
                inc_state_timer += Time.deltaTime;
                // get world-space vector to target from me
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();
                // transform world-space vector to my coordinate frame
                faceDir = transform.InverseTransformDirection(worldFaceDir);
                if(inc_state_timer > state_change_time){
                    inc_state_timer = 0;
                    run_state(State.YELLOW);
                }
            }
            else{
                Vector3 current_point = patrol_points[patrol_index].point.position;
                Vector2 worldFaceDir2 = current_point - transform.position;
                worldFaceDir2.Normalize();
                faceDir = transform.InverseTransformDirection(worldFaceDir2);
                if(Vector2.Distance(transform.position, current_point) > .1){
                    Vector2 worldFaceDir = current_point - transform.position;
                    worldFaceDir.Normalize();
                    Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
                    AI_move.updateCommandData(localDir);
                    AI_move.execute(this);
                }
                else{
                    if(is_cycle){
                        if(patrol_index == patrol_points.Count() - 1){
                            patrol_index = 0;
                        }
                        else{
                            patrol_index += 1;
                        }
                    }
                    else{
                        if(patrol_forward){
                            if(patrol_index < patrol_points.Count() - 1){
                                patrol_index += 1;
                            }
                            else{
                                patrol_forward = false;
                                patrol_backward = true;
                            }
                        }
                        else if(patrol_backward){
                            if(patrol_index > 0){
                                patrol_index -= 1;
                            }
                            else{
                                patrol_forward = true;
                                patrol_backward = false;
                            }
                        }
                    }
                }
                positions.Clear();
                inc_state_timer = 0;
            }
        }
    }

    public void green_alertness(){
        if(alertness == State.GREEN){
            isMoving = false;
            if(lookTarget != null){
                inc_state_timer += Time.deltaTime;
                // get world-space vector to target from me
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();
                // transform world-space vector to my coordinate frame
                //if(transition_faceDir != worldFaceDir){
                //    if(worldFaceDir.x > 0)
                //        transition_faceDir.x += 1;
                //    else
                //        transition_faceDir.x -= 1;
                //    if(worldFaceDir.y > 0)
                //        transition_faceDir.y += 1;
                //    else
                //        transition_faceDir.y -= 1;
                //}
                //transition_faceDir.Normalize();
                //faceDir = transform.InverseTransformDirection(transition_faceDir);
                if(inc_state_timer > state_change_time){
                    inc_state_timer = 0;
                    run_state(State.YELLOW);
                }
            }
            else{
                positions.Clear();
                faceDir = initial_faceDir;
                inc_state_timer = 0;
            }
        }
    }

    public void yellow_alertness(){
        if(alertness == State.YELLOW){
            if(lookTarget == null){
                isMoving = false;
                inc_state_timer = 0;
                dec_state_timer += Time.deltaTime;
                
                if(dec_state_timer > state_change_time){
                    if(positions.Count > 0){
                        Vector2 new_position = positions[positions.Count - 1];
                        positions.RemoveAt(positions.Count - 1);
                        transform.position = new_position;
                    }
                    else{
                        dec_state_timer = 0;
                        run_state(State.GREEN);
                    }
                    faceDir = initial_faceDir;
                }
            }
            if(lookTarget != null){
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();
                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
                positions.Add(transform.position);
                
                AI_move.updateCommandData(localDir);
                AI_move.execute(this);
                dec_state_timer = 0;
                inc_state_timer += Time.deltaTime;
                if(inc_state_timer > state_change_time){
                    inc_state_timer = 0;
                    run_state(State.RED);
                }
            }
        }
    }

    public void red_alertness(){
        if(alertness == State.RED){
            if(lookTarget != null){
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();
                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
                AI_move.updateCommandData(localDir);
                AI_move.execute(this);
                AI_aim.updateCommandData(faceDir);
                AI_aim.execute(this);
                attack_timer += Time.deltaTime;
                if(attack_timer > attack_time_confirmation){
                    attack_timer = 0;
                    AI_attack.execute(this);
                }
            }
            if(lookTarget == null){
                isMoving = false;
                attack_timer = 0;
                dec_state_timer += Time.deltaTime;
                if(dec_state_timer > state_change_time){
                    dec_state_timer = 0;
                    AI_disableAim.execute(this);
                    run_state(State.YELLOW);
                }
            }
        }
    }
}
