using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Laser : MonoBehaviour {

    public float scaleFactor;

    private EnemyActor myEnemy;

	// Use this for initialization
	void Start () {
        myEnemy = GetComponentInParent<EnemyActor>();
	}
	
	// Update is called once per frame
	void Update () {
        EnemyActor actor = GetComponentInParent<EnemyActor>();
        AIState currState = actor.getCurrentState();
        if (currState.get_state() == State.NEUTRAL)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (currState.get_state() == State.ALERT)
            GetComponent<SpriteRenderer>().color = Color.yellow;
        else if (currState.get_state() == State.AGGRESIVE)
            GetComponent<SpriteRenderer>().color = Color.red;
		float angle = Mathf.Atan2(actor.faceDir.y, actor.faceDir.x) * Mathf.Rad2Deg
			+ 180 + myEnemy.transform.localRotation.eulerAngles.z; // corrected for sprite angle
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        float distance = actor.sightDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, actor.transform.TransformDirection(actor.faceDir), actor.sightDistance);
        IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
        foreach (RaycastHit2D hit in sortedHits)
        {
			if (hit.collider != null && hit.collider.gameObject != myEnemy.gameObject && hit.collider.gameObject.name != "Feet_Collider" && hit.collider.tag != "Fence")
            {
                distance = hit.distance;
                break;
            }
        }

        transform.localScale = new Vector3(scaleFactor * distance, transform.localScale.y, transform.localScale.z);
    }
}
