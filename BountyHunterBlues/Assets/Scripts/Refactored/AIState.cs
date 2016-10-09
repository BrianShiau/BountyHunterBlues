using UnityEngine;
using System.Collections;

public interface AIState {
	
	void run_neutral_state();
	void run_alert_state();
	void run_aggresive_state();
	void run_confused_state();
}
