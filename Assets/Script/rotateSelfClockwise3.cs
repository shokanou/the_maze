using UnityEngine;
using System.Collections;

public class rotateSelfClockwise3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, 0, 100*Time.deltaTime), Space.Self);
	}
}
