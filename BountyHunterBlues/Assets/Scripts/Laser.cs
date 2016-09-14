using UnityEngine;
using System.Collections;

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


        float distance;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, actor.transform.TransformDirection(actor.faceDir), actor.sightDistance);
        distance = hit.distance;

        transform.localScale = new Vector3(scaleFactor * distance, 1, 1);
    }
}
