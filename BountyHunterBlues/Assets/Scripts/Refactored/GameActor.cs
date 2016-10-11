﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class GameActor : Actor, Vision
{

    // set by prefab
    public float fov; // angle for visual sight and melee strike
    public float meleeDistance;
    public float sightDistance;
    public float interactionDistance;

    protected GameActor closestAttackable; // closest attackable GameActor as acquired by runVisionDetection   
    protected Interactable interactionTarget; // null unless acquireInteractionTarget sets this to an Interactable

    public abstract void rangedAttack();
    public abstract void meleeAttack();
    public abstract void interact();
    public abstract GameActor[] runVisionDetection(float fov, float sightDistance);

    public override void Start()
    {
        base.Start();
        closestAttackable = null;
        interactionTarget = null;
    }

    public override void Update()
    {
        base.Update();
        acquireClosestAttackable();
        acquireInteractionTarget();
    }

    public GameActor getGameActor(){
        return this;
    }

    public GameActor getClosestAttackable(){
        return closestAttackable;
    }

    protected void acquireInteractionTarget()
    {
        // see what interactables are near me and unobstructed and pick closest as interactionTarget
        interactionTarget = null;    
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
                        interactionTarget = (Interactable)hit.collider.GetComponent(typeof(Interactable));
                        closestDist = dist;
                    }
                }
            }
        }
    }

    protected void acquireClosestAttackable()
    {
        closestAttackable = null;
        float dist = float.MaxValue;
        GameActor[] seenActors = runVisionDetection(fov, sightDistance);
        foreach (GameActor gameActor in seenActors)
        {
            float distBetween = Vector2.Distance(transform.position, gameActor.transform.position);
            if (distBetween < dist)
            {
                dist = distBetween;
                closestAttackable = gameActor;
            }
        }
    }
}
