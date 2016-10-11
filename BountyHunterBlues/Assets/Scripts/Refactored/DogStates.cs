﻿using UnityEngine;
using System.Collections;

public class NeutralDog: AIState {

	DogEnemy enemy;

	public NeutralDog(DogEnemy enemy){
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
        //Debug.Log(enemy.transform.position);

        if(new Vector2(enemy.transform.position.x, enemy.transform.position.y) != enemy.get_neutral_position()){
	        Debug.Log(enemy.transform.position);
	        Debug.Log(enemy.get_neutral_position());
        	enemy.calc_shortest_path(enemy.transform.position, enemy.get_neutral_position());
        //Debug.Log(enemy.path_length());
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
    	}
    	
    	Debug.Log("ok");
	}

	public string name(){
		return "NEUTRAL";
	}
}

public class AlertDog: AIState {
	DogEnemy enemy;

	public AlertDog(DogEnemy enemy){
		this.enemy = enemy;
	}
	public void on_enter(){}

	public void on_exit(){}

	public void execute(){
    	Debug.Log("o");
		Vector2 worldFaceDir = enemy.getClosestAttackable().gameObject.transform.position - enemy.gameObject.transform.position;
        worldFaceDir.Normalize();

        Vector2 localFaceDir = enemy.transform.InverseTransformDirection(worldFaceDir);
        Vector2 dir = Vector2.MoveTowards(enemy.faceDir, localFaceDir, 2 * Time.deltaTime);
        dir.Normalize();
        enemy.faceDir = dir;

        enemy.AI_move.updateCommandData(localFaceDir);
        enemy.AI_move.execute(enemy.getGameActor());
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
