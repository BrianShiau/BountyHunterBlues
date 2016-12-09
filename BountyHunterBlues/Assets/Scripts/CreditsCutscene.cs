using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsCutscene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (JumpToScene ());
	
	}

	IEnumerator JumpToScene(){
		yield return new WaitForSeconds (49.0f);
		SceneManager.LoadScene ("0_MainMenu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
