using UnityEngine;
using System.Collections;

public class cameraPositionSet : MonoBehaviour {

	public Transform player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (0, player.position.y + 30, -19);
	}
}
