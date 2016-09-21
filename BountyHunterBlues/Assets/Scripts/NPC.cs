using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC : MonoBehaviour, Interactable {
	//we should really put this disable in player
	private InputHandler inputHandler;
	private GameObject chatPanel;

	public int currentLine;

	// Use this for initialization
	void Start () {
		inputHandler = GameObject.FindObjectOfType<InputHandler>();
		chatPanel = GameObject.FindGameObjectWithTag ("ChatPanel");
		currentLine = 0;
	}

	// Update is called once per frame
	void Update () {

	}

	public void runInteraction()
	{
		if (currentLine == 0) {
			chatPanel.GetComponent<Image> ().enabled = true;
			chatPanel.GetComponentInChildren<Text> ().text = "A lot more ships leaving than coming these days…";
			chatPanel.GetComponentInChildren<Text> ().enabled = true;
			currentLine++;
			inputHandler.tacticalMode = true;
		} else if(currentLine == 1){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "A lot more ships leaving than coming these days… Things have changed since you’ve last been here. ";
		} else if(currentLine == 2){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "Though you’ve surely seen this before.";
		} else if(currentLine == 3){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "Though you’ve surely seen this before. Happening everywhere these days.";
		} else if(currentLine == 4){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "But don’t worry, it’s the riff-raff that are getting jettisoned.";
		} else if(currentLine == 5){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "But don’t worry, it’s the riff-raff that are getting jettisoned. [chuckles] Consider it a long-overdue spring cleaning.";
		} else if(currentLine == 6){
			currentLine++;
			chatPanel.GetComponentInChildren<Text> ().text = "You’d best watch yourself, lest you be sent the same way.";
		} else {
			chatPanel.GetComponent<Image>().enabled = false;
			chatPanel.GetComponentInChildren<Text> ().enabled = false;
			currentLine = 0;
			inputHandler.tacticalMode = false;
		}
	}
}
