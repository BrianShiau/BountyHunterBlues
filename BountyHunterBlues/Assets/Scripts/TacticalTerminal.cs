using UnityEngine;
using System.Collections;

public class TacticalTerminal : MonoBehaviour, Interactable {

    private PlayerActor player;
    private Camera playerCamera;
    private Camera tacticalCamera;
	// Use this for initialization
	void Start () {
        player = GameObject.FindObjectOfType<PlayerActor>();
        playerCamera = GameObject.Find("0_Player").GetComponentInChildren<Camera>();
        tacticalCamera = GameObject.Find("0_TacticalCamera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void runInteraction()
    {
        tacticalCamera.enabled = !tacticalCamera.enabled;
        playerCamera.enabled = !playerCamera.enabled;
		player.SetTacticalMode(!player.InTacticalMode());
		if (player.InTacticalMode ()) {
			player.DisableGun ();
		} else {
			player.EnableGun ();
		}
    }
}
