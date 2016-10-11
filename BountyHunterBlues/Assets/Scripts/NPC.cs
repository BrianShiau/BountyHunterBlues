using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC : NPCActor, Interactable, Dialogue {
	//we should really put this disable in player
	private PlayerActor player;
	private GameObject chatPanel;
	private GameObject chatImage;

	public Sprite npcImage;

	private string[] strings;

	public int NPCNumber;
	public bool destroyAfterPlay;

	public int currentLine;
	private bool typing;
	Coroutine typingRoutine;

	//have to call start twice for some reason. dont do it the second time.
	private bool startedAlready = false;

	// Use this for initialization
	public override void Start () {
        base.Start();
		if (startedAlready)
			return;
		startedAlready = true;

		if(destroyAfterPlay && PlayerActor.deaths!=0){
			Destroy(this.gameObject);
		}

		player = GameObject.FindObjectOfType<PlayerActor>();
		chatPanel = GameObject.FindGameObjectWithTag ("ChatPanel");
		chatImage = GameObject.FindGameObjectWithTag ("ChatImage");

		typing = false;
		typingRoutine = null;

		switch (NPCNumber){
		case 0: 
			//Opening Tutorial
			strings = new string[] {
				"Oh. I guess I better... get off this ship",
				"Thanks, you hunk of junk.",
			};
			break;
		case 1: 
			//Ending Tutorial
			strings = new string[] {
				""
			};
			break;
		case 10: 
			//Rest Area Big Bad NPC
			strings = new string[] {
				"A lot more ships leaving than coming these days... Things have changed since you’ve last been here. ",
				"Though you’ve surely seen this before. Happening everywhere these days.",
				"But don’t worry, it’s the riff-raff that are getting jettisoned. [chuckles] Consider it a long-overdue spring cleaning.",
				"You’d best watch yourself, lest you be sent the same way."
			};
			break;
		case 20: 
			//Opening Level 1
			strings = new string[] {
				"Glad to see he’s creepy as ever. How the hell did he know I was gonna be here?",
				"Okay, the door out of the loading bay is probably locked.",
				"I wonder if they’re still using those rolly things..."
			};
			break;
		case 21: 
			//Ending Level 1
			strings = new string[] {
				"Ending Level 1 Text",
			};
			break;
		default:
			break;
		}
	}

	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public void runInteraction()
	{
		runDialogue ();
	}

	public void runDialogue()
	{
		if (strings.Length == 0)
			return;
		if (currentLine == 0) {
			chatPanel.GetComponent<Image> ().enabled = true;
			chatPanel.GetComponentInChildren<Text> ().enabled = true;
			chatImage.GetComponent<Image> ().enabled = true;
			chatImage.GetComponent<Image> ().sprite = npcImage;
			player.inTacticalMode = true;
		}
		if (currentLine < strings.Length) {
			if (!typing) {
				typing = true;
				typingRoutine = StartCoroutine (TypeText (strings [currentLine]));
			} else {
				StopCoroutine (typingRoutine);
				typing = false;
				chatPanel.GetComponentInChildren<Text> ().text = strings [currentLine];
				currentLine++;
			}
		} else {
			if (destroyAfterPlay && this.gameObject) {
				Text tutorialText = GetComponentInChildren<Text> ();
				if (tutorialText) {
					tutorialText.text = "Use WASD to move";
				}
				//Destroy (this.gameObject);
				this.tag = "Untagged";
			} else {
				currentLine = 0;
			}
			chatPanel.GetComponent<Image> ().enabled = false;
			chatPanel.GetComponentInChildren<Text> ().enabled = false;
			chatImage.GetComponent<Image> ().enabled = false;
			player.inTacticalMode = false;
		}
	}

	IEnumerator TypeText (string message) 
	{
		chatPanel.GetComponentInChildren<Text> ().text = "";
		foreach (char letter in message.ToCharArray()) 
		{
			chatPanel.GetComponentInChildren<Text> ().text += letter;
			//play typing sound here if there is one
			yield return new WaitForSeconds (.02f);
		}
		typing = false;
		currentLine++;
	}
}
