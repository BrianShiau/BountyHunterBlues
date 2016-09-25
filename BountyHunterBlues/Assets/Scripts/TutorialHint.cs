using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialHint : MonoBehaviour {

	private Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		StartCoroutine (FlashText());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator FlashText(){
		while (true) {
			text.color = new Color (text.color.r, text.color.g, text.color.b, .2f);
			for (int i = 0; i < 8; i++) {
				text.color = new Color (text.color.r, text.color.g, text.color.b, text.color.a + .1f);
				yield return new WaitForSeconds (.3f * (.1f / text.color.a));
			}
			for (int i = 0; i < 8; i++) {
				text.color = new Color (text.color.r, text.color.g, text.color.b, text.color.a - .1f);
				yield return new WaitForSeconds (.3f * (.1f / text.color.a));
			}
		}
	}
}
