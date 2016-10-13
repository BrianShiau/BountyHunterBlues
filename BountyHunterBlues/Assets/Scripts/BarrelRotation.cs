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
        }
        else if(orientation == Actor.Direction.DOWN)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = downSprite;
            else
                GetComponent<SpriteRenderer>().sprite = downAttackSprite;
        }
        else if (orientation == Actor.Direction.RIGHT)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = rightSprite;
            else
                GetComponent<SpriteRenderer>().sprite = rightAttackSprite;
        }
        else if (orientation == Actor.Direction.UP)
        {
            if (attackTimer >= attackTime)
                GetComponent<SpriteRenderer>().sprite = upSprite;
            else
                GetComponent<SpriteRenderer>().sprite = upAttackSprite;
        }
        float angle = Mathf.Atan2(myActor.faceDir.y, myActor.faceDir.x) * Mathf.Rad2Deg
            + 180 + myActor.transform.localRotation.eulerAngles.z; // corrected for sprite angle
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);



    }

    public void setOrientation (Actor.Direction dir)
    {
        orientation = dir;
    }
}
