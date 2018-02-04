using UnityEngine;
using System.Collections;

public class delayToDisappearFraction : MonoBehaviour {

	public float delayTime = 20.0f;
	public float delayTimeFraction = 17.0f;
	Transform fractions;
	Transform seal;
	// Use this for initialization
	void Start () {
		fractions = transform.FindChild ("Fractured Object");
		seal = transform.FindChild ("seal");
	}

	void OnEnable () {
		StartCoroutine (delayToDisappearAll (delayTime));
		StartCoroutine (delayToDisappearSeal (delayTimeFraction));
	}
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator delayToDisappearSeal(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		seal.gameObject.SetActive (false);
		fractions.gameObject.SetActive (true);
	}

	public IEnumerator delayToDisappearAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.SetActive (false);
		seal.gameObject.SetActive (true);
	}
}
