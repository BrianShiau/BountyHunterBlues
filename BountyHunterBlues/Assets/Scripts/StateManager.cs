using UnityEngine;
using System.Collections;

public enum State{
	NEUTRAL, ALERT, AGGRESIVE
}

public class StateManager{

	private float state_time_threshold;
	private float confused_time_threshold;
	private float state_time_up;
	private float state_time_down;
	private float confused_time_down;
	private bool is_attacking;
	private bool confused;
	private State state;
	private EnemyActor enemy;
	private Vector2 last_known_position;

	public StateManager(float state_time_threshold, float confused_time_threshold){
		this.state_time_threshold = state_time_threshold;
		this.confused_time_threshold = confused_time_threshold;
		this.enemy = enemy;
		state_time_up = 0;
		state_time_down = 0;
		confused_time_down = 0;
		confused = false;
		is_attacking = false;
	}

	private void set_state(State state){
		this.state = state;
	}

	private void neutral_state(GameActor target, bool sound_detected){
		if(sound_detected){
			set_state(State.ALERT);
		}
		if(target != null){
			state_time_up += Time.deltaTime;
			if(state_time_up >= state_time_threshold){
				set_state(State.ALERT);
				state_time_up = 0;
			}
		}
		else if(target == null){
			state_time_up = 0;
		}
	}

	private void alert_state(GameActor target, bool sound_detected, bool is_alert){
		if(target != null){
			state_time_down = 0;
			state_time_up += Time.deltaTime;
			if(state_time_up >= state_time_threshold){
				set_state(State.AGGRESIVE);
				state_time_up = 0;
			}
		}
		else if(is_alert){
			return;
		}
		else{
			state_time_up = 0;
			state_time_down += Time.deltaTime;
			if(state_time_down >= state_time_threshold){
				set_state(State.NEUTRAL);
				state_time_down = 0;
			}
		}
	}

	private void aggresive_state(GameActor target){
		if(confused){
			state_time_up = 0;
			confused_time_down += Time.deltaTime;
			if(confused_time_down >= confused_time_threshold){
				set_state(State.ALERT);
				confused_time_down = 0;
				confused = false;
			}
		}
		else if(is_attacking){
			state_time_down = 0;
		}	
		else if(target == null){
			state_time_up = 0;
			state_time_down += Time.deltaTime;
			if(state_time_down >= state_time_threshold){
				set_state(State.ALERT);
				state_time_down = 0;
			}
		}
		else{
			state_time_down = 0;
		}
	}

	public void update_state(bool attacking, bool cloaked, GameActor target, bool sound_detected, bool is_alert){
		if(state == State.NEUTRAL) 	 neutral_state(target, sound_detected);
		if(state == State.ALERT) 	 alert_state(target, sound_detected, is_alert);
		if(state == State.AGGRESIVE) aggresive_state(target);
		if(cloaked)		 confused = true;
		is_attacking = attacking;
		//Debug.Log(is_attacking);
	}

	public State get_state(){
		return state;
	}
}
