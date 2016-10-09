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

	State get_state(){
		return state;
	}
}
