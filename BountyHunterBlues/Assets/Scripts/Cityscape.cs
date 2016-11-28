using UnityEngine;
using System.Collections;

public class Cityscape : MonoBehaviour {
	private GameObject player;
	private Vector3 startingPosition;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("0_Player");
		startingPosition = transform.position;
	}

	// Update is called once per frame
	void Update () {
		Vector3 translation = (player.transform.position - (player.transform.position - startingPosition) / 2F);
		transform.position = new Vector3 (translation.x, 
			player.transform.position.y - ((player.transform.position.y - startingPosition.y)/1.5f), 
			transform.position.z);
	}
}
