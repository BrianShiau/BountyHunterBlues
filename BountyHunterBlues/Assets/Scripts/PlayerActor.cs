using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class PlayerActor : GameActor
{

    public override void Start()
    {
        base.Start();
        healthPool = 3;
    }

    public override void attack()
    {
        if (isAiming)
        {
            Debug.Log("Player shoots");
            if (aimTarget != null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
            {
                aimTarget.takeDamage();
                if (!aimTarget.isAlive())
                    aimTarget = null;
            }
        }
        else
        {
            Debug.Log("attack happening on left click");
            if (lookTarget != null && Vector2.Distance(lookTarget.transform.position, transform.position) <= meleeDistance)
            {
                lookTarget.takeDamage();
                if (!lookTarget.isAlive())
                    lookTarget = null;
            }

        }
        
    }

    public override void interact()
    {

    }

	public override void die()
	{
		// reset the game here for now
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    protected override void runVisionDetection()
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        List<GameActor> seenActors = new List<GameActor>();
        foreach (GameObject actorObject in ActorObjects)
        {
            if (actorObject != this.gameObject) // ignore myself
            {
                Vector2 worldVector = actorObject.transform.position - transform.position;
                worldVector.Normalize();
                Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
                if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, sightDistance);
                    IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance); // sorted by ascending by default
                    foreach (RaycastHit2D hitinfo in sortedHits)
                    {
                        GameObject hitObj = hitinfo.collider.gameObject;
                        if (hitObj.tag != "GameActor")
                            // obstruction in front, ignore the rest of the ray
                            break;

                        else if (!seenActors.Contains(hitObj.GetComponent<GameActor>()))
                        {
                            // the next obj in the ray line is a GameActor we haven't accounted for, add it
                            seenActors.Add(hitObj.GetComponent<GameActor>());
                        }

                        // else the next obj in the ray line is a GameActor we've seen, just ignore it and keep moving down the ray
                    }
                 }
             }
        }

        if (seenActors.Count == 0)
            lookTarget = null;
        else
        {
            // make the closest GameActor in seenActors the new lookTarget
            float dist = sightDistance;
            foreach (GameActor actor in seenActors)
            {
                float nextDist = Vector2.Distance(actor.gameObject.transform.position, transform.position);
                if (nextDist <= dist)
                {
                    dist = nextDist;
                    lookTarget = actor;
                }
            }
            Vector2 worldVector = lookTarget.transform.position - transform.position;
            worldVector.Normalize();
            Debug.DrawRay(transform.position, worldVector * sightDistance, Color.blue);
        }
    }
}
