using UnityEngine;
using System.Collections;

public class DynamicSprite : MonoBehaviour {

    private SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
        mySprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        mySprite.sortingOrder = Mathf.RoundToInt(-1 * transform.position.y * 20);
    }
}
