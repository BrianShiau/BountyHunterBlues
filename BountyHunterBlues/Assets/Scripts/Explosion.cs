using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class Explosion : MonoBehaviour {

    public float explosionRadius;

    protected abstract bool isValidHit(GameActor hitActor);
    protected abstract void explosionHit(GameActor hitActor); 

    public virtual void Start()
    {
        List<GameActor> hitByExplosion = getHitGameActors();
        if(hitByExplosion.Count > 0)
        {
            foreach (GameActor actor in hitByExplosion)
                explosionHit(actor);
        }
    }

    // callback made at end of explosion animation
    public void OnExplosionEnd()
    {
        Destroy(gameObject);
    }

    private List<GameActor> getHitGameActors()
    {
        List<GameActor> hitActors = new List<GameActor>();
        GameActor[] actorsInScene = FindObjectsOfType<GameActor>();
        foreach(GameActor actor in actorsInScene)
        {
            if(isValidHit(actor) && Vector2.Distance(transform.position, actor.transform.position) < explosionRadius)
            {
                Vector2 rayDir = (actor.transform.position - transform.position).normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDir, explosionRadius);
                IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
                foreach (RaycastHit2D hit in hits)
                {
                    if(hit.collider.isTrigger && isValidHit(actor))
                    {
                        GameObject hitObj = hit.collider.gameObject;
                        if (hitObj.tag != "GameActor")
                            break;
                        else
                            hitActors.Add(hitObj.GetComponent<GameActor>());
                    }
                }
            }
        }
        return hitActors;
    }
}
