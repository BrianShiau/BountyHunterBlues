using UnityEngine;
using System.Collections;

public class camera_lerp : MonoBehaviour {

	private Vector3 camera_world_position;
	public GameObject lerp_to;
	// Use this for initialization
	void Start () {
		camera_world_position = transform.TransformPoint(transform.position);
		//Debug.Log(transform.TransformPoint(transform.position));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
