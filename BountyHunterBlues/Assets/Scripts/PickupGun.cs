using UnityEngine;
using System.Collections;

public class PickupGun : MonoBehaviour {

	public Vector3 startSize;
	public Vector3 endSize;

	void Start(){
	}

	void Update(){
		//float t = (Mathf.Sin(Time.time * 10.0f) + 1) / 2.0f;
		float t = Mathf.PingPong(Time.time * 2.0f, 1.0f);
		transform.localScale = Vector3.Lerp (startSize, endSize, t);
	}

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "GameActor" && col.GetComponent<GameActor>() is PlayerActor)
        {
            PlayerActor pActor = (PlayerActor)col.GetComponent<GameActor>();
			pActor.EnableGun();
            Destroy(gameObject);
        }
    }
}
