using UnityEngine;
using System.Collections;

public class UVanmation : MonoBehaviour {

	float offsetX = 0.0f;
	float offsetY = 0.0f;
	float offsetStep = 0.25f;
	int count = 1;
	int duration = 2;
	bool animationDone = false;
	float lerp = 0;
	public float glowSpeed = 0.1f;
	Color magicColor;
	Color glowColor;
	Color clearColor;
	// Use this for initialization
	void Start () {
		magicColor = transform.GetComponent<Renderer> ().material.GetColor("_TintColor");
		glowColor = magicColor;
		glowColor.a = 1.0f;
		clearColor = magicColor;
		clearColor.a = 0;
	}

	void OnDisable(){
		offsetX = 0.0f;
		offsetY = 0.0f;
		animationDone = false;
		count = 1;
		transform.GetComponent <Renderer> ().material.SetColor ("_TintColor", magicColor);
		lerp = 0;
	}

	// Update is called once per frame
	void Update () {
		if (!animationDone) {
		
			if (count % duration == 0) {

				transform.GetComponent <Renderer> ().material.mainTextureOffset = new Vector2 (offsetX, offsetY);
				offsetX += offsetStep;
				if (offsetX >= 1.0f) {
					offsetX = 0;
					offsetY -= offsetStep;
				}
				if (offsetY <= -1.0f) {
					offsetX = 0.75f;
					offsetY = -0.75f;
					animationDone = true;
				}
			}
			if (count == duration)
				count = 0;
			count++;
		
		} 
		else if(lerp <= 1){
			transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (magicColor, glowColor, lerp));
			lerp += glowSpeed;
		}
		else if(lerp > 1){
			transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (glowColor, clearColor, lerp-1));
			lerp += glowSpeed*0.5f;
		}
		if (lerp > 10) {
			transform.GetComponent <Renderer> ().material.mainTextureOffset = new Vector2 (0, 0);
			gameObject.SetActive (false);

		}
	}
}
