using UnityEngine;
using System.Collections;

public class MissileProjectile : Projectile {

    public MissileExplosion missileExplosion;

	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update () {

        // move in straight line
	}

    public void OnTriggerEnter2D(Collider2D col)
    {
        
    }
}
