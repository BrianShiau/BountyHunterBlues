using UnityEngine;
using System.Collections;

public interface AIState {
	
	void on_enter();
	void on_exit();
	void execute();
	string name();
    State get_state();
}