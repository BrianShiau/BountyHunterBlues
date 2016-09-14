using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {

    public GameObject player;
    public bool tacticalMode;
    // Here lists all the controls available
    private Command move;

    private Command aim;
    private Command disableAim;

    private Command interact;
    private Command attack;

    private float attackInputDelay;
    private float interactInputDelay;

    void Start()
    {
        attackInputDelay = 0;
        interactInputDelay = 0;
        move = new MoveCommand(new Vector2(0, 0));
        aim = new AimCommand(new Vector2(0, 1));
        disableAim = new DisableAimCommand();
        interact = new InteractCommand();
        attack = new AttackCommand();
        tacticalMode = false;
    }

    void FixedUpdate()
    {
        LinkedList<Command> nextCommands = handleInput();
        foreach(Command nextCommand in nextCommands)
            nextCommand.execute(player.GetComponent<PlayerActor>());
    }

    private LinkedList<Command> handleInput()
    {
        LinkedList<Command> nextCommands = new LinkedList<Command>();
        bool movement = false;
        if (!tacticalMode)
        { 
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
            else
                // reinit isMoving to false when no move command is issued
                player.GetComponent<GameActor>().isMoving = false;


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

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) && attackInputDelay < 0) // pressing down left mouse button
            {
                attack = new AttackCommand();
                nextCommands.AddLast(attack);
                attackInputDelay = 1;
            }

            if (Input.GetMouseButtonUp(0))
                attackInputDelay = 0;
        } 
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space) && interactInputDelay < 0)
        {
            interact = new InteractCommand();
            nextCommands.AddLast(interact);
            interactInputDelay = 1;
        }

        if (Input.GetKeyUp(KeyCode.Space))
            interactInputDelay = 0;

        // Need to implement Q special ability
        interactInputDelay -= Time.deltaTime;
        attackInputDelay -= Time.deltaTime;

        return nextCommands;
    }

	
}
