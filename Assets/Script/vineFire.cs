using UnityEngine;
using System.Collections;

public class vineFire : MonoBehaviour {

	Transform vineAll;
	Transform smoke;
	Transform fire;
	Transform vine;

	// Use this for initialization
	void Start () {
		vineAll = transform.FindChild("vineAll");
		smoke = vineAll.FindChild("smoke");
		fire = vineAll.FindChild("fire");
		vine = vineAll.FindChild("vine");

		StartCoroutine (delayToAppear (0.8f));
		StartCoroutine (delayToDisappear (3.0f));
		StartCoroutine (delayToDisappearAll (3.0f));
	}
	
	// Update is called once per frame
	void Update () {

	}

	public IEnumerator delayToDisappearAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		vine.gameObject.SetActive (false);
		vineAll.GetComponent<BoxCollider> ().enabled = false;
		Destroy (gameObject);
	}
	public IEnumerator delayToDisappear(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		smoke.GetComponent<ParticleSystem> ().Stop();
		fire.GetComponent<ParticleSystem> ().Stop();
	}
	public IEnumerator delayToAppear(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		smoke.GetComponent<ParticleSystem> ().Play();
		fire.GetComponent<ParticleSystem> ().Play();
	}
}
