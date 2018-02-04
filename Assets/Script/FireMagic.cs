using UnityEngine;
using System.Collections;

public class FireMagic : MonoBehaviour {

	GameObject mainCam;
	GameObject wholeCam;
	player playerScript;
	// Use this for initialization
	void Start () {
		mainCam = GameObject.Find ("Main Camera");
		wholeCam = GameObject.Find ("Camera");


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		playerScript = gameObject.GetComponentInParent<player> ();
		playerScript.playerAble = false;
		StartCoroutine (delayToDisactiveAll (4.0f));
		StartCoroutine (delayToShake (1.0f));
	}

	public IEnumerator delayToDisactiveAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.SetActive (false);
		playerScript.playerAble = true;
	}
	public IEnumerator delayToShake(float delaySeconds)
	{
		print ("shake");
		yield return new WaitForSeconds(delaySeconds);
		if (mainCam.GetComponent<Camera>().enabled == true) {
			mainCam.GetComponent<shakeCameraAnimate> ().shakeCamera (1.8f, 0.03f);
		} 
		else 
		{
			wholeCam.GetComponent<shakeCameraAnimate>().shakeCamera (1.8f,0.03f);
		}


	}
}
