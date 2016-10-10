

//public class tmp_AIActor : {
//    public float rotation_speed;
//    public GameObject playerObject;
//    public State alertness;
//
//    public int audio_distance = 10;
//    public Vector3 sound_location;
//
//    /*
//     * AI could be attached to AIActor like so
//     * AIManager AI;
//     */
//
//    public float state_change_time;
//    public float attack_time_confirmation;
//    private float inc_state_timer;
//    private float dec_state_timer;
//    private Quaternion rotation;
//
//    private Vector3 default_position;
//    private Vector2 initial_position;
//    private Vector2 initial_faceDir;
//    private Vector3 last_seen;
//    private Vector2 transition_faceDir;
//
//    private bool sound_detected = false;
//
//    private PlayerActor player;
//
//
//	// UI Reactions
//	Animator reactionAnim;
//
//    public Color[] stateColors = {
//        Color.green,
//        Color.yellow,
//        Color.red
//    };
//
//    public override void Start(){
//        base.Start();
//
//        path = gameObject.GetComponent<PathFinding>();
//        player = GameObject.Find("Player").GetComponent<PlayerActor>();
//        patrol_forward = true;
//        patrol_backward = false;
//
//        patrol_index = 0;
//        inc_state_timer = 0;
//        dec_state_timer = 0;
//        wait_time_counter = 0;
//        rotation_speed = 4f;
//
//        last_seen = transform.position;
//        default_position = transform.position;
//        initial_faceDir = faceDir;
//        transition_faceDir = faceDir;
//
//        run_state(State.GREEN);
//    }
//
//    public override void Update(){
//        
//    }
//    
//
//
//    public void resetState(){
//        run_state(State.GREEN);
//    }

//
//    private void run_state(State color){
//        alertness = color;
//    }

//    public virtual bool sound_detection(Vector3 audio_point){
//        if(audio_point.x != 0 && audio_point.y != 0){
//            if(Vector2.Distance(transform.position, audio_point) < audio_distance){
//                return true;
//            }
//        }
//        return false;
//    }
//
//    public virtual void green_alertness(){
//        if(alertness == State.GREEN){
//            isMoving = false;
//            if(sound_detection(sound_location) && lookTarget == null){
//				if (reactionAnim) {
//					reactionAnim.SetInteger ("State", 1);
//					Invoke ("resetReactionAnim", 2);
//				}
//                run_state(State.YELLOW_AUDIO);
//            }
//            else if(lookTarget != null){
//                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
//                worldFaceDir.Normalize();
//
//                Vector2 localWorldFaceDir = transform.InverseTransformDirection(worldFaceDir);
//                Vector2 temp = Vector2.MoveTowards(faceDir, localWorldFaceDir, rotation_speed * Time.deltaTime);
//                temp.Normalize();
//                faceDir = temp;
//
//                if(faceDir == localWorldFaceDir){
//                    inc_state_timer += Time.deltaTime;
//                    if(inc_state_timer > state_change_time){
//                        inc_state_timer = 0;
//                        run_state(State.YELLOW);
//                    }
//                }
//            }
//            else{
//                positions.Clear();
//                Vector2 temp = Vector2.MoveTowards(faceDir, initial_faceDir, rotation_speed * Time.deltaTime);
//                temp.Normalize();
//                faceDir = temp;
//                inc_state_timer = 0;
//            }
//        }
//    }
//
//
//    public void calc_shortest_path(Vector3 from, Vector3 to){
//        if(!shortest_path_calculated){
//            path.initialize(from, to);
//            path.calc_path();
//            shortest_path_calculated = true;
//        }
//    }
//
//    public virtual void chase_alertness(){
//        if(alertness == State.CHASE){
//            if(sound_detection(sound_location) && lookTarget == null){                
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.YELLOW_AUDIO);
//                return;
//            }
//            calc_shortest_path(transform.position, last_seen);
//
//            if(shortest_path_index < path.length()){
//                Node current_node = path.get_node(shortest_path_index);
//                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
//                
//                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
//                worldFaceDir.Normalize();
//                faceDir = transform.InverseTransformDirection(worldFaceDir);
//                AI_move.updateCommandData(faceDir);
//                AI_move.execute(this);
//                
//                if(distance_from_node < .8){
//                     shortest_path_index += 1;   
//                }
//            }
//            else{
//                isMoving = false;
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.RETURN);
//                return;
//            }
//            if(lookTarget != null){
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.YELLOW);
//                return;
//            }
//        }
//    }
//
//    public virtual void return_to_default(){
//        if(alertness == State.RETURN){
//            if(sound_detection(sound_location) && lookTarget == null){                
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.YELLOW_AUDIO);
//                return;
//            }
//            calc_shortest_path(transform.position, default_position);
//
//            if(shortest_path_index < path.length()){
//                Node current_node = path.get_node(shortest_path_index);
//                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
//                
//                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
//                worldFaceDir.Normalize();
//                faceDir = transform.InverseTransformDirection(worldFaceDir);
//                AI_move.updateCommandData(faceDir);
//                AI_move.execute(this);
//                
//                if(distance_from_node < .8){
//                     shortest_path_index += 1;   
//                }
//            }
//            else{
//                isMoving = false;
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.GREEN);
//                return;
//            }
//            if(lookTarget != null){
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.YELLOW);
//                return;
//            }
//        }
//    }
//
//    public virtual void yellow_audio(){
//        if(alertness == State.YELLOW_AUDIO){
//            if(sound_detection(sound_location) && lookTarget == null){                
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//            }
//            calc_shortest_path(transform.position, sound_location);
//            sound_location = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
//            
//            if(shortest_path_index < path.length()){
//                Node current_node = path.get_node(shortest_path_index);
//                float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
//
//                Vector2 worldFaceDir = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
//                worldFaceDir.Normalize();
//                faceDir = transform.InverseTransformDirection(worldFaceDir);
//                AI_move.updateCommandData(faceDir);
//                AI_move.execute(this);
//                
//                if(distance_from_node < .8){
//                     shortest_path_index += 1;   
//                }
//            }
//            else{
//                isMoving = false;
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.RETURN);
//                return;
//            }
//            if(lookTarget != null){
//                shortest_path_index = 0;
//                shortest_path_calculated = false;
//                run_state(State.YELLOW);
//                return;
//            }
//        }
//    }
//
//    public virtual void yellow_alertness(){
//        if(alertness == State.YELLOW){
//            if(lookTarget != null){
//                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
//                worldFaceDir.Normalize();
//                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
//                
//                AI_move.updateCommandData(localDir);
//                AI_move.execute(this);
//                dec_state_timer = 0;
//                inc_state_timer += Time.deltaTime;
//                last_seen = lookTarget.transform.position; 
//                if(inc_state_timer > state_change_time){
//                    inc_state_timer = 0;
//                    run_state(State.RED);
//                }
//            }
//            if(lookTarget == null){
//                isMoving = false;
//                inc_state_timer = 0;
//                dec_state_timer += Time.deltaTime;
//                
//                if(dec_state_timer > state_change_time){
//                    shortest_path_index = 0;
//                    shortest_path_calculated = false;
//                    run_state(State.CHASE);
//                }
//            }
//        }
//    }
//
//	public virtual void red_alertness(){
//        if(alertness == State.RED){
//            if(lookTarget != null){
//                Vector2 worldFaceDir = lookTarget.transform.position - transform.position;
//                worldFaceDir.Normalize();
//                Vector2 localDir = transform.InverseTransformDirection(worldFaceDir);
//                AI_move.updateCommandData(localDir);
//                AI_move.execute(this);
//                AI_aim.updateCommandData(faceDir);
//                AI_aim.execute(this);
//                attack_timer += Time.deltaTime;
//                if(attack_timer > attack_time_confirmation){
//                    attack_timer = 0;
//                    AI_attack.execute(this);
//                }
//            }
//            if(lookTarget == null){
//                isMoving = false;
//                attack_timer = 0;
//                dec_state_timer += Time.deltaTime;
//                if(dec_state_timer > state_change_time){
//                    dec_state_timer = 0;
//                    AI_disableAim.execute(this);
//                    run_state(State.YELLOW);
//                }
//            }
//        }
//    }
//}
