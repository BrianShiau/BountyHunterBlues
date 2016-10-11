﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;



public abstract class EnemyActor : GameActor {

	protected StateManager _stateManager;
	protected AIState current_state;
    protected bool hasAttacked;
    private bool alert;
    protected Vector2 last_seen;

    private Vector2 last_neutral_position;
    private Vector2 initial_faceDir;
    private Vector2 audio_location;
    
    public PathFinding path;
    private bool shortest_path_calculated;
    public float node_transition_threshold;
    private int path_index;

	//prefab
	public float audio_distance;
    public float transition_time;
    public float rotation_speed;

	public override void Start(){
		base.Start();
		hasAttacked = false;
		_stateManager = new StateManager(transition_time);
		current_state = new NeutralDog(null);
        last_neutral_position = transform.position;
        path = gameObject.GetComponent<PathFinding>();
        shortest_path_calculated = false;
        path_index = 0;
        initial_faceDir = faceDir;
        audio_location = new Vector2(Int32.MaxValue, Int32.MaxValue);
        alert = false;
        last_seen = new Vector2(Int32.MaxValue, Int32.MaxValue);
	}

	public override void Update(){
		hasAttacked = false;
        base.Update();
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

    public Vector2 get_audio_location(){
        return audio_location;
    }

    public void set_audio_location(Vector2 location){
        //Debug.Log("HERERERERER");
        audio_location = location;
    }

    public bool sound_heard(){
        //Debug.Log(transform.position);
        //Debug.Log(audio_location);
        //Debug.Log(Vector2.Distance(transform.position, audio_location));
        if(Vector2.Distance(transform.position, audio_location) <= audio_distance){
        //    Debug.Log("in");
            set_alert(true);
            set_shortest_path_calculated(false);
            calc_shortest_path(transform.position, get_audio_location());
            return true;
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

    public override void die(){
        Destroy (gameObject);
    }

    public override GameActor[] runVisionDetection(float fov, float sightDistance){
        PlayerActor actorObject = GameObject.FindObjectOfType<PlayerActor>();
        List<GameActor> GameActors = new List<GameActor>();

        Vector2 worldVector = actorObject.transform.position - transform.position;
        worldVector.Normalize();
        Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
        if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2){
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, sightDistance);
            IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance); // sorted by ascending by default
            foreach (RaycastHit2D hitinfo in sortedHits){
                GameObject hitObj = hitinfo.collider.gameObject;
                if (hitObj.tag != "GameActor")
                    // obstruction in front, ignore the rest of the ray
                    break;
                else if (hitObj.GetComponent<GameActor>() is PlayerActor && hitObj.GetComponent<GameActor>().isVisible()){
                    // PlayerActor
                    GameActors.Add(hitObj.GetComponent<GameActor>());
                    break;
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
}
