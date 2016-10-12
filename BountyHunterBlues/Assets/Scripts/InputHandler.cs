using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {

    public GameObject player;
    // Here lists all the controls available
    private Command move;
    private Command stopMove;
    private Command interact;
    private Command rangedAttack;
    private Command meleeAttack;
    private Command look;

    private float attackInputDelay;
    private float meleeAttackInputDelay;
    private float interactInputDelay;

    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerActor>().gameObject;
        attackInputDelay = 0;
        meleeAttackInputDelay = 0;
        interactInputDelay = 0;
        move = new MoveCommand(new Vector2(0, 0));
        stopMove = new MoveStopCommand();
        interact = new InteractCommand();
        rangedAttack = new RangedAttackCommand();
        meleeAttack = new MeleeAttackCommand();
        look = new LookCommand();
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
		if (!player.GetComponent<PlayerActor>().InTacticalMode())
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
                // issue stopMove command if no WASD input given this frame
                nextCommands.AddLast(stopMove);

            

            if (Input.GetMouseButton(0) && meleeAttackInputDelay < 0)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
                look.updateCommandData(worldPoint);
                nextCommands.AddLast(look);
                nextCommands.AddLast(meleeAttack);
                meleeAttackInputDelay = 1;
            }

            if (Input.GetMouseButtonUp(0))
            {
                meleeAttackInputDelay = 0;
            }

            if (Input.GetMouseButton(1) && attackInputDelay < 0) // pressing down right mouse button
            {
                nextCommands.AddLast(rangedAttack);
                attackInputDelay = 1;
            }

            if (Input.GetMouseButtonUp(1))
                attackInputDelay = 0;

            
        }
         
        if ((Input.GetKey(KeyCode.Space) && interactInputDelay < 0) 
            ||  (Input.GetKey(KeyCode.E) && interactInputDelay < 0))
        {
			interactInputDelay = 1;
            interact = new InteractCommand();
            nextCommands.AddLast(interact);
        }

		if (Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp (KeyCode.E)) {
			interactInputDelay = 0;
		}

        

        // Need to implement Q special ability

        // input delay timers
        interactInputDelay -= Time.deltaTime;
        attackInputDelay -= Time.deltaTime;
        meleeAttackInputDelay -= Time.deltaTime;

        return nextCommands;
    }

	
}
