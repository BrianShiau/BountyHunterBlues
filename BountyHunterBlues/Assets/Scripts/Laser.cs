using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Laser : MonoBehaviour {

    public float scaleFactor;

    private EnemyActor myEnemy;
    private SpriteRenderer laserSprite;
    private float defaultDist;

	// Use this for initialization
	void Start () {
        myEnemy = GetComponentInParent<EnemyActor>();
        laserSprite = GetComponent<SpriteRenderer>();
        defaultDist = myEnemy.sightDistance;

    }
	
	// Update is called once per frame
	void Update () {
        AIState currState = myEnemy.getCurrentState();
		if (currState.get_state () == State.NEUTRAL) {
            laserSprite.color = Color.green;
			if (myEnemy.getClosestAttackable() is PlayerActor) {
                laserSprite.enabled = true;
			} else {
                laserSprite.enabled = false;
			}
		} else if (currState.get_state () == State.ALERT) {
            laserSprite.color = Color.yellow;
            laserSprite.enabled = true;
		} else if (currState.get_state () == State.AGGRESIVE) {
            laserSprite.color = Color.red;
		}
		float angle = Mathf.Atan2(myEnemy.faceDir.y, myEnemy.faceDir.x) * Mathf.Rad2Deg
			+ 180 + myEnemy.transform.localRotation.eulerAngles.z; // corrected for sprite angle
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        float distance = defaultDist;

        if (myEnemy.getClosestAttackable() is PlayerActor)
        {
            float distToTarget = Vector2.Distance(transform.position, myEnemy.getClosestAttackable().transform.position);
            if (distToTarget < distance)
                distance = distToTarget;
        }

        else
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, myEnemy.transform.TransformDirection(myEnemy.faceDir), myEnemy.sightDistance);
            IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
            foreach (RaycastHit2D hit in sortedHits)
            {
                if (hit.collider != null && hit.collider.gameObject != myEnemy.gameObject && hit.collider.gameObject.name != "Feet_Collider" && hit.collider.tag != "Fence" && !hit.collider.isTrigger)
                {
                    distance = hit.distance;
                    break;
                }
            }
        }

        transform.localScale = new Vector3(scaleFactor * distance, transform.localScale.y, transform.localScale.z);
    }
}
