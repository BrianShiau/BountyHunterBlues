using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartGameTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
    {
		if (col.tag == "GameActor" && col.GetComponent<GameActor> () is PlayerActor) {
			PlayerActor.deaths = 0;
			StartCoroutine(LoadNextLevel ());
			//GameObject.FindGameObjectWithTag ("BlackFade").GetComponent<Image>().enabled = true;
			//yield return new WaitForSeconds (1);
			//SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);

			//Additive stuff
			//SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1, LoadSceneMode.Additive);
			//SceneManager.SetActiveScene (SceneManager.GetSceneAt(SceneManager.GetActiveScene ().buildIndex + 1));
			//Destroy (this.gameObject);
		}
		
    }

	IEnumerator LoadNextLevel()
	{
		Time.timeScale = 0;
		Image blackFade = GameObject.FindGameObjectWithTag ("BlackFade").GetComponent<Image> ();
		Color color = blackFade.color;
		color.a = 0f;
		blackFade.color = color;
		blackFade.enabled = true;
		while (blackFade.color.a < 1) {
			color = blackFade.color;
			color.a += 0.1f;
			blackFade.color = color;
			yield return StartCoroutine(Utility.WaitForRealTime (.03f));
		}
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}
}
