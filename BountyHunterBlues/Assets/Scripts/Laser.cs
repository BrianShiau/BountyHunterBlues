using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Laser : MonoBehaviour {

    public float scaleFactor;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        AIActor actor = GetComponentInParent<AIActor>();
        if (actor.alertness == State.GREEN)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (actor.alertness == State.YELLOW)
            GetComponent<SpriteRenderer>().color = Color.yellow;
        else if (actor.alertness == State.RED)
            GetComponent<SpriteRenderer>().color = Color.red;
        float angle = Mathf.Atan2(-1 * GetComponentInParent<AIActor>().faceDir.x, -1 * GetComponentInParent<AIActor>().faceDir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);


        float distance = actor.sightDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, actor.transform.TransformDirection(actor.faceDir), actor.sightDistance);
        IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
        foreach (RaycastHit2D hit in sortedHits)
        {
            if (hit.collider != null && hit.collider.gameObject != GetComponentInParent<GameActor>().gameObject)
            {
                distance = hit.distance;
                break;
            }
        }

        transform.localScale = new Vector3(scaleFactor * distance, 1, 1);
    }
}
