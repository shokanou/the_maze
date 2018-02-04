using UnityEngine;
using System.Collections;

public class rockGather : MonoBehaviour {

	private Vector3 velocity = Vector3.zero;
	float gatherTime = 1;
	float xRotate = 1;
	float yRotate = 1;
	float zRotate = 1;
	Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		originalPosition = transform.localPosition;
		gatherTime = Random.Range (0.5f, 1.0f);
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);
	}

	void OnDisable () {
		transform.localPosition = originalPosition;
		gatherTime = Random.Range (0.5f, 1.0f);
		xRotate = Random.Range (-300, 300);
		yRotate = Random.Range (-300, 300);
		zRotate = Random.Range (-300, 300);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref velocity, gatherTime);
		transform.Rotate (new Vector3 (xRotate*Time.deltaTime, yRotate*Time.deltaTime, zRotate*Time.deltaTime), Space.Self);
	}
}
