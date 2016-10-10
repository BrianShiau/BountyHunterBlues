using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;



public abstract class EnemyActor : GameActor {

	protected StateManager _stateManager;
	protected bool hasAttacked;
	protected AIState current_state;

	//prefab
	public int audio_distance;

	public override void Start(){
		base.Start();
		hasAttacked = false;
		_stateManager = new StateManager(2);
		current_state = new NeutralDog();
	}

	public override void Update(){
		hasAttacked = false;
		base.Update();
	}

    public override void die(){
        Destroy (gameObject);
    }

    public override GameActor[] runVisionDetection(float fov, float sightDistance){
        PlayerActor actorObject = GameObject.FindObjectOfType<PlayerActor>();
        List<GameActor> GameActors = new List<GameActor>();

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
                else if (hitObj.GetComponent<GameActor>() is PlayerActor && hitObj.GetComponent<GameActor>().isVisible())
                {
                    // PlayerActor
                    GameActors.Add(hitObj.GetComponent<GameActor>());
                    break;
                }
            }
        }

        return GameActors.ToArray();
    }

}
