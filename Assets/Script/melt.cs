using UnityEngine;
using System.Collections;

public class melt : MonoBehaviour {

	Color iceColorClear;
	Color iceColor;
	float lerp = 0;
	float glowSpeed = 0.005f;
	// Use this for initialization
	void Start () {
		iceColor = transform.GetComponent<Renderer> ().material.GetColor("_TintColor");
		iceColorClear = new Color (iceColorClear.r, iceColorClear.g, iceColorClear.b, 0);
	}

	void OnEnable()
	{
		lerp = 0;
	}

	// Update is called once per frame
	void Update () {
		if (lerp < 1) {
			transform.GetComponent <Renderer> ().material.SetColor ("_TintColor", Color.Lerp (iceColor, iceColorClear, lerp));
			lerp += glowSpeed;
		}
		else {
			gameObject.SetActive (false);
		}
	}
}
