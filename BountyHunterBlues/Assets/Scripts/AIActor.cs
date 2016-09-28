using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum State
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
    public State alertness;

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

    public Color[] stateColors = {
        Color.green,
        Color.yellow,
        Color.red
    };

    public override void Start()
    {
        base.Start();

        path = gameObject.GetComponent<PathFinding>();
        player = GameObject.Find("Player Character").GetComponent<PlayerActor>();
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
        AI_aim = new AimCommand(faceDir);
        AI_disableAim = new DisableAimCommand();
        AI_attack = new AttackCommand();

        run_state(State.GREEN);
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
        print(alertness);
    }
    

    public override void attack()
    {
        if (isAiming)
        {
            {
                if (aimTarget is PlayerActor)
                {
                    hasAttacked = true;
                    aimTarget.takeDamage();
                    if (!aimTarget.isAlive())
                        aimTarget = null;
                }
                else
                    Debug.Log("AI can't attack other AI");
            }
        }
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
        run_state(State.GREEN);
    }

    public void updateState(State newAlertState)
    {

    }

    public override void die()
    {
        Destroy (gameObject);
    }

    public override void runVisionDetection()
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
                        else if (hitObj.GetComponent<GameActor>() is PlayerActor && hitObj.GetComponent<GameActor>().isVisible)
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
    }


    public virtual void green_patrol(){
        if(alertness == State.GREEN && is_patrol){
            isMoving = false;
            if(lookTarget != null){
                // get world-space vector to target from me
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();

                Vector2 localWorldFaceDir = transform.InverseTransformDirection(worldFaceDir);
                Vector2 temp = Vector2.MoveTowards(faceDir, localWorldFaceDir, rotation_speed * Time.deltaTime);
                temp.Normalize();
                faceDir = temp;

                if(faceDir == localWorldFaceDir){
                    inc_state_timer += Time.deltaTime;
                    if(inc_state_timer > state_change_time){
                        inc_state_timer = 0;
                        run_state(State.YELLOW);
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
        if(alertness == State.GREEN){
            isMoving = false;
            if(sound_detection(player.bullet_shot()) && lookTarget == null){
                run_state(State.YELLOW_AUDIO);
            }
            else if(lookTarget != null){
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();

                Vector2 localWorldFaceDir = transform.InverseTransformDirection(worldFaceDir);
                Vector2 temp = Vector2.MoveTowards(faceDir, localWorldFaceDir, rotation_speed * Time.deltaTime);
                temp.Normalize();
                faceDir = temp;

                if(faceDir == localWorldFaceDir){
                    inc_state_timer += Time.deltaTime;
                    if(inc_state_timer > state_change_time){
                        inc_state_timer = 0;
                        run_state(State.YELLOW);
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

    public bool sound_detection(Vector3 audio_point){
        if(audio_point.x != 0 && audio_point.y != 0){
            sound_location = audio_point;
            return Vector2.Distance(transform.position, audio_point) < audio_distance;
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

    public void chase_alertness(){
        if(alertness == State.CHASE){
            if(sound_detection(player.bullet_shot()) && lookTarget == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.YELLOW_AUDIO);
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
                
                if(distance_from_node < .1){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.RETURN);
                return;
            }
            if(lookTarget != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.YELLOW);
                return;
            }
        }
    }

    public void return_to_default(){
        if(alertness == State.RETURN){
            if(sound_detection(player.bullet_shot()) && lookTarget == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.YELLOW_AUDIO);
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
                
                if(distance_from_node < .1){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.GREEN);
                return;
            }
            if(lookTarget != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.YELLOW);
                return;
            }
        }
    }

    public virtual void yellow_audio(){
        if(alertness == State.YELLOW_AUDIO){
            if(sound_detection(player.bullet_shot()) && lookTarget == null){                
                shortest_path_index = 0;
                shortest_path_calculated = false;
            }
            calc_shortest_path(transform.position, sound_location);
            
            if(shortest_path_index < path.length()){
                Node current_node = path.get_node(shortest_path_index);
                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);

                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
                worldFaceDir.Normalize();
                faceDir = transform.InverseTransformDirection(worldFaceDir);
                AI_move.updateCommandData(faceDir);
                AI_move.execute(this);
                
                if(distance_from_node < .1){
                     shortest_path_index += 1;   
                }
            }
            else{
                isMoving = false;
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.RETURN);
                return;
            }
            if(lookTarget != null){
                shortest_path_index = 0;
                shortest_path_calculated = false;
                run_state(State.YELLOW);
                return;
            }
        }
    }

    public virtual void yellow_alertness(){
        if(alertness == State.YELLOW){
            if(lookTarget != null){
                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
                worldFaceDir.Normalize();
                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
                
                AI_move.updateCommandData(localDir);
                AI_move.execute(this);
                dec_state_timer = 0;
                inc_state_timer += Time.deltaTime;
                last_seen = lookTarget.transform.position; 
                if(inc_state_timer > state_change_time){
                    inc_state_timer = 0;
                    run_state(State.RED);
                }
            }
            if(lookTarget == null){
                isMoving = false;
                inc_state_timer = 0;
                dec_state_timer += Time.deltaTime;
                
                if(dec_state_timer > state_change_time){
                    shortest_path_index = 0;
                    shortest_path_calculated = false;
                    run_state(State.CHASE);
                }
            }
        }
    }

	public virtual void red_alertness(){
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

    public override void updateAnimation()
    {
        base.updateAnimation();
        BarrelBase bBase = GetComponentInChildren<BarrelBase>();


        Animator EnemyBarrelAnimator = bBase.GetComponentInChildren<Animator>();
        if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
        {
            if (faceDir.y > 0)
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.UP);
                bBase.facing = Direction.UP;
            }
            else
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.DOWN);
                bBase.facing = Direction.DOWN;
            }
        }

        else
        {
            if (faceDir.x > 0)
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.RIGHT);
                bBase.facing = Direction.RIGHT;
            }
            else
            {
                GetComponentInChildren<BarrelRotation>().setOrientation(GameActor.Direction.LEFT);
                bBase.facing = Direction.LEFT;
            }
        }
    }

}
