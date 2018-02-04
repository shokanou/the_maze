using UnityEngine;
using System.Collections;

public class froze : MonoBehaviour {

	Color iceColorClear;
	Color iceColor;
	float lerp = 0;
	float glowSpeed = 0.005f;
	// Use this for initialization
	void Start () {
		iceColorClear = transform.GetComponent<Renderer> ().material.GetColor("_TintColor");
		iceColor = new Color (iceColorClear.r, iceColorClear.g, iceColorClear.b, 0.5f);
	}

	void OnEnable () {
		lerp = 0;
	}

	void OnDisable(){
		transform.GetComponent <Renderer> ().material.SetColor ("_TintColor", iceColorClear);
		transform.GetComponent<melt> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (lerp < 1) {
			transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (iceColorClear, iceColor, lerp));
			lerp += glowSpeed;
		}

	}
}
