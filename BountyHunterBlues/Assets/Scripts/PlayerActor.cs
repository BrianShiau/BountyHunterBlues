using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerActor : GameActor
{
	public bool hasGun;
	public float reloadTime;
	public float cloakTime;

	private bool gun_fired;
	private bool knifeAttacked;
	private bool enemyHit;
	private bool inTacticalMode;
	private bool tookDamage;
	private bool cloaked;

	private float lastShotTime;
	private float cloakTimer;
	private bool visible;
	private Vector2 aimPoint;
	private Vector2 aimPoint2;
	private Vector2 aimPoint3;

	public int currentLevel;
	public NPC openingText;
	public int magazine_cap;
	private int magazine_size;
	private static int deaths = 0;

	// UI
	private Image gunImage;
	private Slider gunSlider;
	private GameObject gunSliderObject;
	private Image gunSliderFill;
	private Vector2 bulletStartPosition;


	private Vector2 secondRayPosition;
	private Vector2 thirdRayPosition;

	private Image hitFlash;
	Animator hitSmokeAnim;

	private GameObject mainBackground;
	private Vector3 startingPosition;

	private Grid mGrid;
    private Room mRoom;

	public override void Start()
	{
		base.Start();
		lastShotTime = reloadTime;
		cloakTimer = 0;
		gun_fired = false;
		enemyHit = false;
		knifeAttacked = false;
		tookDamage = false;
		visible = true;
		cloaked = false;
		magazine_size = magazine_cap;

		// play opening text only once
		if (deaths == 0) {
			//play opening text
			if (openingText) {
				openingText.Start ();
				openingText.runInteraction ();
			}
		}

		gunImage = GameObject.FindGameObjectWithTag ("GunImage").GetComponent<Image>();
		gunSlider = GameObject.FindGameObjectWithTag ("GunSlider").GetComponent<Slider>();
		gunSliderFill = GameObject.FindGameObjectWithTag ("GunFill").GetComponent<Image>();
		gunSliderObject = gunSlider.gameObject;
		if (hasGun) {
			EnableGun ();
		}
		else {
			gunImage.enabled = false;
			gunSliderObject.SetActive (false);
		}
		hitFlash = GameObject.FindGameObjectWithTag ("HitFlash").GetComponent<Image>();
		mainBackground = GameObject.FindGameObjectWithTag ("MainBackground");
		startingPosition = transform.position;
		mGrid = GameObject.Find("GridOverlay").GetComponent<Grid>();
		setBulletStartPosition(new Vector2(transform.position.x, transform.position.y));

		if (transform.FindChild ("hit animation smoke")) {
			hitSmokeAnim = transform.FindChild ("hit animation smoke").GetComponent<Animator> ();
		}

        // figure out what room I'm in and store a reference to that room
        Room[] roomsInScene = FindObjectsOfType<Room>();
        foreach(Room room in roomsInScene)
            if(room.inRoom(this))
            {
                mRoom = room;
                break;
            }
	}

	public override void Update()
	{
		base.Update();

		knifeAttacked = false;
		gun_fired = false;
		enemyHit = false;
		tookDamage = false;

		if (!visible)
			cloakTimer += Time.deltaTime;
		if (cloakTimer >= cloakTime){
			visible = true;
			cloaked = false;
		}

		gunSlider.value = lastShotTime;
		if (lastShotTime >= 2) {
			gunSliderFill.color = Color.green;
		}else if(lastShotTime >= 1){
			gunSliderFill.color = Color.yellow;
		} else {
			gunSliderFill.color = Color.red;
		}

		reload_magazine();

		mainBackground.transform.position = (transform.position - (transform.position - startingPosition)/10);
	}

	public bool isCloaked(){
		return cloaked;
	}

	public void reload_magazine(){
		if(magazine_size < magazine_cap){
			lastShotTime += Time.deltaTime;
			if(lastShotTime >= reloadTime){
				magazine_size += 1;
				lastShotTime = 0;
			}
		}
		else{
			lastShotTime = reloadTime;
		}
	}

	public void setBulletStartPosition(Vector2 trans){
		if (faceDir.y != 0 && Mathf.Abs(faceDir.y) >= Mathf.Abs(faceDir.x)){
            if (faceDir.y > 0){
                currDirection = Direction.UP;
            }
            else{
                currDirection = Direction.DOWN;
            }
        }
        else{
            if (faceDir.x > 0){
                currDirection = Direction.RIGHT;
            }
            else{
                currDirection = Direction.LEFT;
            }
        }
		if(currDirection == Direction.RIGHT){ 
			bulletStartPosition = new Vector2(trans.x + 1, trans.y);
		}
		if(currDirection == Direction.LEFT){ 
			bulletStartPosition = new Vector2(trans.x - 1, trans.y);
		}
		if(currDirection == Direction.UP){ 
			bulletStartPosition = new Vector2(trans.x, trans.y + 1);
		}
		if(currDirection == Direction.DOWN){ 
			bulletStartPosition = new Vector2(trans.x, trans.y - 1);
		}
	}

	public override void rangedAttack()
	{
		if (visible)
		{
			if (hasGun && magazine_size > 0)
			{
				if(magazine_size == magazine_cap){
					lastShotTime = 0;
				}
				magazine_size -= 1;
				Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
				Vector2 aimVector = transform.InverseTransformPoint(worldPoint); // implied "minus player position wrt its coordinate frame" (which is zero)
				aimVector.Normalize();
				faceDir = aimVector;


				Vector2 worldDir = transform.TransformDirection(aimVector);
				secondRayPosition = new Vector2(transform.position.x - 0.05f, transform.position.y + 0.05f);
				thirdRayPosition = new Vector2(transform.position.x + 0.05f, transform.position.y - 0.05f);

				RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldDir, sightDistance);
				RaycastHit2D[] hits2 = Physics2D.RaycastAll(secondRayPosition, worldDir, sightDistance);
				RaycastHit2D[] hits3 = Physics2D.RaycastAll(thirdRayPosition, worldDir, sightDistance);
				IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);
				IEnumerable<RaycastHit2D> sortedHits2 = hits2.OrderBy(hit2 => hit2.distance);
				IEnumerable<RaycastHit2D> sortedHits3 = hits3.OrderBy(hit3 => hit3.distance);

				GameActor aimTarget = null;
				foreach (RaycastHit2D hitinfo in sortedHits) {
                    if (hitinfo.collider.isTrigger)
                    { // only deal with trigger colliders for finding attackable target
                        GameObject obj = hitinfo.collider.gameObject;
                        aimPoint = hitinfo.point;
                        if (obj.tag != "GameActor")
                        {
                            // non-game actor in front, obstruction blocking aim
                            break;
                        }
                        else if (hitinfo.collider.GetComponent<GameActor>().isVisible() && hitinfo.collider.gameObject != gameObject)
                        {
                            // visible GameActor in Ray that is unobstructed and not me
                            aimTarget = hitinfo.collider.GetComponent<GameActor>();
                            break;
                        }
                    }
					// else, GameActor is either me (which i should ignore) or invisible (which i should also ignore), continue down the ray
				}
				foreach (RaycastHit2D hitinfo in sortedHits2) {
                    if (hitinfo.collider.isTrigger)
                    { // only deal with trigger colliders for finding attackable target
                        GameObject obj = hitinfo.collider.gameObject;
                        aimPoint2 = hitinfo.point;
                        if (obj.tag != "GameActor")
                        {
                            // non-game actor in front, obstruction blocking aim
                            break;
                        }
                        else if (hitinfo.collider.GetComponent<GameActor>().isVisible() && hitinfo.collider.gameObject != gameObject)
                        {
                            // visible GameActor in Ray that is unobstructed and not me
                            aimTarget = hitinfo.collider.GetComponent<GameActor>();
                            break;
                        }
                    }
					// else, GameActor is either me (which i should ignore) or invisible (which i should also ignore), continue down the ray
				}
				foreach (RaycastHit2D hitinfo in sortedHits3) {
                    if (hitinfo.collider.isTrigger)
                    { // only deal with trigger colliders for finding attackable target
                        GameObject obj = hitinfo.collider.gameObject;
                        aimPoint3 = hitinfo.point;
                        if (obj.tag != "GameActor")
                        {
                            // non-game actor in front, obstruction blocking aim
                            break;
                        }
                        else if (hitinfo.collider.GetComponent<GameActor>().isVisible() && hitinfo.collider.gameObject != gameObject)
                        {
                            // visible GameActor in Ray that is unobstructed and not me
                            aimTarget = hitinfo.collider.GetComponent<GameActor>();
                            break;
                        }
                    }
					// else, GameActor is either me (which i should ignore) or invisible (which i should also ignore), continue down the ray
				}
				notifyEnemies();
				gun_fired = true;

				setBulletStartPosition(new Vector2(transform.position.x, transform.position.y));
				StartCoroutine(Utility.drawLine (bulletStartPosition, new Vector3(aimPoint.x, aimPoint.y, 0.0f), Color.cyan, 1f));
				if (aimTarget != null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
				{
                    audioManager.Play("EnemyDeath");
					enemyHit = true;
					aimTarget.takeDamage();
					if (!aimTarget.isAlive())
						aimTarget = null;
				}
                if (audioManager.isPlaying("Gun"))
                    audioManager.Stop("Gun");
                audioManager.Play("Gun");
			}
		}
	}

	private void notifyEnemies(){

        // room optimization
        
        if (mRoom != null)
        {
            foreach (GameActor gameActor in mRoom.gameActorsInRoom)
            {
                if (gameActor is EnemyActor)
                {
                    EnemyActor enemy = (EnemyActor)gameActor;
                    enemy.set_audio_location(transform.position);
                }
            }
        }




        // old code
        /*
		EnemyActor[] enemies = FindObjectsOfType<EnemyActor>();
		GridPoint gPoint = mGrid.worldToGrid(transform.position);
		Node node = mGrid.nodes[gPoint.X, gPoint.Y];

		LinkedList<Node> queue = new LinkedList<Node>();
		LinkedList<Node> toBeReset = new LinkedList<Node>();
		queue.AddLast(node);

		while (queue.Count > 0){
			Node curr = queue.First.Value;
			queue.RemoveFirst();
			toBeReset.AddFirst(curr);
			foreach(EnemyActor enemy in enemies){
				if (Vector2.Distance(enemy.transform.position, mGrid.gridToWorld(curr.point.X, curr.point.Y)) < mGrid.unitsize / 2.0f){
					enemy.set_audio_location(transform.position);
				}
			}
			foreach(NodeConnection connection in curr.connections){
				if(!connection.destination.visited)
				{
					connection.destination.visited = true;
					queue.AddLast(connection.destination);
				}
			}
		}

		foreach(Node n in toBeReset){
			n.visited = false;
		}
        */
        
	}

	public override void meleeAttack()
	{
		if (visible)
		{
            acquireClosestAttackable();
			knifeAttacked = true;
			if (closestAttackable != null && Vector2.Distance(closestAttackable.transform.position, transform.position) <= meleeDistance)
			{
                audioManager.Play("EnemyDeath");
                enemyHit = true;
				closestAttackable.takeDamage();
				if (!closestAttackable.isAlive())
					closestAttackable = null;
			}
		}
	}

	public override void interact()
	{
		if(interactionTarget != null)
			interactionTarget.runInteraction();
	}

	public override void takeDamage()
	{
		base.takeDamage();
		if(isAlive())
		{
			tookDamage = true;
			visible = false;
			cloaked = true;
			cloakTimer = 0;
			if (GetComponentInChildren<HealthBar> ()) {
				GetComponentInChildren<HealthBar> ().setHealth (health);
			}
			if (hitSmokeAnim) {
				hitSmokeAnim.SetBool ("Hit", true);
				Invoke ("resetHitSmokeAnim", 0.1f);
			}
			StartCoroutine (PlayHitFlash());
		}
	}

	IEnumerator PlayHitFlash(){
		hitFlash.color = new Color (hitFlash.color.r, hitFlash.color.g, hitFlash.color.b, 1f);
		yield return new WaitForSeconds (.1f);
		while (hitFlash.color.a > 0) {
			hitFlash.color = new Color (hitFlash.color.r, hitFlash.color.g, hitFlash.color.b, hitFlash.color.a - .1f);
			yield return new WaitForSeconds (.1f);
		}
	}

	private void resetHitSmokeAnim(){
		hitSmokeAnim.SetBool ("Hit", false);
	}

	public override void die()
	{
		GameObject.FindGameObjectWithTag ("DeathFlash").GetComponent<Image>().enabled = true;
		deaths++;

		// reset the game here for now
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}



	public override void runAnimation()
	{
		base.runAnimation();
		if(inTacticalMode)
		{
			gameActorAnimator.SetBool("isMoving", false);
		}
		gameActorAnimator.SetBool("isKnifing", knifeAttacked);
		gameActorAnimator.SetBool("isShooting", gun_fired);
		gameActorAnimator.SetBool("tookDamage", tookDamage);

		float red = gameObject.GetComponent<SpriteRenderer>().color.r;
		float green = gameObject.GetComponent<SpriteRenderer>().color.g;
		float blue = gameObject.GetComponent<SpriteRenderer>().color.b;
		if (!visible)
			GetComponent<SpriteRenderer>().color = new Color(red, blue, green, .5f);
		else
			GetComponent<SpriteRenderer>().color = new Color(red, blue, green, 1.0f);

	}

    public override GameActor[] runVisionDetection(float fov, float sightDistance)
    {
        GameObject[] ActorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        List<GameActor> seenActors = new List<GameActor>();
        foreach (GameObject actorObject in ActorObjects)
        {
            if (actorObject != this.gameObject) // ignore myself
            {
                Vector2 worldVector = actorObject.transform.position - transform.position;
                worldVector.Normalize();
                Vector2 toTargetDir = transform.InverseTransformDirection(worldVector);
                if (Mathf.Abs(Vector2.Angle(faceDir, toTargetDir)) < fov / 2)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldVector, sightDistance);
                    IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance); // sorted by ascending by default
                    foreach (RaycastHit2D hitinfo in sortedHits)
                    {
                        if (hitinfo.collider.isTrigger) // only deal with trigger colliders for finding attackable target
                        {
                            GameObject hitObj = hitinfo.collider.gameObject;

                            if (hitObj.tag != "GameActor")
                                // obstruction in front, ignore the rest of the ray
                                break;

                            else if (hitObj.GetComponent<GameActor>() is EnemyActor && hitObj.GetComponent<GameActor>().isVisible()
                                && !seenActors.Contains(hitObj.GetComponent<GameActor>()))
                                // the next obj in the ray line is a AIActor we haven't accounted for, add it
                                seenActors.Add(hitObj.GetComponent<GameActor>());

                            // else the next obj in the ray line is a PlayerActor or an AIActor we've seen, just ignore it and keep moving down the ray
                        }
                    }
                }
            }
        }
        return seenActors.ToArray();
    }

	public override void OnTriggerEnter2D(Collider2D other)
	{
		base.OnTriggerEnter2D(other);
		SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
		SpriteRenderer healthBarRenderer = transform.FindChild("health bar_0").GetComponent<SpriteRenderer>();
		SpriteRenderer hitSmokeRenderer = transform.FindChild("hit animation smoke").GetComponent<SpriteRenderer>();

		healthBarRenderer.sortingOrder = mySprite.sortingOrder - 1;
		hitSmokeRenderer.sortingOrder = mySprite.sortingOrder + 1;
	}

	public void EnableGun(){
		hasGun = true;
		gunImage.enabled = true;
		gunSliderObject.SetActive (true);
		gunSliderFill.color = Color.green;
	}

    public override bool isVisible()
	{
		return visible;
	}

	public float getLastShotTime(){
		return lastShotTime;
	}

	public static int Deaths(){
		return deaths;
	}

	public static void SetDeaths(int num){
		deaths = num;
	}

	public bool InTacticalMode(){
		return inTacticalMode;
	}

	public void SetTacticalMode(bool mode){
		inTacticalMode = mode;
	}

    public void openDoorTo(Room otherRoom)
    {
        mRoom.openDoorTo(otherRoom);
    }
}