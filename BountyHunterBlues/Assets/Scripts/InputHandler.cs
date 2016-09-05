using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

    public GameObject player;

    // Here lists all the controls available
    private Command move;

    private Command aim;
    private Command disableAim;

    private Command interact;
    private Command attack;

    void Start()
    {
        move = new MoveCommand(new Vector2(0, 0));
        aim = new AimCommand(new Vector2(0, 1));
        disableAim = new DisableAimCommand();
        interact = new InteractCommand();
        attack = new AttackCommand();
    }

    void Update()
    {
        Command nextCommand = handleInput();
        if (nextCommand != null)
        {
            nextCommand.execute(player.GetComponent<PlayerActor>());
        }

        // this part will go in the GameLogicManager
        GameObject[] actorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        foreach(GameObject actorObj in actorObjects)
        {
            actorObj.GetComponent<GameActor>().acquireLookTarget();
        }
    }

    public Command handleInput()
    {
        bool movement = false;
        Vector2 movementVector = new Vector2(0, 0);
        // basing WASD on +x-axis, +z-axis, -x-axis, -z-axis respectively
        if (Input.GetKey(KeyCode.W))
        {
            movementVector = movementVector + new Vector2(1, 0);
            movement = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector = movementVector + new Vector2(0, 1);
            movement = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector = movementVector + new Vector2(-1, 0);
            movement = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector = movementVector + new Vector2(0, -1);
            movement = true;
        }

        if (movement)
        {
            movementVector.Normalize();
            move.updateCommandData(movementVector);
            return move;
        }
            

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1)) // pressed or pressing down right mouse button
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
            
            Vector3 projectedPoint = Vector3.ProjectOnPlane(worldPoint, new Vector3(0, 1, 0)); // need to project that vector onto xz plane
            Vector3 projectedPlayerPoint = Vector3.ProjectOnPlane(player.transform.position, new Vector3(0, 1, 0)); // project player point onto xz plane (just in case. player should be on xz plane)
            Vector3 aimVector = projectedPoint - projectedPlayerPoint; // get the vector pointing to worldPoint from the player pos
            aimVector.Normalize();
            aim.updateCommandData(new Vector2(aimVector.x, aimVector.z));
            return aim;
        }
        if (Input.GetMouseButtonUp(1)) // releasing right mouse button
        {
            disableAim = new DisableAimCommand();
            return disableAim;
        }

        if (Input.GetMouseButtonDown(0)) // pressing down left mouse button
        {
            attack = new AttackCommand();
            return attack;
        } 
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact = new InteractCommand();
            return interact;
        }
        
        // Need to implement Q special ability

        return null;
    }

	
}
