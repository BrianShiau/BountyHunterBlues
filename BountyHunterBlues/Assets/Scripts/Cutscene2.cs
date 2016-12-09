using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Cutscene2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (JumpToScene ());
	
	}

	IEnumerator JumpToScene(){
		yield return new WaitForSeconds (17.0f);
		SceneManager.LoadScene ("0_Level3_r4");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
