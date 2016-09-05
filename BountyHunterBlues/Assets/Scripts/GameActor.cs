using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    public float moveSpeed; // subject to change based on testing
    public Vector2 faceDir; // normalized vector that indiciates the center of the vision cone

    protected bool isAiming; // will need to specify "isAiming with what" later for special items
    protected int healthPool;
    protected GameActor lookTarget; // null unless look ray collides with an unobstructed valid GameActor
    protected GameActor aimTarget; // null unless isAiming is true and aim ray collides with an unobstructed valid GameActor

    public abstract void attack(); 
    public abstract void interact();
	public abstract void die();

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
		if (healthPool == 0) {
			die ();
		}
    }

    public virtual void aim(Vector2 dir)
    {
        isAiming = true;
        
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
