using UnityEngine;
using System.Collections;

public class collision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("here");
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log("ok");
		Debug.Log(collision.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
