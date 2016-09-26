using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC : MonoBehaviour, Interactable {
	//we should really put this disable in player
	private PlayerActor player;
	private GameObject chatPanel;

	private string[] strings;

	public int NPCNumber;
	public bool destroyAfterPlay;

	public int currentLine;

	// Use this for initialization
	public void Start () {
		if(destroyAfterPlay && PlayerActor.deaths!=0){
			Destroy(this.gameObject);
		}

		player = GameObject.FindObjectOfType<PlayerActor>();
		chatPanel = GameObject.FindGameObjectWithTag ("ChatPanel");

		switch (NPCNumber){
		case 0: 
			//Opening Tutorial
			strings = new string[] {"Oh. I guess I better…",
				"Oh. I guess I better… get off this ship",
				"Thanks, you hunk of junk.",
			};
			break;
		case 1: 
			//Ending Tutorial
			strings = new string[] {""
			};
			break;
		case 10: 
			//Rest Area Big Bad NPC
			strings = new string[] {"A lot more ships leaving than coming these days…",
				"A lot more ships leaving than coming these days… Things have changed since you’ve last been here. ",
				"Though you’ve surely seen this before.",
				"Though you’ve surely seen this before. Happening everywhere these days.",
				"But don’t worry, it’s the riff-raff that are getting jettisoned.",
				"But don’t worry, it’s the riff-raff that are getting jettisoned. [chuckles] Consider it a long-overdue spring cleaning.",
				"You’d best watch yourself, lest you be sent the same way."
			};
			break;
		case 20: 
			//Opening Level 1
			strings = new string[] {"Glad to see he’s creepy as ever. How the hell did he know I was gonna be here?",
				"OK, the door out of the loading bay is probably locked.",
				"I wonder if they’re still using those rolly things…"
			};
			break;
		case 21: 
			//Ending Level 1
			strings = new string[] {"Ending Level 1 Text",
			};
			break;
		default:
			break;
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public void runInteraction()
	{
		if (strings.Length == 0)
			return;
		if (currentLine == 0) {
			chatPanel.GetComponent<Image> ().enabled = true;
			chatPanel.GetComponentInChildren<Text> ().text = strings [0];
			chatPanel.GetComponentInChildren<Text> ().enabled = true;
			currentLine++;
			player.inTacticalMode = true;
		} else if (currentLine < strings.Length) {
			chatPanel.GetComponentInChildren<Text> ().text = strings [currentLine];
			currentLine++;
		} else {
			chatPanel.GetComponent<Image> ().enabled = false;
			chatPanel.GetComponentInChildren<Text> ().enabled = false;
			currentLine = 0;
			player.inTacticalMode = false;
			if(destroyAfterPlay){
				Destroy(this.gameObject);
			}
		}
	}
}
