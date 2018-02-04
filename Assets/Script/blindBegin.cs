using UnityEngine;
using System.Collections;

public class blindBegin : MonoBehaviour {

	public Transform playerBlind;
	// Use this for initialization
	void Start () {
	
	}



	void OnEnable(){
		playerBlind.GetComponentInChildren<ParticleSystem> ().Play ();
	}

	void OnDisable(){
		playerBlind.GetComponentInChildren<ParticleSystem> ().Stop ();
	}

	
	// Update is called once per frame
	void Update () {
		

	}
}
