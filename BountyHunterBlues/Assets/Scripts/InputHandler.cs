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
            

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) // pressed or pressing down right mouse button
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aim.updateCommandData(new Vector2(worldPoint.x, worldPoint.z));
            return aim;
        }
        if (Input.GetMouseButtonUp(0)) // releasing right mouse button
        {
            disableAim = new DisableAimCommand();
            return disableAim;
        }

        if (Input.GetMouseButtonDown(1)) // pressing down left mouse button
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
