using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Sprite[] sprites;
	public int health;

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().sprite = sprites [health-1];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setHealth(int hp){
		health = hp;
		//GetComponent<SpriteRenderer> ().sprite = sprites [health-1];
		StartCoroutine (FlashHealthBar());
	}

	IEnumerator FlashHealthBar(){
		GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
		GetComponent<SpriteRenderer> ().sprite = sprites [health-1];

		for (int i = 0; i < 3; i++) {
			GetComponent<SpriteRenderer> ().sprite = sprites [health];
			yield return new WaitForSeconds (.1f);
			GetComponent<SpriteRenderer> ().sprite = sprites [health-1];
			yield return new WaitForSeconds (.1f);
		}

		yield return new WaitForSeconds (1);
		GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, .75f);
	}
}
