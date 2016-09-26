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
    public bool gun_fired;
    public bool knifeAttacked;
    public bool inTacticalMode;
    public float reloadTime;
    public float cloakTime;

    private float lastShotTime;
    private float cloakTimer;

	public int currentLevel;
	public NPC openingText;
	public static int deaths = 0;

	// UI
	public Image gunImage;
	public Slider gunSlider;
	public GameObject gunSliderObject;
	public Image gunSliderFill;

	//public Image 

    public override void Start()
    {
        base.Start();
        lastShotTime = reloadTime;
        cloakTimer = 0;
        gun_fired = false;
        knifeAttacked = false;

		// play opening text only once
		if (deaths == 0) {
			//play opening text
			if (openingText) {
				openingText.Start ();
				openingText.runInteraction ();
			}
		}

		if (hasGun) {
			gunImage.enabled = true;
			gunSliderObject = gunSlider.gameObject;
			gunSliderObject.SetActive (true);
			gunSliderFill.color = Color.green;
		}
		else {
			gunImage.enabled = false;
			gunSliderObject = gunSlider.gameObject;
			gunSliderObject.SetActive (false);
		}
    }

    public override void Update()
    {
        base.Update();
        lastShotTime += Time.deltaTime;
        knifeAttacked = false;

        if (!isVisible)
            cloakTimer += Time.deltaTime;
        if (cloakTimer >= cloakTime)
            isVisible = true;

        gunSlider.value = lastShotTime;
		if (lastShotTime >= 2) {
			gunSliderFill.color = Color.green;
		}else if(lastShotTime >= 1){
			gunSliderFill.color = Color.yellow;
		} else {
			gunSliderFill.color = Color.red;
		}
    }

    public Vector3 bullet_shot(){
        if(gun_fired){
            gun_fired = false;
            return transform.position;
        }
        return new Vector3(0, 0, 0);
    }

    public override void attack()
    {
        if (isVisible)
        {
            if (hasGun && isAiming && lastShotTime >= reloadTime)
            {
                Debug.Log("Player shoots");
                gun_fired = true;
                if (aimTarget != null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
                {
                    aimTarget.takeDamage();
                    if (!aimTarget.isAlive())
                        aimTarget = null;
                }
                lastShotTime = 0;
            }
        }
        
    }

    public override void meleeAttack()
    {
        if (isVisible)
        {
            knifeAttacked = true;
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
        if(interactionTarget != null)
            interactionTarget.runInteraction();
    }

    public override void takeDamage()
    {
        base.takeDamage();
        if(isAlive())
        {
            isVisible = false;
            cloakTimer = 0;
			if (GetComponentInChildren<HealthBar> ()) {
				GetComponentInChildren<HealthBar> ().setHealth (healthPool);
			}
        }
    }

    public override void die()
	{
		GameObject.FindGameObjectWithTag ("DeathFlash").GetComponent<Image>().enabled = true;
		deaths++;

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

                        else if(hitObj.GetComponent<GameActor>() is AIActor && hitObj.GetComponent<GameActor>().isVisible 
                                && !seenActors.Contains(hitObj.GetComponent<GameActor>()))
                            // the next obj in the ray line is a AIActor we haven't accounted for, add it
                            seenActors.Add(hitObj.GetComponent<GameActor>());   

                        // else the next obj in the ray line is a PlayerActor or an AIActor we've seen, just ignore it and keep moving down the ray
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



        // see what interactables are near me and unobstructed and pick closest as interactionTarget
        Interactable closestInteractable = null;
        float closestDist = float.MaxValue;
        GameObject[] InteractableObjects = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject interactableObject in InteractableObjects)
        {
            
            float dist = Vector2.Distance(interactableObject.transform.position, transform.position);
            if (dist <= interactionDistance)
            {
                Vector2 worldVector = (interactableObject.transform.position - transform.position).normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, interactionDistance);
                IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
                foreach (RaycastHit2D hit in sortedHits)
                {
                    if (hit.collider.gameObject != gameObject && hit.collider.tag != "Interactable")
                        break;

                    if (hit.collider.tag == "Interactable" && hit.distance <= closestDist)
                    {
                        closestInteractable = (Interactable)hit.collider.GetComponent(typeof(Interactable));
                        closestDist = dist;
                    }
                }
            }
        }
        interactionTarget = closestInteractable;
    }

    public override void updateAnimation()
    {
        base.updateAnimation();
        if(inTacticalMode)
        {
            GameActorAnimator.SetBool("isMoving", false);
        }
        GameActorAnimator.SetBool("isKnifing", knifeAttacked);

        float red = gameObject.GetComponent<SpriteRenderer>().color.r;
        float green = gameObject.GetComponent<SpriteRenderer>().color.g;
        float blue = gameObject.GetComponent<SpriteRenderer>().color.b;
        if (!isVisible)
            GetComponent<SpriteRenderer>().color = new Color(red, blue, green, .5f);
        else
            GetComponent<SpriteRenderer>().color = new Color(red, blue, green, 1.0f);

    }

}
