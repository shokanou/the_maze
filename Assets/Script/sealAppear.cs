using UnityEngine;
using System.Collections;

public class sealAppear : MonoBehaviour {

	float lerp = 0;
	float step = 0.05f;
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable () {
		lerp = 0;
	}

	void OnDisable () {
		lerp = 0;
		transform.GetComponent <Renderer> ().material.SetColor("_TintColor",new Color (0.5f, 0.6f, 0.3f, 0.0f));
	}
	
	// Update is called once per frame
	void Update () {
		if (lerp < 1) {
			lerp += step;
		}
		transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (0.5f, 0.6f, 0.3f, 0), new Color (0.5f, 0.6f, 0.3f, 0.2f), lerp));
	}
}
