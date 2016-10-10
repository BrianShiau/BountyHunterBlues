using UnityEngine;
using System.Collections;

public class NeutralDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){}

	public string name(){
		return "NEUTRAL";
	}
}

public class AlertDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){}

	public string name(){
		return "ALERT";
	}
}

public class AggresiveDog: AIState {
	public void on_enter(){}
	
	public void on_exit(){}

	public void execute(){}

	public string name(){
		return "AGGRESIVE";
	}
}
