using UnityEngine;
using System.Collections;
using System;

public class TerminalBall : NPCActor, Interactable {
	public Door opensThisdoor;
    public Transform moveTo;

    private bool interacted = false;
    private int path_index;

	// Use this for initialization
	public override void Start(){
        base.Start();
        path = gameObject.GetComponent<PathFinding>();
        path_index = 0;
	}
	
	// Update is called once per frame
	public override void Update(){
        base.Update();
        if(interacted == false){
        	patrol();
        }
        if(interacted == true){
        	performAction();
        }
	}

	public void patrol(){
        if(patrolManager.get_patrol_length() > 0){
			if (patrolManager.get_next_patrol_point ().x == Int32.MaxValue && patrolManager.get_next_patrol_point ().y == Int32.MaxValue) {
				stopMove ();
			} 
			else{
				Vector2 worldFace = patrolManager.get_next_patrol_point () - new Vector2 (transform.position.x, transform.position.y);
				worldFace.Normalize ();
				faceDir = transform.InverseTransformDirection (worldFace);
				move(faceDir);
			}
		}
	}

	public void performAction(){
		if(get_path_index() < path.length()){
			Node current_node = path.get_node(get_path_index());
	        float distance_from_node = Vector2.Distance(transform.position, current_node.worldPosition);
	                
	        Vector2 worldFace = current_node.worldPosition - new Vector2(transform.position.x, transform.position.y);
	        worldFace.Normalize();
	        faceDir = transform.InverseTransformDirection(worldFace);
	        setIsPatrolling(false);
	        move(faceDir);
	        if(distance_from_node < node_transition_threshold){
	            inc_path_index();   
	        }
	        if(get_path_index() == path.length()){
	        	audioManager.Play("Beep");
				opensThisdoor.specialDoor = false;
				opensThisdoor.runInteraction();
	        }
    	}
	}

	public void reset_path_index(){
        path_index += 1;
    }

    public void inc_path_index(){
        path_index += 1;
    }

    public int get_path_index(){
        return path_index;
    }


	public void runInteraction(){
		if(interacted == false){
        	audioManager.Play("Beep");
			interacted = true;
			path.initialize(transform.position, moveTo.position);
	        path.calc_path();
	        this.gameObject.transform.GetChild(0).gameObject.active = false;
		}
	}
}
