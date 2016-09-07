using UnityEngine;
using System.Collections;

public class PickupGun : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "GameActor" && col.GetComponent<GameActor>() is PlayerActor)
        {
            PlayerActor pActor = (PlayerActor)col.GetComponent<GameActor>();
            pActor.hasGun = true;
			pActor.gunImage.enabled = true;
			pActor.gunSlider.enabled = true;
			pActor.gunSliderObject.SetActive (true);
			pActor.gunSliderFill.color = Color.green;
            Destroy(gameObject);
        }
    }
}
