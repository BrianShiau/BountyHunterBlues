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
    private Command meleeAttack;

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
        aim = new AimCommand(new Vector2(0, 1));
        disableAim = new DisableAimCommand();
        interact = new InteractCommand();
        attack = new AttackCommand();
        meleeAttack = new MeleeAttackCommand();
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
        if (!player.GetComponent<PlayerActor>().inTacticalMode)
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

            

            if (Input.GetMouseButton(0) && meleeAttackInputDelay < 0)
            {
                // Aim and knife in the same frame
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
                Vector2 aimVector = player.transform.InverseTransformPoint(worldPoint); // implied "minus player position wrt its coordinate frame" (which is zero)
                aimVector.Normalize();
                aim.updateCommandData(aimVector);
                nextCommands.AddLast(aim);
                nextCommands.AddLast(meleeAttack);
                nextCommands.AddLast(disableAim);
                meleeAttackInputDelay = 1;
            }

            if (Input.GetMouseButtonUp(0))
            {
                meleeAttackInputDelay = 0;
            }

            if (Input.GetMouseButton(1) && attackInputDelay < 0) // pressing down right mouse button
            {
                // Aim and shoot in same frame
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
                Vector2 aimVector = player.transform.InverseTransformPoint(worldPoint); // implied "minus player position wrt its coordinate frame" (which is zero)
                aimVector.Normalize();
                aim.updateCommandData(aimVector);
                nextCommands.AddLast(aim);
                nextCommands.AddLast(attack);
                nextCommands.AddLast(disableAim);
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
