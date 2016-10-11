using UnityEngine;
using System.Collections;

public class NeutralDog: AIState {

	GameActor actor;
	Vector2 self;

	public NeutralDog(GameActor actor, GameObject self){
		this.actor = actor;
		this.self = self;
	}

	public void on_enter(){}

	public void on_exit(){}

	public void execute(GameObject self){
		Vector2 worldFaceDir = actor.gameObject.transform.position - self.transform.position;
        worldFaceDir.Normalize();

        Vector2 localFaceDir = self.transform.InverseTransformDirection(worldFaceDir);
        Vector2 dir = Vector2.MoveTowards(self.faceDir, localFaceDir, 2 * Time.deltaTime);
        dir.Normalize();
        Debug.Log(dir);
        self.faceDir = actor.gameObject.transform.position;
        Debug.Log(self.faceDir);
	}

	public string name(){
		return "NEUTRAL";
	}
}

public class AlertDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(GameObject self){}

	public string name(){
		return "ALERT";
	}
}

public class AggresiveDog: AIState {
	public void on_enter(){}

	public void on_exit(){}

	public void execute(GameObject self){}

	public string name(){
		return "AGGRESIVE";
	}
}
