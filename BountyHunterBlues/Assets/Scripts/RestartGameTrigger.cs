using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartGameTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "GameActor" && col.GetComponent<GameActor>() is PlayerActor)
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Additive);
		
    }
}
