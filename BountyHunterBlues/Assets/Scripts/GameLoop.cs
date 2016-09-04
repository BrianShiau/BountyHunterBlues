using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

	// Use this for initialization
	void Start () {
		load_scene()
		load_level()
		load_npcs()
		load_items()
		load_player()
	}
	
	// Update is called once per frame
	player_loop(){
		check_player_movement()
		check_player_action(){
			check_slash(){
				return result
			}
			check_fire(){
				return result
			}
			check_interact(){
				return result
			}
		}
	}

	npc_loop(){
		animate_AI(){
			animate_nearby_AI()
			run_AI_detection(){
				if_ray_hit(){
					run_AI_alert_sequence(){
						yellow(){
							investigate()
						}
						red(){
							alert_or_attack()
						}
					}
				}
				else(){
					green()
				}
			}
			run_AUDIO_detection(){
				if_sound_detected(){
					run_AI_alert_sequence(){
						yellow(){
							investigate()
						}
						red(){
							alert_or_attack()
						}
					}
				}
				else(){
					green()
				}
			}
		}
		check_AI_action(){
			check_slash(){
				return result
			}
			check_fire(){
				return result
			}
			check_interact(){
				return result
			}
		}
	}

	object_loop(){
		check_interactable_objects(){
			check_interaction(){
				pick_up()
				use()
				switch()
				proximity()
			}
		}
	}

	objective_loop(){
		objective_complete(){
			return result
		}
		scene_exit(){
			check_objective_completion(){
				if_complete(){
					return result
				}
				else(){
					return result
				}
			}
		}
	}

	void Update () {
		player_loop()
		npc_loop()
		object_loop()
		objective_loop()

		if(objective_complete()){
			scene_transition()
		}
	}
}
