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

	void neutral_state(GameObject target){

	}
	void alert_state(GameObject target){

	}
	void aggresive_state(GameObject target){

	}
	void confused_state(GameObject target){

	}

	State get_state(GameObject target){
		neutral_state(target);
		alert_state(target);
		aggresive_state(target);
		confused_state(target);

		return state;
	}
}
