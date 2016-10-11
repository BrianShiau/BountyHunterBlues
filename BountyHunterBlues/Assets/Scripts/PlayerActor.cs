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
	public bool gun_fired;
	public bool knifeAttacked;
	public bool enemyHit;
	public bool inTacticalMode;
	public bool tookDamage;
	public float reloadTime;
	public float cloakTime;

	private float lastShotTime;
	private float cloakTimer;
	private bool visible;
	private Vector2 aimPoint;

	public int currentLevel;
	public NPC openingText;
	public static int deaths = 0;

	// UI
	public Image gunImage;
	public Slider gunSlider;
	public GameObject gunSliderObject;
	public Image gunSliderFill;

	private Image hitFlash;

	public Vector3 fire_location;
	private Grid mGrid;

	// Audio
	public AudioClip hitShotSound;
	public AudioClip missShotSound;
	public AudioClip rechargeSound;
	private AudioSource gunAudioSource;
	private AudioSource gunRechargeSource;

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
		fire_location = new Vector3(0, 0, 0);

		// play opening text only once
		if (deaths == 0) {
			//play opening text
			if (openingText) {
				openingText.Start ();
				openingText.runInteraction ();
			}
		}

		if (hasGun) {
			gunImage.enabled = true;
			gunSliderObject = gunSlider.gameObject;
			gunSliderObject.SetActive (true);
			gunSliderFill.color = Color.green;
		}
		else {
			gunImage.enabled = false;
			gunSliderObject = gunSlider.gameObject;
			gunSliderObject.SetActive (false);
		}
		hitFlash = GameObject.FindGameObjectWithTag ("HitFlash").GetComponent<Image>();
		mGrid = GameObject.Find("GridOverlay").GetComponent<Grid>();
	}

	public override void Update()
	{
		base.Update();
		lastShotTime += Time.deltaTime;
		knifeAttacked = false;
		gun_fired = false;
		enemyHit = false;
		tookDamage = false;

		if (!visible)
			cloakTimer += Time.deltaTime;
		if (cloakTimer >= cloakTime)
			visible = true;

		gunSlider.value = lastShotTime;
		if (lastShotTime >= 2) {
			gunSliderFill.color = Color.green;
		}else if(lastShotTime >= 1){
			gunSliderFill.color = Color.yellow;
		} else {
			gunSliderFill.color = Color.red;
		}
	}

	public override void rangedAttack()
	{
		if (visible)
		{
			if (hasGun && lastShotTime >= reloadTime)
			{
				Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
				Vector2 aimVector = transform.InverseTransformPoint(worldPoint); // implied "minus player position wrt its coordinate frame" (which is zero)
				aimVector.Normalize();

				Vector2 worldDir = transform.TransformDirection(aimVector);
				RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, worldDir, sightDistance);
				IEnumerable<RaycastHit2D> sortedHits = hits.OrderBy(hit => hit.distance);

				GameActor aimTarget = null;
				foreach (RaycastHit2D hitinfo in sortedHits) {
					GameObject obj = hitinfo.collider.gameObject;
					aimPoint = hitinfo.point;
					if (obj.tag != "GameActor"){
						// non-game actor in front, obstruction blocking aim
						break;
					} else if(hitinfo.collider.GetComponent<GameActor>().isVisible() && hitinfo.collider.gameObject != gameObject)
					{
						// visible GameActor in Ray that is unobstructed and not me
						aimTarget = hitinfo.collider.GetComponent<GameActor>();
						break;
					}
					// else, GameActor is either me (which i should ignore) or invisible (which i should also ignore), continue down the ray
				}
				notifyEnemies();
				fire_location = transform.position;
				gun_fired = true;
				StartCoroutine(Utility.drawLine (transform.position, new Vector3(aimPoint.x, aimPoint.y, 0.0f), Color.red, 1f));
				if (aimTarget != null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
				{
					enemyHit = true;
					aimTarget.takeDamage();
					if (!aimTarget.isAlive())
						aimTarget = null;
				}
				lastShotTime = 0;
			}
		}
	}

	private void notifyEnemies()
	{
		EnemyActor[] enemies = FindObjectsOfType<EnemyActor>();
		GridPoint gPoint = mGrid.worldToGrid(transform.position);
		Node node = mGrid.nodes[gPoint.X, gPoint.Y];

		LinkedList<Node> queue = new LinkedList<Node>();
		LinkedList<Node> toBeReset = new LinkedList<Node>();
		queue.AddLast(node);

		while (queue.Count > 0)
		{
			Node curr = queue.First.Value;
			queue.RemoveFirst();
			toBeReset.AddFirst(curr);
			foreach(EnemyActor enemy in enemies)
			{
				if (Vector2.Distance(enemy.transform.position, mGrid.gridToWorld(curr.point.X, curr.point.Y)) < mGrid.unitsize / 2.0f){
					
					enemy.set_audio_location(transform.position);
					
				}
			}
			foreach(NodeConnection connection in curr.connections)
			{
				if(!connection.destination.visited)
				{
					connection.destination.visited = true;
					queue.AddLast(connection.destination);
				}
			}
		}

		foreach(Node n in toBeReset)
		{
			n.visited = false;
		}

	}

	public override void meleeAttack()
	{
		if (visible)
		{
			knifeAttacked = true;
			if (closestAttackable != null && Vector2.Distance(closestAttackable.transform.position, transform.position) <= meleeDistance)
			{
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
			cloakTimer = 0;
			if (GetComponentInChildren<HealthBar> ()) {
				GetComponentInChildren<HealthBar> ().setHealth (health);
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
						GameObject hitObj = hitinfo.collider.gameObject;

						if (hitObj.tag != "GameActor")
							// obstruction in front, ignore the rest of the ray
							break;

						else if(hitObj.GetComponent<GameActor>() is AIActor && hitObj.GetComponent<GameActor>().isVisible() 
							&& !seenActors.Contains(hitObj.GetComponent<GameActor>()))
							// the next obj in the ray line is a AIActor we haven't accounted for, add it
							seenActors.Add(hitObj.GetComponent<GameActor>());   

						// else the next obj in the ray line is a PlayerActor or an AIActor we've seen, just ignore it and keep moving down the ray
					}
				}
			}
		}
		return seenActors.ToArray();
	}

	public override bool isVisible()
	{
		return visible;
	}

	public float getLastShotTime(){
		return lastShotTime;
	}
}