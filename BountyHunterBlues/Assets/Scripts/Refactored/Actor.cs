using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Actor : MonoBehaviour, Animatable, IEquatable<Actor> {

    public enum Direction
    {
        DOWN, LEFT, UP, RIGHT
    }

    // set by prefab
    public Vector2 faceDir;
    public float moveSpeed;
    public int health;
    public PatrolPoint[] patrolPoints;
    public AudioSerializable[] sources;

    protected AudioManager audioManager;
    //protected PatrolManager patrolManager;
    protected Animator gameActorAnimator;
    protected Direction currDirection;
    protected bool isMoving;
    protected bool markedToDie;
    

    
    public virtual void die()
    {
        markedToDie = true;
    }
    

    // Use this for initialization
    public virtual void Start () {
        gameActorAnimator = GetComponent<Animator>();
        audioManager = initAudioManager();
        //patrolManager = new PatrolManager();
        isMoving = false;
        markedToDie = false;
	}
	
	// Update is called once per frame
	public virtual void Update () {
        updateDirection();
        runAnimation();
        //patrolManager.updatePatrol();
	}

    public bool Equals(Actor other)
    {
        return other != null && other.gameObject == gameObject;
    }


    public virtual bool isVisible() // needs to be overriden by PlayerActor
    {
        return true;
    }

    public bool isAlive()
    {
        return health < 0;
    }

    public virtual void takeDamage()
    {
        health--;
        if (health == 0)
            die();
    }

    public void kill()
    {
        health = 0;
        die();
    }

    public void move(Vector2 dir)
    {
        dir.Normalize();
        faceDir = dir;

        Vector2 newPos = moveSpeed * dir * Time.deltaTime;
        transform.Translate(newPos);
        isMoving = true;
    }

    public void stopMove()
    {
        isMoving = false;
    }

    // implements animation for direction and moving. Override as needed
    public virtual void runAnimation()
    {
        if (gameActorAnimator != null)
        {
            gameActorAnimator.SetBool("isMoving", isMoving);
            gameActorAnimator.SetInteger("Direction", (int)currDirection);
        }
    }

    // implements default init for AudioManager. Needs AudioSerializable sources to be defined in prefab
    protected AudioManager initAudioManager()
    {
        List<DynamicAudioSource> dySources = new List<DynamicAudioSource>();
        foreach(AudioSerializable element in sources)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            NamedAudioSource namedSource = new NamedAudioSource();
            namedSource.name = element.sourceName;
            namedSource.source = source;
            List<NamedAudioClip> clips = new List<NamedAudioClip>();
            foreach(NamedAudioClip clip in element.Clips)
                clips.Add(clip);

            DynamicAudioSource dySource = new DynamicAudioSource(namedSource, clips);
            dySources.Add(dySource);
        }

        return new AudioManager(dySources);
    }

    protected void updateDirection()
    {
        // update direction state
        if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)) // up and down facing priority over left and right
        {
            if (faceDir.y > 0)
                currDirection = Direction.UP;
            else
                currDirection = Direction.DOWN;
        }

        else
        {
            if (faceDir.x > 0)
                currDirection = Direction.RIGHT;
            else
                currDirection = Direction.LEFT;
        }
    }

    
}
