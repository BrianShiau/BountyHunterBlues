using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, Interactable {

	// use animation for this later
	public Sprite openSprite;
	public bool closed;
	public bool specialDoor;
    public Room nextRoom;

    private PlayerActor player;
	private NPC npc;

    void Start()
    {
        closed = true;
        player = FindObjectOfType<PlayerActor>();
		npc = GetComponent<NPC> ();
    }

    public void runInteraction()
    {
		if (specialDoor) {
			if (npc) {
				npc.runInteraction ();
			}
			return;
		}
        if (closed)
        {
            closed = false;
            GetComponent<SpriteRenderer>().sprite = openSprite;

            Destroy(GetComponent<BoxCollider2D>());
            Destroy(transform.GetChild(0).gameObject);

            player.openDoorTo(nextRoom);

            // pictures arent cut right, offset for now
            // transform.Translate(0.0f, 0.0f, 0.0f);
        }
    }

	public void EndInteraction(){
	}
}
