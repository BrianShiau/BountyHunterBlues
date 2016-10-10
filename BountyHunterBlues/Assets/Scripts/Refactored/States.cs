using UnityEngine;
using System.Collections;

public class NeutralDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){}

	public string name(){
		Debug.Log("NEULTRAL");
		return "NEUTRAL";
	}
}

public class AlertDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){}

	public string name(){
		Debug.Log("ALERT");
		return "ALERT";
	}
}

public class AggresiveDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){}

	public string name(){
		Debug.Log("AGGRESIVE");
		return "AGGRESIVE";
	}
}
