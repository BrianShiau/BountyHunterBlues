using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerLaser : MonoBehaviour {
    public float scaleFactor;

    PlayerActor player;
    SpriteRenderer laserSprite;
	// Use this for initialization
	void Start () {
        player = GetComponentInParent<PlayerActor>();
        laserSprite = GetComponent<SpriteRenderer>();
        
	}
	
	// Update is called once per frame
	void Update () {
        float red = laserSprite.color.r;
        float green = laserSprite.color.g;
        float blue = laserSprite.color.b;
        if (player.isAiming)
        {
            laserSprite.color = new Color(red, green, blue, 1.0f);
        }
        else
        {
            laserSprite.color = new Color(red, green, blue, 0.0f);
        }

        
    }

    public void changeLaserDir(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
            + 180 + player.transform.localRotation.eulerAngles.z; // corrected for sprite angle
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float distance = player.sightDistance;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, player.transform.TransformDirection(player.faceDir), player.sightDistance);
        IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
        foreach (RaycastHit2D hit in sortedHits)
        {
			if (hit.collider != null && hit.collider.gameObject != player.gameObject && hit.collider.gameObject.name != "Feet Collider" && hit.collider.tag != "Fence")
            {
                distance = hit.distance;
                break;
            }
        }

        transform.localScale = new Vector3(scaleFactor * distance, transform.localScale.y, transform.localScale.z);
    }

    public void updatePivot(Vector2 worldPos)
    {
        transform.position = worldPos;
    }
    
}
