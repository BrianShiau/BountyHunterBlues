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

    void FixedUpdate()
    {
        LinkedList<Command> nextCommands = handleInput();
        foreach(Command nextCommand in nextCommands)
            nextCommand.execute(player.GetComponent<PlayerActor>());
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
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
            Vector2 aimVector = player.transform.InverseTransformPoint(worldPoint); // implied "minus player position wrt its coordinate frame" (which is zero)
            aimVector.Normalize();
            aim.updateCommandData(aimVector);
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
