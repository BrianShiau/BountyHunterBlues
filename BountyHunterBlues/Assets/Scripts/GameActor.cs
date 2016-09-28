using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class GameActor : MonoBehaviour, IEquatable<GameActor>, Hearable
{

    public float moveSpeed; // subject to change based on testing
    public Vector2 faceDir; // normalized vector that indicates the center of the vision cone
    public Vector2 aimDir; // normalized vector that indicates direction of aim (only useful if isAiming is true)
    public float fov; // angle for visual sight and melee strike
    public float meleeDistance;
    public float sightDistance;
    public float interactionDistance;

    public bool isAiming; // will need to specify "isAiming with what" later for special items
    public bool isMoving;
    public bool isVisible;
    public int healthPool;
    public GameActor lookTarget; // null unless look ray collides with an unobstructed valid GameActor
    public GameActor aimTarget; // null unless isAiming is true and aim ray collides with an unobstructed valid GameActor
    public Interactable interactionTarget; // null unless runVisionDetection sets this to an Interactable

    public Animator GameActorAnimator;
    public enum Direction
    {
        DOWN, LEFT, UP, RIGHT
    }

    public abstract void attack();
    public abstract void meleeAttack();
    public abstract void interact();
	public abstract void die();
    public abstract void initAudio();
    public abstract void runAudio();

    public abstract void runVisionDetection();

    public virtual void Start()
    {
        isAiming = false;
        isMoving = false;
        isVisible = true;
        lookTarget = null;
        aimTarget = null;
        faceDir.Normalize();
        aimDir.Normalize();
        GameActorAnimator = GetComponent<Animator>();
        initAudio();
    }

    public virtual void Update()
    {
        runVisionDetection();
        //if (lookTarget == null)
        //{
            Vector2 faceDirWorld = transform.TransformDirection(faceDir);
            //Debug.DrawRay(transform.position, faceDirWorld * sightDistance, Color.green);
        //}
        updateAnimation();
        runAudio();
    }

    public bool Equals(GameActor other)
    {
        return other != null && other.gameObject == gameObject;
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

    public virtual void takeDamage()
    {
        healthPool--;
        Debug.Log("GameActor took Damage");
		if (healthPool == 0) {
			die ();
		}
    }

    public virtual void aim(Vector2 dir)
    {
        isAiming = true;
        dir.Normalize();
        aimDir = dir;
        faceDir = dir;

        // before you do anything, update your lookTarget
        runVisionDetection();

        Vector2 worldDir = transform.TransformDirection(dir);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldDir, sightDistance);
        IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
        //Debug.DrawRay(transform.position, worldDir * sightDistance, Color.red);

        aimTarget = null;
        foreach (RaycastHit2D hitinfo in sortedHits) {
            GameObject obj = hitinfo.collider.gameObject;
            if (obj.tag != "GameActor")
                // non-game actor in front, obstruction blocking aim
                break;
            else if(hitinfo.collider.GetComponent<GameActor>().isVisible && hitinfo.collider.gameObject != gameObject)
            {
                // visible GameActor in Ray that is unobstructed and not me
                aimTarget = hitinfo.collider.GetComponent<GameActor>();
                break;
            }

            // else, GameActor is either me (which i should ignore) or invisible (which i should also ignore), continue down the ray
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
        transform.Translate(newPos);
        isMoving = true;
        
    }

    public virtual void updateAnimation()
    {
        // update moving state
        GameActorAnimator.SetBool("isMoving", isMoving);

        // update direction state
        if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
        {
            if (faceDir.y > 0)
                GameActorAnimator.SetInteger("Direction", (int)Direction.UP);
            else
                GameActorAnimator.SetInteger("Direction", (int)Direction.DOWN);
        }

        else
        {
            if (faceDir.x > 0)
                GameActorAnimator.SetInteger("Direction", (int)Direction.RIGHT);
            else
                GameActorAnimator.SetInteger("Direction", (int)Direction.LEFT);
        }

    }
}
