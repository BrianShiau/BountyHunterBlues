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
	}

	public override void die(){
		Destroy (gameObject);
	}
}
