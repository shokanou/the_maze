using UnityEngine;
using System.Collections;

public class blink : MonoBehaviour {

	public float duration = 3.0f;
	Color magicColor;
	Color glowColor;
	Color clearColor;
	// Use this for initialization
	void Start () {
		magicColor = transform.GetComponent<Renderer> ().material.GetColor("_TintColor");
		glowColor = magicColor;
		glowColor.a = 0.5f;
		clearColor = magicColor;
		clearColor.a = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float lerp = Mathf.PingPong (Time.time, duration) / duration;
		transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (clearColor, glowColor, lerp));
	}
}
