using UnityEngine;
using System.Collections;

public class DynamicSprite : MonoBehaviour {

    private SpriteRenderer mySprite;
    private Transform SpriteLayerPoint;

	// Use this for initialization
	void Start () {
        mySprite = GetComponent<SpriteRenderer>();
        SpriteLayerPoint = transform.Find("SpriteLayerPoint");
       
    }
	
	// Update is called once per frame
	void Update () {
        float yPos = SpriteLayerPoint != null ? SpriteLayerPoint.position.y : transform.position.y;
        mySprite.sortingOrder = Mathf.RoundToInt(-1 * yPos * 20);
    }
}
