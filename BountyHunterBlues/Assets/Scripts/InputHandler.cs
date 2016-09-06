using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        LinkedList<Command> nextCommands = handleInput();
        foreach(Command nextCommand in nextCommands)
            nextCommand.execute(player.GetComponent<PlayerActor>());

        // this part will go in the GameLogicManager
        GameObject[] actorObjects = GameObject.FindGameObjectsWithTag("GameActor");
        foreach(GameObject actorObj in actorObjects)
        {
            actorObj.GetComponent<GameActor>().acquireLookTarget();
        }
    }

    public LinkedList<Command> handleInput()
    {
        LinkedList<Command> nextCommands = new LinkedList<Command>();
        bool movement = false;
        Vector2 movementVector = new Vector2(0, 0);
        // basing WASD on +x-axis, +y-axis, -x-axis, -y-axis respectively
        if (Input.GetKey(KeyCode.W))
        {
            movementVector.y++;
            movement = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector.x--;
            movement = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector.y--;
            movement = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector.x++;
            movement = true;
        }

        if (movement)
        {
            movementVector.Normalize();
            move.updateCommandData(movementVector);
            nextCommands.AddLast(move);
        }
            

        if (Input.GetMouseButton(1)) // pressed or pressing down right mouse button
        {
            // check for ranged attack
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
            
            Vector3 projectedPoint = Vector3.ProjectOnPlane(worldPoint, new Vector3(0, 0, 1)); // need to project that vector onto xy plane
            Vector3 aimVector = projectedPoint - player.transform.position; // get the vector pointing to worldPoint from the player pos
            aimVector.Normalize();
            Vector2 aim2DVector = new Vector2(aimVector.x, aimVector.y);
            aim.updateCommandData(aim2DVector);
            nextCommands.AddLast(aim);
        }
        else if (Input.GetMouseButtonUp(1)) // releasing right mouse button
        {
            disableAim = new DisableAimCommand();
            nextCommands.AddLast(disableAim);
        }

        if (Input.GetMouseButtonDown(0)) // pressing down left mouse button
        {
            attack = new AttackCommand();
            nextCommands.AddLast(attack);
        } 
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact = new InteractCommand();
            nextCommands.AddLast(interact);
        }
        
        // Need to implement Q special ability

        return nextCommands;
    }

	
}
