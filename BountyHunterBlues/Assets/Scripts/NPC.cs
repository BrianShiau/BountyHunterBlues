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

		if(destroyAfterPlay && PlayerActor.Deaths()!=0){
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
				"Of all the backwater space stations he could have sent me to, it had to be the one where I grew up…",
				"I haven’t been there in years and I can’t say I’ve missed the place.",
				"I’ll try to make it quick; take out my target and get out as quick as possible.",
				"Bounty better be worth it.",
				"He could’ve given me more to go on, though. “CEO of Eva Corporation,” what am I supposed to do with that?",
				"Ah, are we docking? Better get out quick.",
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
				"My God! Is that you? I never thought I’d see you again!",
				"Considering how we broke up, I would’ve been fine with that.",
				"What are you doing here? You shouldn’t have come back.",
			};
			break;
		case 11: 
			//TV 1
			strings = new string[] {
                "...soaring cost of living. In spite of this, EvaCorp continues to buy up properties,",
                "with plans to start three separate construction projects in the next year...",
			};
			break;
		case 12: 
			//TV 2
			strings = new string[] {
                "…we at EvaCorp promise to revitalize YOUR community.",
                "We pride ourselves on our commitment to you and making sure that YOU",
                "can make the best of your future, with EvaCorp products and services...",
            };
			break;
		case 20: 
			//Opening Level 1
			strings = new string[] {
				"Soon as I get home I run into my ex. Wonderful.",
				"Let's just... move on.",
				"Okay, the door out of the loading bay is probably locked.",
				"I wonder if they’re still using those rolly things to control the doors...",
			};
			break;
		case 21: 
			//Ending Level 1
			strings = new string[] {
				"Ending Level 1 Text",
			};
			break;
		default:
			strings = new string[] { };
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
			player.SetTacticalMode(true);
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
			player.SetTacticalMode(false);
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
