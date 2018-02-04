using UnityEngine;
using System.Collections;

public class WindMagic : MonoBehaviour {

	player playerScript;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		playerScript = gameObject.GetComponentInParent<player> ();
		playerScript.playerAble = false;
		StartCoroutine (delayToActive (2.0f));
		StartCoroutine (delayToDisactive (4.0f));
		StartCoroutine (delayToDisactiveAll (5.0f));
	}

	public IEnumerator delayToActive(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		transform.FindChild ("TR").gameObject.SetActive (true);
	}
	public IEnumerator delayToDisactive(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		transform.FindChild ("TR").gameObject.SetActive (false);
	}	
	public IEnumerator delayToDisactiveAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.SetActive (false);
		playerScript.playerAble = true;
	}
}
