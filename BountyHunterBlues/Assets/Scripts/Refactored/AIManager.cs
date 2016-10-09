using UnityEngine;
using System.Collections;

public class AIManager{

	private GameObject player_reference;
	private State state;

	AIManager(GameObject obj){
		player_reference = obj;
	}
	
	private enum State{
	    NEUTRAL, ALERT, AGGRESIVE, CONFUSED
	}

	void neutral_state(){

	}
	void alert_state(){

	}
	void aggresive_state(){

	}
	void confused_state(){
		
	}

	State get_state(){
		neutral_state();
		alert_state();
		aggresive_state();
		confused_state();

		return state;
	}
}
