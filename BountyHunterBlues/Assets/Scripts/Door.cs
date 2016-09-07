using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	// use animation for this later
	public Sprite openSprite;

	public bool closed;

	private GameObject player;

	// Use this for initialization
	void Start () {
		// use this when we can tag player with player 
		// player = GameObject.FindGameObjectWithTag ("Player");
		GameObject[] gameActors = GameObject.FindGameObjectsWithTag("GameActor");
		foreach (GameObject gameActor in gameActors) {
			if ( gameActor.GetComponent<GameActor>() is PlayerActor ) {
				player = gameActor;
			}
		}
		closed = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (player) {
			if (closed) {
				if ((player.transform.position - transform.position).magnitude < 3) {
					closed= false;
					GetComponent<SpriteRenderer> ().sprite = openSprite;

					Destroy (GetComponent<BoxCollider2D> ());

					// pictures arent cut right, offset for now
					transform.Translate (0.0f, 0.5f, 0.0f);
				}
			}
		}
	}
}
