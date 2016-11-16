using UnityEngine;
using System.Collections;

public class TacticalTerminal : MonoBehaviour, Interactable {

    private PlayerActor player;
    private camera_lerp lerpCamera;
	// Use this for initialization
	void Start () {
        player = GameObject.FindObjectOfType<PlayerActor>();
        lerpCamera = GameObject.Find("0_Player").GetComponentInChildren<camera_lerp>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void runInteraction()
    {
        lerpCamera.set_lerp(true);
        lerpCamera.toggle_tactical_mode();
		player.SetTacticalMode(!player.InTacticalMode());
		if (player.InTacticalMode ()) {
			player.DisableGun ();
			player.DisableKnifeImage ();
		} else {
			player.EnableGun ();
			player.EnableKnifeImage ();
		}
    }

	public void EndInteraction(){
		runInteraction ();
	}

	public bool ForceInteraction(){
		return false;
	}
}
