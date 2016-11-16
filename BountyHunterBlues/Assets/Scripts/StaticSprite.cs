using UnityEngine;
using System.Collections;

public class StaticSprite : MonoBehaviour {

    public SpriteRenderer onTopOf;
    private SpriteRenderer mySprite;

	// Use this for initialization
	void Start () {
        if (onTopOf != null)
            GetComponent<SpriteRenderer>().sortingOrder = onTopOf.GetComponent<SpriteRenderer>().sortingOrder + 1;
        else
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-1 * transform.position.y * 20);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
