using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour {

    public PlayerActor player;
    // Here lists all the controls available
    private Command move;
    private Command stopMove;
    private Command interact;
	private Command endInteract;
    private Command rangedAttack;
    private Command meleeAttack;
    private Command aim;
    private Command disableAim;
    private Command look;

    private float attackInputDelay;
    private float meleeAttackInputDelay;
    private float interactInputDelay;
	private bool isPaused;
    private bool isAiming;
    private bool meleeStoppedAiming;
	private float pauseInputDelay;
	private GameObject menu;
	private bool inFirstHitMenu;

    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerActor>();
        attackInputDelay = 0;
        meleeAttackInputDelay = 0;
        interactInputDelay = 0;
		pauseInputDelay = -1;
		isPaused = false;
        isAiming = false;
        meleeStoppedAiming = false;
        move = new MoveCommand(new Vector2(0, 0));
        stopMove = new MoveStopCommand();
        interact = new InteractCommand();
		endInteract = new EndInteractCommand ();
        rangedAttack = new RangedAttackCommand();
        meleeAttack = new MeleeAttackCommand();
        aim = new AimCommand();
        disableAim = new ReleaseAimCommand();
        look = new LookCommand();
		menu = GameObject.FindGameObjectWithTag ("PauseMenu");
		menu.gameObject.SetActive (false);
		menu.transform.FindChild ("BlackFade").GetComponent<Image> ().enabled = true;
		inFirstHitMenu = false;
    }

    void FixedUpdate()
    {
        LinkedList<Command> nextCommands = handleInput();
        foreach(Command nextCommand in nextCommands)
            nextCommand.execute(player);
    }

	void Update(){
		if (!player.InDialogueMode() && !player.InTacticalMode() && Input.GetKey (KeyCode.Escape) && pauseInputDelay < 0){
			Pause ();
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			pauseInputDelay = -1;
		}

		//if paused by non input handler
		if (Time.timeScale == 0 && !isPaused) {
			if (player.InTacticalMode () || player.InDialogueMode ()) {
				if (interactInputDelay <= 0) {
					if (!Input.GetKey (KeyCode.Escape) && Input.anyKey) {
						interactInputDelay = 1;
						interact.execute (player);
					}
				}

				if (!Input.anyKey)
					interactInputDelay = 0;

				interactInputDelay -= Time.unscaledDeltaTime;
			}
		}

		if (inFirstHitMenu) {
			if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.E)) {
				EndFirstHitMenu ();
			}
		}
	}

	public void Pause(){
		StopCoroutine ("PauseInputDelay");
		isPaused = !isPaused;
		if (isPaused) {
			Time.timeScale = 0f;
			menu.gameObject.SetActive (true);

		} else {
			if (!inFirstHitMenu) {
				Time.timeScale = 1;
			}
			menu.gameObject.SetActive (false);
		}
		pauseInputDelay = 1;
		StartCoroutine ("PauseInputDelay");
	}

	public void PauseFromMenu(){
		Pause ();
		pauseInputDelay = -1;
	}

	public void StartFirstHitMenu(){
		inFirstHitMenu = true;
		Time.timeScale = 0f;
		GameObject firstHitMenu = GameObject.FindGameObjectWithTag ("FirstHitMenu");
		firstHitMenu.SetActive(true);
		firstHitMenu.transform.FindChild ("BlackFade").GetComponent<Image> ().enabled = true;
		firstHitMenu.transform.FindChild ("FirstHitText").GetComponent<Text> ().enabled = true;
	}

	public void EndFirstHitMenu(){
		inFirstHitMenu = false;
		Time.timeScale = 1f;
		GameObject firstHitMenu = GameObject.FindGameObjectWithTag ("FirstHitMenu");
		firstHitMenu.SetActive(false);
		firstHitMenu.transform.FindChild ("BlackFade").GetComponent<Image> ().enabled = false;
		firstHitMenu.transform.FindChild ("FirstHitText").GetComponent<Text> ().enabled = false;
	}

    private LinkedList<Command> handleInput()
    {
        LinkedList<Command> nextCommands = new LinkedList<Command>();
        bool movement = false;

        if(player.InTacticalMode() || player.InDialogueMode())
        {
            if (interactInputDelay <= 0)
            {
                if (!Input.GetKey(KeyCode.Escape) && Input.anyKey)
                {
                    interactInputDelay = 1;
                    nextCommands.AddLast(interact);
                }    
            }

            if(!Input.anyKey)
                interactInputDelay = 0;

			if (Input.GetKey (KeyCode.Escape)) {
				nextCommands.AddLast(endInteract);
				pauseInputDelay = 1;
			}
        }
		else
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

                if(player.NEW_GUN_MODE && isAiming)
                {
                    isAiming = false;
                    nextCommands.AddLast(disableAim);
                    meleeStoppedAiming = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                meleeAttackInputDelay = 0;
            }

            if (player.NEW_GUN_MODE)
            {
                if (meleeStoppedAiming)
                {
                    if (Input.GetMouseButtonUp(1))
                        meleeStoppedAiming = false;
                    
                }
                else
                {
                    if (Input.GetMouseButton(1) && attackInputDelay < 0) // pressing down right mouse button
                    {
                        isAiming = true;
                        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse point in world space
                        aim.updateCommandData(worldPoint);
                        nextCommands.AddLast(aim);

                    }
                    else if (isAiming) // no longer pressing button, but was aiming last frame
                    {
                        isAiming = false;
                        nextCommands.AddLast(rangedAttack);
                        nextCommands.AddLast(disableAim);

                    }
                }
                
            }

            else
            {

                if (Input.GetMouseButton(1) && attackInputDelay < 0) // pressing down right mouse button
                {
                    nextCommands.AddLast(rangedAttack);
                    attackInputDelay = 1;
                }

                if (Input.GetMouseButtonUp(1))
                    attackInputDelay = 0;
            }

            if ((Input.GetKey(KeyCode.Space) && interactInputDelay < 0)
                || (Input.GetKey(KeyCode.E) && interactInputDelay < 0))
            {
                interactInputDelay = 1;
                nextCommands.AddLast(interact);
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.E))
            {
                interactInputDelay = 0;
            }
        }
         
        

        // Need to implement Q special ability

        // input delay timers
        interactInputDelay -= Time.deltaTime;
        attackInputDelay -= Time.deltaTime;
        meleeAttackInputDelay -= Time.deltaTime;

        return nextCommands;
    }

	private IEnumerator PauseInputDelay(){
		yield return StartCoroutine(Utility.WaitForRealTime (1));
		pauseInputDelay = -1;
	}
}
