using UnityEngine;
using System.Collections;

public class EarthMagic : MonoBehaviour {

	GameObject rockAnime;
	player playerScript;
	// Use this for initialization
	void Start () {
		rockAnime = transform.FindChild ("rockAnimation").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		playerScript = gameObject.GetComponentInParent<player> ();
		playerScript.playerAble = false;
		StartCoroutine (delayToActive (1.0f));
		StartCoroutine (delayToDisactive (4.0f));
		StartCoroutine (delayToDisactiveAll (6.0f));
	}

	public IEnumerator delayToActive(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		rockAnime.SetActive (true);
	}
	public IEnumerator delayToDisactive(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		rockAnime.SetActive (false);
	}	
	public IEnumerator delayToDisactiveAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.SetActive (false);
		playerScript.playerAble = true;
	}
}
