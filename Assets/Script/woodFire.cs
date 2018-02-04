using UnityEngine;
using System.Collections;

public class woodFire : MonoBehaviour {

	Transform smoke;
	Transform fire;
	Transform wood;

	// Use this for initialization
	void Start () {
		smoke = transform.FindChild("smoke");
		fire = transform.FindChild("fire");
		wood = transform.FindChild("wooddoor");

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
		wood.gameObject.SetActive (false);
		transform.GetComponent<BoxCollider> ().enabled = false;
		if (gameObject.name == "vineAll") {
			Destroy (transform.parent.gameObject);
		} 
		else 
		{
			Destroy (gameObject);
		}

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
