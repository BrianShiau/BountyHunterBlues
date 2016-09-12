using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

	// use animation for this later
	public Sprite openSprite;
	public bool closed;

    public void runInteraction()
    {
        if (closed)
        {
            closed = false;
            GetComponent<SpriteRenderer>().sprite = openSprite;

            Destroy(GetComponent<BoxCollider2D>());

            // pictures arent cut right, offset for now
            transform.Translate(0.0f, 0.5f, 0.0f);
        }
    }
}
