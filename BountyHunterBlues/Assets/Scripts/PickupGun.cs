using UnityEngine;
using System.Collections;

public class PickupGun : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "GameActor" && col.GetComponent<GameActor>() is PlayerActor)
        {
            PlayerActor pActor = (PlayerActor)col.GetComponent<GameActor>();
            pActor.hasGun = true;
            Destroy(gameObject);
        }
    }
}
