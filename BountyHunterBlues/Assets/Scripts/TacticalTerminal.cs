using UnityEngine;
using System.Collections;

public class TacticalTerminal : MonoBehaviour, Interactable {

    private InputHandler inputHandler;
    private Camera playerCamera;
    private Camera tacticalCamera;
	// Use this for initialization
	void Start () {
        inputHandler = GameObject.FindObjectOfType<InputHandler>();
        playerCamera = GameObject.Find("Player Character").GetComponentInChildren<Camera>();
        tacticalCamera = GameObject.Find("TacticalCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void runInteraction()
    {
        tacticalCamera.enabled = !tacticalCamera.enabled;
        playerCamera.enabled = !playerCamera.enabled;
        inputHandler.tacticalMode = !inputHandler.tacticalMode;


    }
}
