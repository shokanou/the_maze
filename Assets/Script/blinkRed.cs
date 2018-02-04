using UnityEngine;
using System.Collections;

public class blinkRed : MonoBehaviour {

	public float duration = 3.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float lerp = Mathf.PingPong (Time.time, duration) / duration;
		transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (0.9f, 0.1f, 0.1f, 0), new Color (0.9f, 0.1f, 0.1f, 0.5f), lerp));
	}
}
