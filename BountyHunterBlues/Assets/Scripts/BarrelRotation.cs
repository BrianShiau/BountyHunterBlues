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
    private float attackTimer;
    
	// Use this for initialization
	void Start () {
        orientation = GameActor.Direction.LEFT;
        myActor = GetComponentInParent<DogEnemy>();
        attackTimer = attackTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (myActor.attackedThisFrame())
            attackTimer = 0;
        if (attackTimer < attackTime)
            attackTimer += Time.deltaTime;
        
        if (orientation == GameActor.Direction.LEFT)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = leftSprite;
            else
                GetComponent<SpriteRenderer>().sprite = leftAttackSprite;
            float angle = Mathf.Atan2(-1 * GetComponentInParent<AIActor>().faceDir.x, -1 * GetComponentInParent<AIActor>().faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if(orientation == GameActor.Direction.DOWN)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = downSprite;
            else
                GetComponent<SpriteRenderer>().sprite = downAttackSprite;
            float angle = Mathf.Atan2(-1 * GetComponentInParent<AIActor>().faceDir.y, 1 * GetComponentInParent<AIActor>().faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if (orientation == GameActor.Direction.RIGHT)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = rightSprite;
            else
                GetComponent<SpriteRenderer>().sprite = rightAttackSprite;
            float angle = Mathf.Atan2(1 * GetComponentInParent<AIActor>().faceDir.x, 1 * GetComponentInParent<AIActor>().faceDir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        else if (orientation == GameActor.Direction.UP)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = upSprite;
            else
                GetComponent<SpriteRenderer>().sprite = upAttackSprite;
            float angle = Mathf.Atan2(1 * GetComponentInParent<AIActor>().faceDir.y, -1 * GetComponentInParent<AIActor>().faceDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, -1 * Vector3.forward);
        }
        

        
	}

    public void setOrientation (GameActor.Direction dir)
    {
        orientation = dir;
    }
}
