using UnityEngine;
using System.Collections;

public class PatrolManager :{
	public PatrolPoint[] patrol_point;
	GameObject AI_reference;
	float patrol_distance;
	float distance_threshold;
	int waypoint;
	bool is_cycle;

	PatrolManager(GameObject obj, bool is_cycle = false){
		AI_referece = obj;
		this.is_cycle = is_cycle;
	}

	public Vector2 get_next_waypoint(){
		patrol_distance = Vector2.Distance(AI_reference.transform.position, patrol_point[waypoint]);
		if(patrol_distance < distance_threshold){
			waypoint += 1;
		}

		return patrol_point[waypoint];
	}
}
