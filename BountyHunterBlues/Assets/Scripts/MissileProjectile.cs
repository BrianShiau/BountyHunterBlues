using UnityEngine;
using System.Collections;

public class MissileProjectile : Projectile {
    
    public GameObject ExplosionObject;

    private Vector2 dir;
    private MissileEnemy owner;

    public static MissileProjectile Create(GameObject prefab, Vector3 position, Vector3 rotation)
    {
        GameObject newGameObject = Instantiate(prefab, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z)) as GameObject;
        return newGameObject.GetComponent<MissileProjectile>();
    }

	// Use this for initialization
	public override void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();

        // move in straight line
        Vector2 newPos = speed * dir * Time.deltaTime;
        transform.Translate(newPos);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger && col.gameObject != owner.gameObject)
        {
            GameObject hitObj = col.gameObject;
            if (hitObj.GetComponent<EnemyActor>() == null) // ignore enemy colliders
            {
                
                MissileExplosion.Create(ExplosionObject, transform.position);
                Destroy(gameObject);

            }
        }
        else if (col.tag == "Wall")
        {
            MissileExplosion.Create(ExplosionObject, transform.position);
            Destroy(gameObject);
        }
    }

    public void setInitialDir(Vector2 dir)
    {
        this.dir = transform.InverseTransformDirection(dir);

        this.dir.Normalize();
    }

    public void setOwner(MissileEnemy owner)
    {
        this.owner = owner;
    }
}
