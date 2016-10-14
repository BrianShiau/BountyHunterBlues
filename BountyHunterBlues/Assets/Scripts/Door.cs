using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

	// use animation for this later
	public Sprite openSprite;
	public bool closed;
	public bool specialDoor;

    void Start()
    {
        closed = true;

    }

    public void runInteraction()
    {
		if (specialDoor)
			return;
        if (closed)
        {
            closed = false;
            GetComponent<SpriteRenderer>().sprite = openSprite;

            Destroy(GetComponent<BoxCollider2D>());
            Destroy(transform.GetChild(0).gameObject);

            // pictures arent cut right, offset for now
            transform.Translate(0.0f, 0.5f, 0.0f);
        }
    }
}
