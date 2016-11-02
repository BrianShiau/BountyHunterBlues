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

	public bool pauseTime;

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
		//Tutorial
		case 0: 
			//Opening Tutorial
			strings = new string[] {
				"Of all the backwater space stations I could be sent to, it had to be the one where I grew up…",
				" I haven’t missed this place.",
				"I’ll try to make this quick. Take out this “CEO of Eva Corp” or whatever and get the hell out of here.",
				"Ah, are we docking? Guess I better get going.",
			};
			break;
		case 1: 
			//Pickup Gun
			strings = new string[] {
				"Oh cool, a gun. This’ll be useful.",
				"Looks like I have to hold the trigger down to make it more accurate.",
				"And once I let go it’ll fire… might attract some unwanted attention.",
			};
			break;
		case 2: 
			//Ending Tutorial
			strings = new string[] {
				""
			};
			break;

		//Airlock Rest Area
		case 10: 
			//Rest Area Big Bad NPC
			strings = new string[] {
				"My God, is that you? What are you doing here?",
				"I never thought I’d see you again.",
				"I would’ve been fine with that, you know… considering how we broke up.",
				"Looks like my shuttle is here.",
			};
			break;
		case 11: 
			//TV 1
			strings = new string[] {
                "...CEO has come under fire for what some consider unscrupulous business practices.",
                "In spite of this, EvaCorp continues to buy up properties,",
                "with plans to start three separate luxury construction projects in the next year...",
			};
			break;
		case 12: 
			//TV 2
			strings = new string[] {
                "…we at EvaCorp promise to revitalize YOUR community.",
                "We pride ourselves on our commitment to you and making sure that YOU can make the best of your future,",
                 "with EvaCorp products and services...",
            };
			break;

		//Level 1
		case 20: 
			//Opening Level 1
			strings = new string[] {
				"Soon as I get home I run into my ex. Wonderful.",
				"...",
				"The door out of the loading bay is probably locked.",
				"I wonder if they’re still using those rolly-robot things to control the doors...",
			};
			break;
		case 21: 
			//Ending Door Text
			strings = new string[] {
				"There must be a way to get this door open...",
			};
			break;
		case 22: 
			//Ending Level 1
			strings = new string[] {
				"Ending Level 1 Text",
			};
			break;

		//Bar Rest Area
        case 30:
            //bartender
            strings = new string[] {
                "Looks like a certain cowgirl’s back.",
				"This ain’t the same place it used to be.",
				"Things ain’t been so good for us.",
                "Lots of folks have been leaving, running off. Disappearing.",
                "Can’t say I blame ‘em. We’re all old now, worn and ragged.",
				"Some of us can’t leave though, even if we wanted to.",
				"Roots go deep, and pockets...not deep enough.",
				"No hope in a place like this.",
				"But I stay, serving drinks to the soon to be ghosts like me.",
				"I s'pose it ain’t so bad.",

            };
                break;
        case 31:
            //dock worker
            strings = new string[] {
				"My brother disappeared two weeks ago.",
				"Vanished off the face of the station.",
				"Police say he just up and ran off...but that’s not like him.",

            };
                break;
        case 32:
            //waitress
            strings = new string[] {
                "Would you believe it? My landlord ran off last month and some big company bought the building.",
                "They say they’re gonna tear it down in a few months. Wanna build some new high rises for the rich folks.",
                "Was the only place I could afford, and now what?",

            };
                break;
        case 33:
            //hooker
            strings = new string[] {
				"See that gentleman passed out in the corner over there? Hear he’s getting divorced.",
                "Yeah, their two girls disappeared, just poof...poor things.",
				"His partner’s not taking it well, and neither is he as you might be able to tell",
                "...",
                "Hey, you wanna have a good time, you let me know, alright?",

            };
                break;
        case 34:
            //passed out guy
            strings = new string[] {
                "He's out cold.",

            };
                break;
            default:
			strings = new string[] { };
			break;

		//Level 2

		//Warehouse Rest Area

		//Level 3

		//Levelless
		/*case 22: 
		//Hit Mecahinc
		strings = new string[] {
            "When you get hit you lose a health point.",
            "But you will also go invisable for 2 seconds",
            "This gives you a chance to hide from enemy site",
        };
		break;*/
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
			player.SetDialogueMode(true);
			player.DisableGunImage ();
			if (pauseTime)
				Time.timeScale = 0;
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
				Destroy (this.gameObject);
				//this.tag = "Untagged";
			} else {
				currentLine = 0;
			}
			chatPanel.GetComponent<Image> ().enabled = false;
			chatPanel.GetComponentInChildren<Text> ().enabled = false;
			chatImage.GetComponent<Image> ().enabled = false;
			player.SetDialogueMode(false);
			player.EnableGunImage ();
			if (pauseTime)
				Time.timeScale = 1;
		}
	}

	IEnumerator TypeText (string message) 
	{
		chatPanel.GetComponentInChildren<Text> ().text = "";
		foreach (char letter in message.ToCharArray()) 
		{
			chatPanel.GetComponentInChildren<Text> ().text += letter;
			//play typing sound here if there is one
			if(pauseTime)
				yield return Utility.WaitForRealTime(.001f);
			else
				yield return new WaitForSeconds(.001f);
		}
		typing = false;
		currentLine++;
	}
}
