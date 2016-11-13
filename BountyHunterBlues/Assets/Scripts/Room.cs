using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {
    public List<GameActor> gameActorsInRoom;

	// Use this for initialization
	void Start () {
        GameActor[] gameActors = FindObjectsOfType<GameActor>();
        foreach (GameActor gameActor in gameActors)
            if (inRoom(gameActor))
                gameActorsInRoom.Add(gameActor);

	}

    public void openDoorTo(Room otherRoom)
    {
        foreach (GameActor gameActor in otherRoom.gameActorsInRoom)
            gameActorsInRoom.Add(gameActor);
    }

    public bool inRoom(GameActor actor)
    {
        Vector2 actorPosition = actor.transform.position;
        return actorPosition.x >= transform.position.x
            && actorPosition.x <= transform.position.x + transform.localScale.x
            && actorPosition.y >= transform.position.y
            && actorPosition.y <= transform.position.y + transform.localScale.y;
    }
}
