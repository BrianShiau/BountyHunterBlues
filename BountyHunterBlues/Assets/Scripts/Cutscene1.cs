using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Cutscene1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (JumpToScene ());
	
	}

	IEnumerator JumpToScene(){
		yield return new WaitForSeconds (7.0f);
		SceneManager.LoadScene ("0_Level0");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
