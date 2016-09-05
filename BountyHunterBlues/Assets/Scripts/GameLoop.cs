using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {
/*
	// Use this for initialization
	void Start () {
		load_scene()
		load_level()
		load_npcs()
		load_items()
		load_player()
	}
	
	// Update is called once per frame
	void player_loop(){
		Command nextCommand = handleInput();
        if (nextCommand != null)
            nextCommand.execute(player.GetComponent<PlayerActor>());
	}

	void npc_loop(){
		animate_AI(){
			animate_AI()
			run_AI_detection(){
				Green:
					Stationary position and orientation
				    No weapon drawn (faceDir)
				    if(ray_cone hit - ray)
				        rotate()
				    If(look_ray hit || audio_trigger)
				        Set lookTarget = player || audio_location
				        run_delay()
				        run(yellow)
				Yellow:
				    Variable position, variable orientation
				    run_timer()
				    No weapon drawn
				    Follow lookTarget
				        if(lookTarget is null)
				        	if(audio) delay(6)
				            else delay(3)
				            Teleport back to original position and orientation
				            run(green)
				        if(timer == red_timer):
				            run(red)
				Red:
				    Variable position, variable orientation
				    Hold right mouse button
				    Aim down lookTarget
				    run_aim_confirmed_timer()
				        Visual indication
				    if(timer_up && target != null)
				        fire()
				    if(target == null)
				        run_delay()
				        run(yellow)
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
			check_fire(){
				return result
			}
		}
	}

	void object_loop(){
		check_interactable_objects(){
			check_interaction(){
				pick_up()
			}
		}
	}

	void objective_loop(){
		objective_complete(){
			return result
		}
		scene_exit(){
			check_objective_completion(){
				if_complete(){
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
*/
}
