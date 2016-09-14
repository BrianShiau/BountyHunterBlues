using UnityEngine;
using System.Collections;

public class BarrelRotation : MonoBehaviour {

    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite upSprite;
    public Sprite rightSprite;

    private GameActor.Direction orientation;
    
	// Use this for initialization
	void Start () {
        orientation = GameActor.Direction.LEFT;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (orientation == GameActor.Direction.LEFT)
        {
            GetComponent<SpriteRenderer>().sprite = leftSprite;
            float angle = Mathf.Atan2(-1 * GetComponentInParent<AIActor>().faceDir.x, -1 * GetComponentInParent<AIActor>().faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if(orientation == GameActor.Direction.DOWN)
        {
            GetComponent<SpriteRenderer>().sprite = downSprite;
            float angle = Mathf.Atan2(-1 * GetComponentInParent<AIActor>().faceDir.y, 1 * GetComponentInParent<AIActor>().faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if (orientation == GameActor.Direction.RIGHT)
        {
            GetComponent<SpriteRenderer>().sprite = rightSprite;
            float angle = Mathf.Atan2(1 * GetComponentInParent<AIActor>().faceDir.x, 1 * GetComponentInParent<AIActor>().faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if (orientation == GameActor.Direction.UP)
        {
            GetComponent<SpriteRenderer>().sprite = upSprite;
            float angle = Mathf.Atan2(1 * GetComponentInParent<AIActor>().faceDir.y, -1 * GetComponentInParent<AIActor>().faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        

        
	}

    public void setOrientation (GameActor.Direction dir)
    {
        orientation = dir;
    }
}
