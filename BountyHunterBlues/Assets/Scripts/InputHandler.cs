using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

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

    public Command handleInput()
    {

        // basing WASD on +x-axis, +z-axis, -x-axis, -z-axis respectively
        if (Input.GetKeyDown(KeyCode.W))
        {
            move.updateCommandData(new Vector2(1, 0));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            move.updateCommandData(new Vector2(0, 1));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            move.updateCommandData(new Vector2(-1, 0));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            move.updateCommandData(new Vector2(0, -1));
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
