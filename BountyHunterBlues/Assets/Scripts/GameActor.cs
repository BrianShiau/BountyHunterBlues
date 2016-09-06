using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    public float moveSpeed; // subject to change based on testing
    public Vector2 faceDir; // normalized vector that indicates the center of the vision cone
    public Vector2 aimDir; // normalized vector that indicates direction of aim (only useful if isAiming is true)
    public float fov; // angle for visual sight and melee strike
    public float meleeDistance;
    public float sightDistance;

    public bool isAiming; // will need to specify "isAiming with what" later for special items
    public int healthPool;
    public GameActor lookTarget; // null unless look ray collides with an unobstructed valid GameActor
    public GameActor aimTarget; // null unless isAiming is true and aim ray collides with an unobstructed valid GameActor

    public abstract void attack(); 
    public abstract void interact();
	public abstract void die();

    public virtual void Start()
    {
        isAiming = false;
        lookTarget = null;
        aimTarget = null;
    }

    void Update()
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        bool foundLookTarget = false;
        foreach (GameObject actorObject in ActorObjects)
        {
            if (actorObject != this.gameObject)
            {
                Vector2 worldVector = actorObject.transform.position - transform.position;
                worldVector.Normalize();
                Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
                if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) <= fov / 2)
                {
                    RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, worldVector, sightDistance);

                    if (hitinfo.collider != null && hitinfo.collider.tag == "GameActor")
                    {
                        Debug.DrawRay(transform.position, worldVector * sightDistance, Color.blue);
                        lookTarget = actorObject.GetComponent<GameActor>(); // arbitrarily chooses first valid target in LoS as "lookTarget" should change to list later
                        foundLookTarget = true;
                    }

                }
            }
        }

        if (!foundLookTarget)
            lookTarget = null;

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
        Debug.Log("GameActor took Damage");
		if (healthPool == 0) {
			die ();
		}
    }

    public void acquireLookTarget()
    {
        
    }

    public virtual void aim(Vector2 dir)
    {
        isAiming = true;
        dir.Normalize();
        aimDir = dir;
        faceDir = dir;

        Vector2 worldDir = transform.TransformDirection(dir);
        RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, worldDir, sightDistance);
        Debug.DrawRay(transform.position, worldDir * sightDistance, Color.red);
        
        if (hitinfo.collider != null && hitinfo.collider.tag == "GameActor")
        {
            Debug.Log("Have aimTarget");
            aimTarget = hitinfo.collider.GetComponent<GameActor>();
        }
        else
            aimTarget = null;

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
        transform.Translate(newPos);
    }
}
