using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

    // Here lists all the controls available
    private Command move;

    private Command aim;
    private Command lowerAim;

    private Command interact;
    private Command attack;

    public Command handleInput()
    {

        // basing WASD on +x-axis, +z-axis, -x-axis, -z-axis respectively
        if (Input.GetKeyDown(KeyCode.W))
        {
            move = new MoveCommand(new Vector2(1, 0));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            move = new MoveCommand(new Vector2(0, 1));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            move = new MoveCommand(new Vector2(-1, 0));
            return move;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            move = new MoveCommand(new Vector2(1, 0));
            return move;
        }
        if (Input.GetMouseButtonDown(0)) // pressing down right mouse button
        {
            aim = new AimCommand(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            return aim;
        }
        if (Input.GetMouseButtonUp(0)) // releasing right mouse button
        {
            lowerAim = new LowerAimCommand();
            return lowerAim;
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
