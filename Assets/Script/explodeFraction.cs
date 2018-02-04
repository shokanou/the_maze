using UnityEngine;
using System.Collections;

public class explodeFraction : MonoBehaviour {

	private Vector3 velocity = Vector3.zero;
	Vector3 targetPosition = Vector3.zero;
	float splitTime = 1;
	float xRotate = 1;
	float yRotate = 1;
	float zRotate = 1;
	Vector3 originalPosition;
	float lerp = 0;
	float step = 0.05f;
	// Use this for initialization


	void Start () {
		originalPosition = transform.localPosition;
		targetPosition = 15 * originalPosition + new Vector3(0,20,0);

	}
	void OnEnable(){
		lerp = 0;
		splitTime = Random.Range (0.5f, 1.0f);
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);

	}

	void OnDisable(){
		transform.localPosition = originalPosition;
		transform.localRotation = Quaternion.Euler (Vector3.zero);
		splitTime = Random.Range (0.5f, 1.0f);
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);

	}

	// Update is called once per frame
	void Update () {
		if (lerp < 1) {
			lerp += step;
		}
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, splitTime);
		transform.Rotate (new Vector3 (xRotate*Time.deltaTime, yRotate*Time.deltaTime, zRotate*Time.deltaTime), Space.Self);
	//	print (transform.GetComponent <Renderer> ().material.color);
		//transform.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (0.5f, 0.6f, 0.3f, 0.2), new Color (0.5f, 0.6f, 0.3f, 0.0f), lerp));
	}


}
