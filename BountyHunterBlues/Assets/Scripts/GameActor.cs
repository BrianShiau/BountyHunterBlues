using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    public abstract void rangedGunAttack();
    public abstract void loseHealth();

    public void move(Vector2 vel) // speed and direction
    {

    }

	
}
