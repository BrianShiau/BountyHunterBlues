using UnityEngine;
using System.Collections;

public class StaticSprite : MonoBehaviour {

    private SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
        mySprite = GetComponent<SpriteRenderer>();
        mySprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
