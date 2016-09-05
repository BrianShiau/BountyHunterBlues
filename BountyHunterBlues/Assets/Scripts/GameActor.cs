﻿using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    public float moveSpeed; // subject to change based on testing
    public Vector2 faceDir; // normalized vector that indicates the center of the vision cone
    public Vector2 aimDir; // normalized vector that indicates direction of aim (only useful if isAiming is true)
    public float fov; // angle for visual sight and melee strike
    public float meleeDistance;
    public float sightDistance;

    protected bool isAiming; // will need to specify "isAiming with what" later for special items
    protected int healthPool;
    protected GameActor lookTarget; // null unless look ray collides with an unobstructed valid GameActor
    protected GameActor aimTarget; // null unless isAiming is true and aim ray collides with an unobstructed valid GameActor

    public abstract void attack(); 
    public abstract void interact();
	public abstract void die();

    public virtual void Start()
    {
        isAiming = false;
        lookTarget = null;
        aimTarget = null;
    }

    public bool isAlive()
    {
        return healthPool > 0;
    }
    
    public void kill()
    {
        healthPool = 0;
		die ();
    }

    public void takeDamage()
    {
        healthPool--;
        Debug.Log("I've taken Damage");
		if (healthPool == 0) {
			die ();
		}
    }

    public void acquireLookTarget()
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        foreach(GameObject actorObject in ActorObjects)
        {
            Vector2 toTargetDir = actorObject.transform.position - transform.position;
            toTargetDir.Normalize();
            if (Vector2.Angle(faceDir, toTargetDir) <= fov/2)
            {
                RaycastHit hitinfo;
                Physics.Raycast(transform.position, toTargetDir, out hitinfo, sightDistance);
                Debug.DrawRay(transform.position, sightDistance * toTargetDir, Color.blue);
                if(hitinfo.collider != null && hitinfo.collider.tag == "GameActor")
                {
                    lookTarget = actorObject.GetComponent<GameActor>();
                }
            }

        }
    }

    public virtual void aim(Vector2 dir)
    {
        isAiming = true;
        dir.Normalize();
        aimDir = dir;
        faceDir = dir;

        RaycastHit hitinfo;
        Physics.Raycast(transform.position, dir, out hitinfo, sightDistance);
        if (hitinfo.collider != null && hitinfo.collider.tag == "GameActor")
        {
            Debug.Log("Added aimTarget");
            aimTarget = hitinfo.collider.GetComponent<GameActor>();
        }

    }

    public virtual void disableAim()
    {
        isAiming = false;
        aimTarget = null;
    }

    public virtual void move(Vector2 dir)
    {
        dir.Normalize();
        faceDir = dir;
        Vector2 newPos = moveSpeed * dir * Time.deltaTime;
        gameObject.transform.Translate(new Vector3(newPos.x, 0, newPos.y));
    }

	
}
