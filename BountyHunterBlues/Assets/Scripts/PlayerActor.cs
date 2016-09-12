using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerActor : GameActor
{
    public bool hasGun;
    public float reloadTime;

    private float lastShotTime;

	// UI
	public Image gunImage;
	public Slider gunSlider;
	public GameObject gunSliderObject;
	public Image gunSliderFill;

    public override void Start()
    {
        base.Start();
        hasGun = false;
        lastShotTime = reloadTime;

		gunImage.enabled = false;
		gunSliderObject = gunSlider.gameObject;
		gunSliderObject.SetActive (false);
    }

    public override void Update()
    {
        base.Update();
        lastShotTime += Time.deltaTime;

		gunSlider.value = lastShotTime;
		if (lastShotTime >= 2) {
			gunSliderFill.color = Color.green;
		}else if(lastShotTime >= 1){
			gunSliderFill.color = Color.yellow;
		} else {
			gunSliderFill.color = Color.red;
		}
    }

    public override void attack()
    {
        if (hasGun && isAiming && lastShotTime >= reloadTime)
        {
            Debug.Log("Player shoots");
            if (aimTarget != null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
            {
                aimTarget.takeDamage();
                if (!aimTarget.isAlive())
                    aimTarget = null;
            }
            lastShotTime = 0;
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
        interactionTarget.runInteraction();
    }

	public override void die()
	{
		GameObject.FindGameObjectWithTag ("DeathFlash").GetComponent<Image>().enabled = true;

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



        // see what interactables are in my vision cone within interactionDistance and pick closest as interactionTarget
        float shortestInteractionDist = float.MaxValue;
        bool foundInteractable = false;
        GameObject[] InteractableObjects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject interactableObject in InteractableObjects)
        {
            Vector2 worldVector = interactableObject.transform.position - transform.position;
            worldVector.Normalize();
            Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
            if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, toTargetDir, interactionDistance);
                if (hit && hit.collider != null && hit.collider.tag == "Interactable" && hit.distance <= interactionDistance)
                {
                    interactionTarget = hit.collider.GetComponent<Interactable>();
                    shortestInteractionDist = hit.distance;
                    foundInteractable = true;
                }
            }
        }

        if (!foundInteractable)
            interactionTarget = null;
    }
}
