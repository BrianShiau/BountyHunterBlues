using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;



public abstract class EnemyActor : GameActor {

	protected StateManager _stateManager;
	protected AIState current_state;
    protected bool hasAttacked;
    private bool alert;
    private bool chasing;
    private bool confused;
    protected Vector2 last_seen;

    private Vector2 last_neutral_position;
    private Vector2 initial_faceDir;
    private Vector2 audio_location;
    private GameObject player;
    private PlayerActor playerActor;
    private float confused_look_time;
    private float confused_look_threshold;
    private float facedir_inc;
    private Vector2 move_facedir;

    private bool shortest_path_calculated;
    private int path_index;
    public int path_threshold;
    private bool shot;

    private Transform raySource;
	// UI Reactions
	Animator reactionAnim;
	int reactionStack;

	private GameObject directionPointer;
	private SpriteRenderer marker;

	//prefab
	public float audio_distance;
    public float transition_time;
    public float confused_transition_time;
    public float rotation_speed;

	public GameObject fence;
	public GameObject fenceCollider;

	public override void Start(){
		base.Start();
        player = GameObject.Find("0_Player");
        playerActor = (PlayerActor) player.GetComponent(typeof(PlayerActor));
        hasAttacked = false;
        confused = false;
        chasing = false;
        confused_look_time = 1;
        confused_look_threshold = 1;
        facedir_inc = 0.5f;
        _stateManager = new StateManager(transition_time, confused_transition_time);
        current_state = new NeutralDog(null);
        last_neutral_position = transform.position;
        
        path.set_threshold(path_threshold);
        shortest_path_calculated = false;
        path_index = 0;
        initial_faceDir = faceDir;
        audio_location = new Vector2(Int32.MaxValue, Int32.MaxValue);
        alert = false;
        shot = false;
        last_seen = new Vector2(Int32.MaxValue, Int32.MaxValue);

        if (transform.FindChild ("Reactions")) {
            reactionAnim = transform.FindChild ("Reactions").GetComponent<Animator> ();
            reactionAnim.speed = 10.0f;
            reactionStack = 0;
        }
		directionPointer = transform.FindChild ("DirectionPointer").gameObject;
        raySource = transform.Find("RaySource");
		marker = transform.FindChild ("Marker").gameObject.GetComponent<SpriteRenderer> ();
    }

    public override void Update(){
        hasAttacked = false;
        base.Update();

		if (alert || closestAttackable is PlayerActor) {
			directionPointer.GetComponent<SpriteRenderer> ().enabled = false;
		} else {
			directionPointer.GetComponent<SpriteRenderer> ().enabled = true;
		}

		float angle = Mathf.Atan2 (faceDir.y, faceDir.x);

		directionPointer.transform.eulerAngles = new Vector3(
			directionPointer.transform.eulerAngles.x,
			directionPointer.transform.eulerAngles.y,
			-90+angle*Mathf.Rad2Deg);

		Vector3 offset = new Vector3 (0, .25f, 0);
		float distanceScale = 1.5f;
		directionPointer.transform.position = new Vector3(
			transform.position.x + faceDir.x * distanceScale, 
			transform.position.y + faceDir.y * distanceScale,
			transform.position.z)
			+ offset;

		if (playerActor.InTacticalMode ()) {
			marker.enabled = true;
		} else {
			marker.enabled = false;
		}
    }

    public PlayerActor get_player_actor(){
        return playerActor;
    }

    public GameObject get_player_object(){
        return player;
    }

    public Vector2 get_last_seen(){
        return last_seen;
    }

    public void set_last_seen(Vector2 value){
        last_seen = value;
    }

    public void set_alert(bool value){
        alert = value;
    }

    public bool is_alert(){
        return alert;
    }

    public void set_chasing(bool value){
        chasing = value;
    }

    public bool is_chasing(){
        return chasing;
    }

    public void perform_confusion(){
        if(Math.Abs(faceDir.y) >= 1 && Math.Abs(faceDir.x) >= 1){
            confused_look_time += Time.deltaTime;
            if(confused_look_time > confused_look_threshold){
                move_facedir = new Vector2(faceDir.y + facedir_inc, faceDir.x + facedir_inc);
                facedir_inc = -facedir_inc;
                confused_look_time = 0;
            }
        }
        if(Math.Abs(faceDir.y) < 1 && Math.Abs(faceDir.x) < 1){
            confused_look_time += Time.deltaTime;
            if(confused_look_time > confused_look_threshold){
                move_facedir = new Vector2(faceDir.y + facedir_inc, faceDir.x);
                facedir_inc = -facedir_inc;
                confused_look_time = 0;
            }
        }
        if(Math.Abs(faceDir.y) >= 1 && Math.Abs(faceDir.x) < 1){
            confused_look_time += Time.deltaTime;
            if(confused_look_time > confused_look_threshold){
                move_facedir = new Vector2(faceDir.y, faceDir.x + facedir_inc);
                facedir_inc = -facedir_inc;
                confused_look_time = 0;
            }
        }
        if(Math.Abs(faceDir.y) < 1 && Math.Abs(faceDir.x) >= 1){
            confused_look_time += Time.deltaTime;
            if(confused_look_time > confused_look_threshold){
                move_facedir = new Vector2(faceDir.y + facedir_inc, faceDir.x);
                facedir_inc = -facedir_inc;
                confused_look_time = 0;
            }
        }
        Vector2 temp = Vector2.MoveTowards(faceDir, move_facedir, rotation_speed * Time.deltaTime);
        temp.Normalize();
        faceDir = temp;
    }

    public void reset_confused_time(){
        confused_look_time = 1;
        move_facedir = faceDir;
    }

    public void set_confused(bool value){
        confused = value;
    }

    public bool get_confused(){
        return confused;
    }

    public bool player_is_cloaked(){
        return playerActor.isCloaked();
    }

    public void is_confused(){
        if(playerActor.isCloaked() && alert && !chasing){
            set_confused(true);
			if (reactionAnim) {
				reactionAnim.SetInteger ("State", 2);
				Invoke ("resetReactionAnim", 2);
				reactionStack++;
			}
        }
    }

    public Vector2 get_audio_location(){
        return audio_location;
    }

    public void set_audio_location(Vector2 location, bool shot){
        this.shot = shot;
        audio_location = location;
    }

    public bool sound_heard(){
        if(Vector2.Distance(transform.position, audio_location) <= audio_distance && shot){
            shot = false;
            set_alert(true);
            path.set_audio_detected(true);
            set_shortest_path_calculated(false);
            calc_shortest_path(transform.position, get_audio_location());
            if(path.length() == 0){
                return false;
            }

			RemoveFence ();
			if (reactionAnim) {
				reactionAnim.SetInteger ("State", 1);
				Invoke ("resetReactionAnim", 2);
				reactionStack++;
			}

            return true;
        }
        return false;
    }

    public void calc_shortest_path(Vector3 from, Vector3 to){
        if(!shortest_path_calculated){
            path.initialize(from, to);
            path.calc_path();
            path.set_audio_detected(false);
            if(path.length() > path_threshold){
                path.clear();
                alert = false;
                audio_location = new Vector2(Int32.MaxValue, Int32.MaxValue);
            }
            shortest_path_calculated = true;
        }
    }

    private void resetReactionAnim(){
        if(--reactionStack == 0)
            reactionAnim.SetInteger ("State", 0);
    }

    public void set_shortest_path_calculated(bool value){
        shortest_path_calculated = value;
    }

    public Vector2 get_initial_faceDir(){
        return initial_faceDir;
    }

    public float get_node_transition_threshold(){
        return node_transition_threshold;
    }

    public void reset_path_index(){
        path_index = 0;
    }

    public void inc_path_index(){
        path_index += 1;
    }

    public int get_path_index(){
        return path_index;
    }

    public int path_length(){
        return path.length();
    }

    public Vector2 get_neutral_position(){
        return last_neutral_position;
    }

    public void set_neutral_position(Vector2 last_neutral_position){
        this.last_neutral_position = last_neutral_position;
    }

    public override GameActor[] runVisionDetection(float fov, float sightDistance){
        PlayerActor actorObject = GameObject.FindObjectOfType<PlayerActor>();
        List<GameActor> GameActors = new List<GameActor>();
        Vector3 rayOrigin;
        if (raySource == null)
            rayOrigin = transform.position;
        else
            rayOrigin = raySource.position;
        Vector2 worldVector = actorObject.transform.position - rayOrigin;
        worldVector.Normalize();
        Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
        if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2){
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, sightDistance);
            IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance); // sorted by ascending by default
            foreach (RaycastHit2D hitinfo in sortedHits){      
                if (hitinfo.collider.isTrigger) // only deal with trigger colliders for finding attackable target
                {
                    GameObject hitObj = hitinfo.collider.gameObject;
                    if (hitObj.tag != "GameActor")
                        // obstruction in front, ignore the rest of the ray
                        break;
                    //else if (hitObj.GetComponent<GameActor>() is PlayerActor && hitObj.GetComponent<GameActor>().isVisible())
                    else if (hitObj.GetComponent<GameActor>() is PlayerActor)
                    {
                        // PlayerActor
                        GameActors.Add(hitObj.GetComponent<GameActor>());
                        break;
                    }
                }
            }
        }
        return GameActors.ToArray();
    }

    public bool attackedThisFrame()
    {
        return hasAttacked;
    }

    public AIState getCurrentState()
    {
        return current_state;
    }

	public override void die(){
		base.die ();
		RemoveFence ();
		gameActorAnimator.SetBool ("isHit", true);
		transform.FindChild ("Reactions").gameObject.SetActive(false);
		transform.FindChild ("Feet_Collider").gameObject.SetActive(false);
		transform.FindChild ("DirectionPointer").gameObject.SetActive(false);
		GetComponent<BoxCollider2D> ().enabled = false;
	}

	public void RemoveFence(){
		if (fence) {
			Destroy (fence);
		}
		if (fenceCollider) {
			Destroy (fenceCollider);
		}
	}

    public override void disableAim()
    {
        throw new NotImplementedException();
    }

    public override void aim(Vector2 worldPos)
    {
        throw new NotImplementedException();
    }
}
