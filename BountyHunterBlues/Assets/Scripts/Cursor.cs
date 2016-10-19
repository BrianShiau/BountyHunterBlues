﻿using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {
	public Texture2D cursorTexture;
	public Texture2D secondaryCursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Texture2D[] cursorTextures;

	private GameObject pointerMidReload;
	private Camera screenCamera;

	// Use this for initialization
	void Start () {
		UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.height/2, cursorTexture.width/2), cursorMode);
		pointerMidReload = transform.FindChild ("MidPointers").gameObject;
		screenCamera = GetComponentInChildren<Camera> ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		if (GetComponentInParent<PlayerActor> ().InTacticalMode()) {
			UnityEngine.Cursor.visible = false;
			pointerMidReload.SetActive (false);
			return;
		} else {
			UnityEngine.Cursor.visible = true;
			pointerMidReload.SetActive (true);
		}
			
		int cursorIndex = (int)((GetComponentInParent<PlayerActor> ().getLastShotTime () / GetComponentInParent<PlayerActor> ().reloadTime) * cursorTextures.Length);
		cursorIndex = Mathf.Min (cursorIndex, cursorTextures.Length-1);
		UnityEngine.Cursor.SetCursor(cursorTextures[cursorIndex], new Vector2(cursorTexture.height/2, cursorTexture.width/2), cursorMode);

		Vector2 pivot = new Vector2(Screen.width/2, Screen.height/2);
		float distFromCenterX = Event.current.mousePosition.x - Screen.width / 2;
		float distFromCenterY = Event.current.mousePosition.y - Screen.height / 2;
		float angle = Mathf.Atan2 (distFromCenterY, distFromCenterX);

		pointerMidReload.transform.eulerAngles = new Vector3(
			pointerMidReload.transform.eulerAngles.x,
			pointerMidReload.transform.eulerAngles.y,
			-90-angle*Mathf.Rad2Deg);

		float x = (Event.current.mousePosition.x + Screen.width / 2) / 2;
		float y = (Event.current.mousePosition.y + Screen.height / 2) / 2;

		Vector3 worldSpace = screenCamera.ScreenToWorldPoint(
			new Vector3(x, Screen.height - y, screenCamera.nearClipPlane));
		pointerMidReload.transform.position = worldSpace;

		//old texture
		//GUIUtility.RotateAroundPivot(90+angle*Mathf.Rad2Deg, pivot);
		//GUI.DrawTexture (new Rect (x, y, 48, 48), secondaryCursorTexture);
	}


	//doesnt work for some reason
	public static Texture2D textureFromSprite(Sprite sprite)
	{
		if(sprite.rect.width != sprite.texture.width){
			Texture2D newText = new Texture2D((int)Mathf.Floor(sprite.rect.width-100),(int)Mathf.Floor(sprite.rect.height-100));
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
				(int)sprite.textureRect.y, 
				(int)sprite.textureRect.width, 
				(int)sprite.textureRect.height );
			//Debug.Log ();
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;

		} else
			return sprite.texture;
	}
}
