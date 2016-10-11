using UnityEngine;
using System.Collections;


public class StateManager{

	private float state_time_threshold;
	private float state_time_up;
	private float state_time_down;
	private State state;
	private EnemyActor enemy;

	public StateManager(float state_time_threshold, EnemyActor enemy){
		this.state_time_threshold = state_time_threshold;
		this.enemy = enemy;
		state_time_up = 0;
		state_time_down = 0;
	}

	public enum State{
    	NEUTRAL, ALERT, AGGRESIVE
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

	private void alert_state(GameActor target){
		if(target != null){
			state_time_down = 0;
			state_time_up += Time.deltaTime;
			if(state_time_up >= state_time_threshold){
				//set_state(State.AGGRESIVE);
				state_time_up = 0;
			}
		}
		else{
			state_time_up = 0;
			state_time_down += Time.deltaTime;
			if(state_time_down >= state_time_threshold){
				enemy.set_shortest_path_calculated(false);
				set_state(State.NEUTRAL);
				state_time_down = 0;
			}
		}
	}

	private void aggresive_state(GameActor target){
		if(target == null){
			state_time_down += Time.deltaTime;
			if(state_time_down >= state_time_threshold){
				set_state(State.ALERT);
				state_time_down = 0;
			}
		}
	}


	public State get_state(GameActor target, bool sound_detected){
		if(state == State.NEUTRAL) 	 neutral_state(target, sound_detected);
		if(state == State.ALERT) 	 alert_state(target);
		if(state == State.AGGRESIVE) aggresive_state(target);

		return state;
	}
}
