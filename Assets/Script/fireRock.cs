using UnityEngine;
using System.Collections;

public class fireRock : MonoBehaviour {
	
	Transform childRock;
	float xRotate = 1;
	float yRotate = 1;
	float zRotate = 1;
	Vector3 diffPosition = new Vector3 (0, 25, -10);
	Vector3 originalPosition;
	Vector3 targetPosition;
	// Use this for initialization
	void Start () {
		childRock = transform.FindChild("bolder1");
		targetPosition = transform.localPosition;
		originalPosition = transform.localPosition + diffPosition;
		transform.localPosition = originalPosition;
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);
	}
	void OnDisable () {
		transform.localPosition = originalPosition;
		childRock.gameObject.SetActive (true);
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);
	}
	// Update is called once per frame
	void Update () {
		//print (Time.deltaTime);
		transform.Translate (-diffPosition * Time.deltaTime * 0.4f);
		childRock.Rotate (new Vector3 (xRotate*Time.deltaTime, yRotate*Time.deltaTime, zRotate*Time.deltaTime), Space.Self);
		if (transform.position.y < -5)
			childRock.gameObject.SetActive (false);
	}
}
