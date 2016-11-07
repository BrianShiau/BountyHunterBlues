using UnityEngine;
using System.Collections;

public class camera_lerp : MonoBehaviour {

	private Vector3 camera_world_position;
	private bool tactical_mode;
	private bool can_lerp;
	public GameObject lerp_to;
	public float lerp_speed;
	public float lerp_distance;
	private float original_lerp_distance;
	private Vector3 lerp_back_position;

	// Use this for initialization
	void Start () {
		tactical_mode = false;
		can_lerp = false;
		original_lerp_distance = GetComponent<Camera>().orthographicSize;
		update_camera_position();
	}
	
	// Update is called once per frame
	void Update () {
		update_camera_position();
		lerp();
	}

	public void update_camera_position(){
		camera_world_position = transform.position;
		lerp_back_position = transform.parent.position;
	}

	public void set_lerp(bool value){
		can_lerp = value;
	}

	public void toggle_tactical_mode(){
		tactical_mode = !tactical_mode;
	}

	public void set_camera_position(Vector3 position){
		transform.localPosition = position;
	}

	public Vector2 get_lerp_to(){
		return new Vector2(lerp_to.transform.position.x, lerp_to.transform.position.y);
	}

	public Vector2 get_lerp_back(){
		return new Vector2(lerp_back_position.x, lerp_back_position.y);
	}
	public Vector2 get_camera_position(){
		return new Vector2(camera_world_position.x, camera_world_position.y);
	}

	public void lerp(){
		if(can_lerp){
			if(tactical_mode){
				Vector2 temp = Vector2.Lerp(get_camera_position(), get_lerp_to(), Time.deltaTime * lerp_speed);
				transform.position = new Vector3(temp.x, temp.y, transform.position.z);
				GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, lerp_distance, Time.deltaTime * lerp_speed);
			}
			else{
				Vector2 temp = Vector2.Lerp(get_camera_position(), get_lerp_back(), Time.deltaTime * lerp_speed);
				transform.position = new Vector3(temp.x, temp.y, transform.position.z);
				GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, original_lerp_distance, Time.deltaTime * lerp_speed);
			}
			if(0.01 > Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.y), new Vector2(0, 0))){
				set_lerp(false);
			}
		}
		else{
			set_camera_position(new Vector3(0, 0, -10));
		}
	}
}
