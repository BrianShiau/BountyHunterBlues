using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class PlayerActor : GameActor
{

    public override void Start()
    {
        base.Start();
        healthPool = 3;
    }

    public override void attack()
    {
        if (isAiming)
        {
            Debug.Log("Player shoots");
            if (aimTarget != null && Vector3.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
            {
                aimTarget.takeDamage();
                if (!aimTarget.isAlive())
                    aimTarget = null;
            }
        }
        else
        {
            Debug.Log("attack happening on left click");
            if (lookTarget != null && Vector3.Distance(lookTarget.transform.position, transform.position) <= meleeDistance)
            {
                lookTarget.takeDamage();
                if (!lookTarget.isAlive())
                    lookTarget = null;
            }

        }
        
    }

    public override void interact()
    {

    }

	public override void die()
	{
		// reset the game here for now
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
