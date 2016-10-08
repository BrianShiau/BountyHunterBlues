using UnityEngine;
using System.Collections;

public class PatrolManager { 
	private PatrolPoint[] patrol_points;
	private GameObject AI_reference;
	private float patrol_distance;
	private float distance_threshold;
	private float wait_time;
	private int patrol_index;
	private bool is_cycle;
	private bool is_reverse;

	PatrolManager(GameObject obj, bool is_cycle = false){
		AI_reference = obj;
		this.is_cycle = is_cycle;
		is_reverse = false;
		wait_time = 0;
	}

	public Vector2 get_next_patrol_index(){
		patrol_distance = Vector2.Distance(AI_reference.transform.position, patrol_points[patrol_index].point.position);
		if(patrol_distance < distance_threshold){
			if(wait_time < patrol_points[patrol_index].wait_time){
				wait_time += Time.deltaTime;
			}
			else{
				wait_time = 0;
				if(is_cycle){
					if(is_reverse){
						if(patrol_index == 0){
							is_reverse = false;
						}
						else{
							patrol_index -= 1;
						}
					}
					else{
						if(patrol_index == patrol_points.Length - 1){
							is_reverse = true;
						}
						else{
							patrol_index += 1;
						}
	
					}			
				}
				else{
					if(patrol_index == patrol_points.Length - 1){
                        patrol_index = 0;
					}
					else{
						patrol_index += 1;
					}
				}
			}
		}
		

		return patrol_points[patrol_index].point.position;
	}
}
