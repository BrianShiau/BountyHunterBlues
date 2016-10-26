using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {
	public Texture2D cursorTexture;
	public Texture2D secondaryCursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Texture2D[] cursorTextures;

	private GameObject midPointers;
	private Camera screenCamera;
	private GameObject[] midPointerReloads;
	private Animator[] midPointerReloadAnims;
	private GameObject directionPointer;

	private PlayerActor player;

	// Use this for initialization
	void Start () {
		UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.height/2, cursorTexture.width/2), cursorMode);
		midPointers = transform.FindChild ("MidPointers").gameObject;
		midPointerReloads = new GameObject[3];
		midPointerReloads[0] = midPointers.transform.FindChild ("PointerMidReload").gameObject;
		midPointerReloads[1] = midPointers.transform.FindChild ("PointerMidReload (1)").gameObject;
		midPointerReloads[2] = midPointers.transform.FindChild ("PointerMidReload (2)").gameObject;
		midPointerReloadAnims = new Animator[3];
		midPointerReloadAnims[0] = midPointerReloads[0].GetComponent<Animator> ();
		screenCamera = GetComponentInChildren<Camera> ();
		player = GetComponent<PlayerActor> ();
		midPointerReloadAnims [0].speed = 2 / player.reloadTime;
		directionPointer = transform.FindChild ("DirectionPointer").gameObject;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		//disable cursors in tactical mode
		if (GetComponentInParent<PlayerActor> ().InTacticalMode()) {
			UnityEngine.Cursor.visible = false;
			midPointers.SetActive (false);
			return;
		} else {
			UnityEngine.Cursor.visible = true;
			if (player.hasGun) {
				if (!player.InDialogueMode ()) {
					midPointers.SetActive (true);
				} else {
					midPointers.SetActive (false);
				}
			} else {
				midPointers.SetActive (false);
			}
		}

		//ammo counter animation
		float lastShotTime = player.getLastShotTime ();
		if (lastShotTime > player.reloadTime-.1) {
			midPointerReloadAnims [0].SetFloat ("ReloadTime", 2.0f);
		}else if (lastShotTime < .1){
			midPointerReloadAnims [0].SetFloat ("ReloadTime", 0.0f);
		}

		//ammo counter count
		int start = Mathf.Min(player.getMagazineSize (), 2);
		for (int i = 2; i > start; i--) {
			midPointerReloads [i].SetActive (false);
		}
		for (int i = start; i >= 0; i--) {
			midPointerReloads [i].SetActive (true);
			midPointerReloads [i].transform.localPosition = new Vector3 (midPointerReloads [i].transform.localPosition.x, -5 + 5*(start-i), midPointerReloads [i].transform.localPosition.z);
		}

		//hardware cursors for main cursor
		int cursorIndex = (int)((GetComponentInParent<PlayerActor> ().getLastShotTime () / GetComponentInParent<PlayerActor> ().reloadTime) * cursorTextures.Length);
		cursorIndex = Mathf.Min (cursorIndex, cursorTextures.Length-1);
		UnityEngine.Cursor.SetCursor(cursorTextures[cursorIndex], new Vector2(cursorTexture.height/2, cursorTexture.width/2), cursorMode);

		//ammo counter transformation
		SpriteRenderer sr = midPointerReloads[0].GetComponent<SpriteRenderer>();
		midPointers.transform.localScale = new Vector3(1,1,1);

		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		Vector3 imgScale = new Vector3(1f, 1f, 1f);
		Vector2 ratio = new Vector2(width / height, height / width);
		imgScale.x = worldScreenWidth / width;
		imgScale.y = imgScale.x * ratio.y; 
		midPointers.transform.localScale = imgScale;

		midPointers.transform.eulerAngles = new Vector3(
			midPointers.transform.eulerAngles.x,
			midPointers.transform.eulerAngles.y,
			-90);
		Vector3 worldSpace = screenCamera.ScreenToWorldPoint(new Vector3(-350+20*imgScale.x*width, -170+10*imgScale.y*height, screenCamera.nearClipPlane));
		midPointers.transform.position = worldSpace;

		//directional pointer
		Vector2 pivot = new Vector2(Screen.width/2, Screen.height/2);
		float distFromCenterX = Event.current.mousePosition.x - Screen.width / 2;
		float distFromCenterY = Event.current.mousePosition.y - Screen.height / 2;
		float angle = Mathf.Atan2 (distFromCenterY, distFromCenterX);

		directionPointer.transform.eulerAngles = new Vector3(
			midPointers.transform.eulerAngles.x,
			midPointers.transform.eulerAngles.y,
			-90-angle*Mathf.Rad2Deg);

		float x = (Event.current.mousePosition.x + Screen.width / 2) / 2;
		float y = (Event.current.mousePosition.y + Screen.height / 2) / 2;

		Vector3 center = screenCamera.ScreenToWorldPoint(
			new Vector3(Screen.width/2, Screen.height/2, screenCamera.nearClipPlane));
		Vector3 worldSpace2 = screenCamera.ScreenToWorldPoint(
			new Vector3(x, Screen.height - y, screenCamera.nearClipPlane));
		Vector3 normalized = worldSpace2 - center;
		normalized.Normalize ();
		Vector3 offset = new Vector3 (0, -.7f, 0);
		directionPointer.transform.position = center + normalized + offset;

		//triple mid pointers which follow the cursor
		/*Vector2 pivot = new Vector2(Screen.width/2, Screen.height/2);
		float distFromCenterX = Event.current.mousePosition.x - Screen.width / 2;
		float distFromCenterY = Event.current.mousePosition.y - Screen.height / 2;
		float angle = Mathf.Atan2 (distFromCenterY, distFromCenterX);

		midPointers.transform.eulerAngles = new Vector3(
			midPointers.transform.eulerAngles.x,
			midPointers.transform.eulerAngles.y,
			-90-angle*Mathf.Rad2Deg);

		float x = (Event.current.mousePosition.x + Screen.width / 2) / 2;
		float y = (Event.current.mousePosition.y + Screen.height / 2) / 2;

		Vector3 worldSpace = screenCamera.ScreenToWorldPoint(
			new Vector3(x, Screen.height - y, screenCamera.nearClipPlane));
		midPointers.transform.position = worldSpace;*/


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
