using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

    // Here lists all the controls available
    private Command key_W;
    private Command key_A;
    private Command key_S;
    private Command key_D;

    private Command RClick;
    private Command LClick;

    private Command key_Q;
    private Command key_E;

    public Command handleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            return key_W;
        if (Input.GetKeyDown(KeyCode.A))
            return key_A;
        if (Input.GetKeyDown(KeyCode.S))
            return key_S;
        if (Input.GetKeyDown(KeyCode.D))
            return key_D;

        if (Input.GetMouseButtonDown(0))
            return RClick;
        if (Input.GetMouseButtonDown(1))
            return LClick;

        if (Input.GetKeyDown(KeyCode.Q))
            return key_Q;
        if (Input.GetKeyDown(KeyCode.E))
            return key_E;

        return null;
    }

	
}
