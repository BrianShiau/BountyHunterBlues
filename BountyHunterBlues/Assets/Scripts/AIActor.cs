﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum State2
{
    GREEN, YELLOW, RED, YELLOW_AUDIO, RETURN, CHASE
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
    public bool hasAttacked;
    private bool patrol_forward;
    private bool patrol_backward;
    private float wait_time_counter;
    
    public float rotation_speed;
    public GameObject playerObject;
    public State2 alertness;

    public int audio_distance = 10;
    public int shortest_path_index = 0;
    public Vector3 sound_location;

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

    private Vector3 default_position;
    private Vector2 initial_position;
    private Vector2 initial_faceDir;
    private Vector3 last_seen;
    private List<Vector3> positions = new List<Vector3>();
    private Vector2 transition_faceDir;
    public float move_speed;

    private bool shortest_path_calculated = false;
    private bool sound_detected = false;

    private GameObject barrel;
    PathFinding path; 
    private PlayerActor player;

    // Audio
    public AudioClip patrolStompSound;
    public AudioClip chaseStompSound;
    public AudioClip fireShotSound;

    private AudioSource moveAudioSource;
    private AudioSource shotAudioSource;

	// UI Reactions
	Animator reactionAnim;

    public Color[] stateColors = {
        Color.green,
        Color.yellow,
        Color.red
    };

    public override void Start()
    {
        base.Start();

        path = gameObject.GetComponent<PathFinding>();
        player = GameObject.Find("Player").GetComponent<PlayerActor>();
        patrol_forward = true;
        patrol_backward = false;

        patrol_index = 0;
        inc_state_timer = 0;
        dec_state_timer = 0;
        wait_time_counter = 0;
        rotation_speed = 4f;

        last_seen = transform.position;
        default_position = transform.position;
        initial_faceDir = faceDir;
        transition_faceDir = faceDir;

        move_speed = 2;

        AI_move = new MoveCommand(new Vector2(0, 0));
        //AI_aim = new AimCommand(faceDir);
        //AI_disableAim = new DisableAimCommand();
        //AI_attack = new AttackCommand();

        run_state(State2.GREEN);

		if (transform.FindChild ("Reactions")) {
			reactionAnim = transform.FindChild ("Reactions").GetComponent<Animator> ();
			reactionAnim.speed = 3.0f;
		}
    }

    public override void Update()
    {
        base.Update();
        hasAttacked = false;

        if(is_patrol){
            green_patrol();
        }
        else{
            green_alertness();
        }

        yellow_audio();
        return_to_default();
        yellow_alertness();
        red_alertness();
        chase_alertness();
        //player.gun_fired = false;
    }
    

    //public override void attack()
    //{
    //    if (isAiming)
    //    {
    //        {
    //            if (aimTarget is PlayerActor)
    //            {
    //                hasAttacked = true;
    //                aimTarget.takeDamage();
    //                if (!aimTarget.isAlive())
    //                    aimTarget = null;
    //            }
    //            else
    //                Debug.Log("AI can't attack other AI");
    //        }
    //    }
    //}


    public override void rangedAttack(){
        if (closestAttackable is PlayerActor){
            hasAttacked = true;
            closestAttackable.takeDamage();
            if (!closestAttackable.isAlive())
                closestAttackable = null;
        }
        else
            Debug.Log("AI can't attack other AI");
        
    }

    public override void meleeAttack()
    {
        throw new NotImplementedException();
    }

    public override void interact()
    {
        throw new NotImplementedException();
    }

    public void resetState()
    {
        run_state(State2.GREEN);
    }

    public void updateState(State2 newAlertState)
    {

    }

    public override void die()
    {
        Destroy (gameObject);
    }

    public override GameActor[] runVisionDetection(float fov, float sightDistance)
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        GameActor tempclosestAttackable = null;

        PlayerActor actorObject = GameObject.FindObjectOfType<PlayerActor>();


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
                else if (hitObj.GetComponent<GameActor>() is PlayerActor && hitObj.GetComponent<GameActor>().isVisible())
                {
                    // PlayerActor
                    tempclosestAttackable = hitObj.GetComponent<GameActor>();
                    break;
                }
                // else the next obj in the ray line is an AIActor, just ignore it and keep moving down the ray
            }
        }
        
        if (tempclosestAttackable == null)
            closestAttackable = null;
        else
        {
            closestAttackable = tempclosestAttackable;
            worldVector = closestAttackable.gameObject.transform.position - transform.position;
            Debug.DrawRay(transform.position, worldVector * sightDistance, Color.magenta);
        }

        return new GameActor[3];
    }


    private void run_state(State2 color){
        alertness = color;
    }

	private void resetReactionAnim(){
		reactionAnim.SetInteger ("State", 0);
	}

    public virtual void green_patrol(){
        if(alertness == State2.GREEN && is_patrol){
            isMoving = false;
            if(sound_detection(sound_location) && closestAttackable == null){
				if (reactionAnim) {
					reactionAnim.SetInteger ("State", 1);
					Invoke ("resetReactionAnim", 2);
				}
                run_state(State2.YELLOW_AUDIO);
            }
            if(closestAttackable != null){
                // get world-space vector to target from me
                Vector2 worldFaceDir = closestAttackable.transform.position - transform.position;
                worldFaceDir.Normalize();

                Vector2 localWorldFaceDir = transform.InverseTransformDirection(worldFaceDir);
                Vector2 temp = Vector2.MoveTowards(faceDir, localWorldFaceDir, rotation_speed * Time.deltaTime);
                temp.Normalize();
                faceDir = temp;

                if(faceDir == localWorldFaceDir){
                    inc_state_timer += Time.deltaTime;
                    if(inc_state_timer > state_change_time){
                        inc_state_timer = 0;
                        run_state(State2.YELLOW);
                    }
                }
            }
            else{
                
                Vector3 current_point = patrol_points[patrol_index].point.position;
                float wait_time = patrol_points[patrol_index].wait_time;
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
                    if(wait_time <= wait_time_counter){
                        wait_time_counter = 0;
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
                    else{
                        wait_time_counter += Time.deltaTime;
                    }
                }
                positions.Clear();
                inc_state_timer = 0;
            }
        }
    }

    public virtual void green_alertness(){
        if(alertness == State2.GREEN){
            isMoving = false;
            if(sound_detection(sound_location) && closestAttackable == null){
				if (reactionAnim) {
					reactionAnim.SetInteger ("State", 1);
					Invoke ("resetReactionAnim", 2);
				}
                run_state(State2.YELLOW_AUDIO);
            }
            else if(closestAttackable != null){
                Vector2 worldFaceDir = closestAttackable.transform.position - transform.position;
                worldFaceDir.Normalize();

                Vector2 localWorldFaceDir = transform.InverseTransformDirection(worldFaceDir);
                Vector2 temp = Vector2.MoveTowards(faceDir, localWorldFaceDir, rotation_speed * Time.deltaTime);
                temp.Normalize();
                faceDir = temp;

                if(faceDir == localWorldFaceDir){
                    inc_state_timer += Time.deltaTime;
                    if(inc_state_timer > state_change_time){
                        inc_state_timer = 0;
                        run_state(State2.YELLOW);
                    }
                }
            }
            else{
                positions.Clear();
                Vector2 temp = Vector2.MoveTowards(faceDir, initial_faceDir, rotation_speed * Time.deltaTime);
                temp.Normalize();
                faceDir = temp;
                inc_state_timer = 0;
            }
        }
    }

    public virtual bool sound_detection(Vector3 audio_point){
        if(audio_point.x != 0 && audio_point.y != 0){
            if(Vector2.Distance(transform.position, audio_point) < audio_distance){
                return true;
            }
        }
        return false;
    }

    public void calc_shortest_path(Vector3 from, Vector3 to){
        if(!shortest_path_calculated){
            path.initialize(from, to);
            path.calc_path();
            shortest_path_calculated = true;
        }
    }

    public virtual void chase_alertness(){
        if(alertness == State2.CHASE){
            if(sound_detection(sound_location) && closestAttackable == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.YELLOW_AUDIO);
                return;
            }
            calc_shortest_path(transform.position, last_seen);

            if(shortest_path_index < path.length()){
                Node current_node = path.get_node(shortest_path_index);
                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
                
                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
                worldFaceDir.Normalize();
                faceDir = transform.InverseTransformDirection(worldFaceDir);
                AI_move.updateCommandData(faceDir);
                AI_move.execute(this);
                
                if(distance_from_node < .8){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.RETURN);
                return;
            }
            if(closestAttackable != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.YELLOW);
                return;
            }
        }
    }

    public virtual void return_to_default(){
        if(alertness == State2.RETURN){
            if(sound_detection(sound_location) && closestAttackable == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.YELLOW_AUDIO);
                return;
            }
            calc_shortest_path(transform.position, default_position);

            if(shortest_path_index < path.length()){
                Node current_node = path.get_node(shortest_path_index);
                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
                
                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
                worldFaceDir.Normalize();
                faceDir = transform.InverseTransformDirection(worldFaceDir);
                AI_move.updateCommandData(faceDir);
                AI_move.execute(this);
                
                if(distance_from_node < .8){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.GREEN);
                return;
            }
            if(closestAttackable != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.YELLOW);
                return;
            }
        }
    }

    public virtual void yellow_audio(){
        if(alertness == State2.YELLOW_AUDIO){
            if(sound_detection(sound_location) && closestAttackable == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
            }
            calc_shortest_path(transform.position, sound_location);
            sound_location = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            
            if(shortest_path_index < path.length()){
                Node current_node = path.get_node(shortest_path_index);
                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);

                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
                worldFaceDir.Normalize();
                faceDir = transform.InverseTransformDirection(worldFaceDir);
                AI_move.updateCommandData(faceDir);
                AI_move.execute(this);
                
                if(distance_from_node < .8){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.RETURN);
                return;
            }
            if(closestAttackable != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State2.YELLOW);
                return;
            }
        }
    }

    public virtual void yellow_alertness(){
        if(alertness == State2.YELLOW){
            if(closestAttackable != null){
                Vector2 worldFaceDir = closestAttackable.transform.position - transform.position;
                worldFaceDir.Normalize();
                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
                
                AI_move.updateCommandData(localDir);
                AI_move.execute(this);
                dec_state_timer = 0;
                inc_state_timer += Time.deltaTime;
                last_seen = closestAttackable.transform.position; 
                if(inc_state_timer > state_change_time){
                    inc_state_timer = 0;
                    run_state(State2.RED);
                }
            }
            if(closestAttackable == null){
                isMoving = false;
                inc_state_timer = 0;
                dec_state_timer += Time.deltaTime;
                
                if(dec_state_timer > state_change_time){
                    shortest_path_index = 0;
                    shortest_path_calculated = false;
                    run_state(State2.CHASE);
                }
            }
        }
    }

	public virtual void red_alertness(){
        if(alertness == State2.RED){
            if(closestAttackable != null){
                Vector2 worldFaceDir = closestAttackable.transform.position - transform.position;
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
            if(closestAttackable == null){
                isMoving = false;
                attack_timer = 0;
                dec_state_timer += Time.deltaTime;
                if(dec_state_timer > state_change_time){
                    dec_state_timer = 0;
                    AI_disableAim.execute(this);
                    run_state(State2.YELLOW);
                }
            }
        }
    }

    //public override void updateAnimation()
    //{
    //    base.updateAnimation();
    //    BarrelBase bBase = GetComponentInChildren<BarrelBase>();
//
    //    Animator EnemyBarrelAnimator = bBase.GetComponentInChildren<Animator>();
    //    if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
    //    {
    //        if (faceDir.y > 0)
    //        {
    //            GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.UP);
    //            bBase.facing = Direction.UP;
    //        }
    //        else
    //        {
    //            GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.DOWN);
    //            bBase.facing = Direction.DOWN;
    //        }
    //    }
//
    //    else
    //    {
    //        if (faceDir.x > 0)
    //        {
    //            GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.RIGHT);
    //            bBase.facing = Direction.RIGHT;
    //        }
    //        else
    //        {
    //            GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.LEFT);
    //            bBase.facing = Direction.LEFT;
    //        }
    //    }
    //}

    //public override void initAudio()
    //{
    //    moveAudioSource = gameObject.AddComponent<AudioSource>();
    //    moveAudioSource.clip = patrolStompSound;
    //    moveAudioSource.loop = true;
    //    moveAudioSource.playOnAwake = false;
    //    moveAudioSource.volume = 1.0f;
    //    
    //    
//
    //    shotAudioSource = gameObject.AddComponent<AudioSource>();
    //    shotAudioSource.clip = fireShotSound;
    //    shotAudioSource.loop = false;
    //    shotAudioSource.playOnAwake = false;
    //    shotAudioSource.volume = 1.0f;
    //}

    //public override void runAudio()
    //{
    //    if (isMoving)
    //    {
    //        float volumeRolloff = .05f * (Vector2.Distance(transform.position, player.transform.position));
    //        if (is_patrol)
    //        {
    //            moveAudioSource.volume = .7f - volumeRolloff;
    //            moveAudioSource.clip = patrolStompSound;
    //        }
    //        else
    //        {
    //            moveAudioSource.volume = 1.0f - volumeRolloff;
    //            moveAudioSource.clip = chaseStompSound;
    //        }
    //        if(!moveAudioSource.isPlaying)
    //            moveAudioSource.Play();
    //    }
    //    else
    //        moveAudioSource.Stop();
//
    //    if (hasAttacked)
    //        shotAudioSource.Play();
    //}
}
