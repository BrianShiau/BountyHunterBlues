using UnityEngine;
using System.Collections;

public class BarrelRotation : MonoBehaviour {

    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite upSprite;
    public Sprite rightSprite;

    public Sprite downAttackSprite;
    public Sprite leftAttackSprite;
    public Sprite upAttackSprite;
    public Sprite rightAttackSprite;

    public const float attackTime = .1f;


    private GameActor.Direction orientation;
    private DogEnemy myActor;
    private Vector2 faceDir;
    private float attackTimer;
    
	// Use this for initialization
	void Start () {
        orientation = GameActor.Direction.LEFT;
        myActor = GetComponentInParent<DogEnemy>();
        faceDir = myActor.faceDir;
        attackTimer = attackTime;
	}
	
	// Update is called once per frame
	void Update () {
        faceDir = myActor.faceDir;
        if (myActor.attackedThisFrame())
            attackTimer = 0;
        if (attackTimer < attackTime)
            attackTimer += Time.deltaTime;
        
        if (orientation == Actor.Direction.LEFT)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = leftSprite;
            else
                GetComponent<SpriteRenderer>().sprite = leftAttackSprite;
            float angle = Mathf.Atan2(-1 * faceDir.x, -1 * faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, 1 * Vector3.forward);
        }
        else if(orientation == Actor.Direction.DOWN)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = downSprite;
            else
                GetComponent<SpriteRenderer>().sprite = downAttackSprite;
            float angle = Mathf.Atan2(-1 * faceDir.y, 1 * faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, 1 * Vector3.forward);
        }
        else if (orientation == Actor.Direction.RIGHT)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = rightSprite;
            else
                GetComponent<SpriteRenderer>().sprite = rightAttackSprite;
            float angle = Mathf.Atan2(1 * faceDir.x, 1 * faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, 1 * Vector3.forward);
        }
        else if (orientation == Actor.Direction.UP)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = upSprite;
            else
                GetComponent<SpriteRenderer>().sprite = upAttackSprite;
            float angle = Mathf.Atan2(1 * faceDir.y, -1 * faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, 1 * Vector3.forward);
        }
        

        
	}

    public void setOrientation (Actor.Direction dir)
    {
        orientation = dir;
    }
}
