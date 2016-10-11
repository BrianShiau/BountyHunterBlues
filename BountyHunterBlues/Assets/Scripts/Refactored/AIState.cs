using UnityEngine;
using System.Collections;

public interface AIState {
	
	void on_enter();
	void on_exit();
	void execute(GameObject self);
	string name();
}