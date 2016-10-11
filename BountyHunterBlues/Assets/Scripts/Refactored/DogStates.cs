using UnityEngine;
using System.Collections;

public class NeutralDog: AIState {

	DogEnemy enemy;

	public NeutralDog(DogEnemy enemy){
		this.enemy = enemy;
	}

	public void on_enter(){ 
		enemy.set_shortest_path_calculated(false);
        enemy.calc_shortest_path(enemy.transform.position, enemy.get_neutral_position());
	}

	public void on_exit(){ 
		enemy.set_shortest_path_calculated(false);
	}

	public void execute(){
    	if(enemy.getClosestAttackable() != null){
			Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
	        worldFaceDir.Normalize();

	        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
	        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
	        dir.Normalize();
	        enemy.faceDir = dir;
    	}
    	else if(enemy.get_path_index() < enemy.path_length()){
        	Node current_node = enemy.path.get_node(enemy.get_path_index());
        	float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);
                
            Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
            worldFace.Normalize();
            enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);

            enemy.AI_move.updateCommandData(enemy.faceDir);
            enemy.AI_move.execute(enemy.getGameActor());
            if(distance_from_node < enemy.get_node_transition_threshold()){
                enemy.inc_path_index();   
            }
        }
    	else{
    		enemy.path.clear();
    		enemy.reset_path_index();
			Vector2 temp = Vector2.MoveTowards(enemy.faceDir, enemy.get_initial_faceDir(), enemy.rotation_speed * Time.deltaTime);
            temp.Normalize();
            enemy.faceDir = temp;
            enemy.stopMove();
    	}
	}


	public string name(){
		return "NEUTRAL";
	}
}

public class AlertDog: AIState {
	DogEnemy enemy;
	Vector2 last_seen;

	public AlertDog(DogEnemy enemy){
		this.enemy = enemy;
	}
	public void on_enter(){ 
		last_seen = new Vector2(0, 0);
	}

	public void on_exit(){
		last_seen = new Vector2(0, 0);
	}

	public void execute(){
		if(enemy.getClosestAttackable() != null){
			Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
	        worldFaceDir.Normalize();

	        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
	        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, enemy.rotation_speed * Time.deltaTime);
	        dir.Normalize();
	        enemy.faceDir = dir;
	        enemy.AI_move.updateCommandData(localFaceDir);
	        enemy.AI_move.execute(enemy.getGameActor());
	        last_seen = new Vector2(enemy.getClosestAttackable().gameObject.transform.position.x, enemy.getClosestAttackable().gameObject.transform.position.y);
			enemy.set_shortest_path_calculated(false);
		}
		else if(enemy.getClosestAttackable() == null && last_seen.x != 0 && last_seen.y != 0){
        	enemy.calc_shortest_path(enemy.transform.position, last_seen);
        	if(enemy.get_path_index() < enemy.path_length()){
	        	Node current_node = enemy.path.get_node(enemy.get_path_index());
	        	float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);
	                
	            Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
	            worldFace.Normalize();
	            enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);

	            enemy.AI_move.updateCommandData(enemy.faceDir);
	            enemy.AI_move.execute(enemy.getGameActor());
	            if(distance_from_node < enemy.get_node_transition_threshold()){
	                enemy.inc_path_index();
	            }
	        }
	        else{
	        	enemy.set_shortest_path_calculated(false);
				last_seen = new Vector2(0, 0);
	        }
		}
		else if(enemy.getClosestAttackable() == null && enemy.get_audio_location().x != 0 && enemy.get_audio_location().y != 0){
			if(Vector2.Distance(enemy.get_audio_location(), enemy.gameObject.transform.position) <= enemy.audio_distance){
				enemy.calc_shortest_path(enemy.transform.position, last_seen);
	        	if(enemy.get_path_index() < enemy.path_length()){
		        	Node current_node = enemy.path.get_node(enemy.get_path_index());
		        	float distance_from_node = Vector2.Distance(enemy.transform.position, current_node.worldPosition);
		                
		            Vector2 worldFace = current_node.worldPosition - new Vector2(enemy.transform.position.x, enemy.transform.position.y);
		            worldFace.Normalize();
		            enemy.faceDir = enemy.transform.InverseTransformDirection(worldFace);

		            enemy.AI_move.updateCommandData(enemy.faceDir);
		            enemy.AI_move.execute(enemy.getGameActor());
		            if(distance_from_node < enemy.get_node_transition_threshold()){
		                enemy.inc_path_index();
		            }
		        }
		        else{
		        	enemy.set_shortest_path_calculated(false); 
		        	enemy.set_audio_location(new Vector2(0, 0));
		        }
			}
		}
		else{
            enemy.stopMove();
		}
	}

	public string name(){
		return "ALERT";
	}
}

public class AggresiveDog: AIState {
	DogEnemy enemy;

	public AggresiveDog(DogEnemy enemy){
		this.enemy = enemy;
	}

	public void on_enter(){}

	public void on_exit(){}

	public void execute(){
		Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
        worldFaceDir.Normalize();

        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, 2 * Time.deltaTime);
        dir.Normalize();
        enemy.faceDir = dir;
	}

	public string name(){
		return "AGGRESIVE";
	}
}
