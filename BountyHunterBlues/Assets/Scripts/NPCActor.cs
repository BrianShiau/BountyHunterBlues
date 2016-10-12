using UnityEngine;
using System.Collections;
using System;

public class NPCActor : Actor{

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
		if(patrolManager.get_next_patrol_point().x == Int32.MaxValue && patrolManager.get_next_patrol_point().y == Int32.MaxValue){
			stopMove ();
		}
		else if(patrolManager.get_patrol_length() > 0){
			Vector2 worldFace = patrolManager.get_next_patrol_point() - new Vector2(transform.position.x, transform.position.y);
			worldFace.Normalize();
			faceDir = transform.InverseTransformDirection(worldFace);
			move(faceDir);
		}
	}

	public override void die()
	{
		Destroy (gameObject);
	}
}
