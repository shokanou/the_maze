using UnityEngine;
using System.Collections;

public class delayToActiveCollider : MonoBehaviour {


	public float delayTime = 1.0f;
	public float delayTimeInactive = 3.0f;
	// Use this for initialization
	void Start () {
	}

	void OnEnable(){
		StartCoroutine (delayToActive (delayTime));
		StartCoroutine (delayToInactive (delayTimeInactive));
	}

	// Update is called once per frame
	void Update () {
	}

	public IEnumerator delayToActive(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.GetComponent<BoxCollider> ().enabled = true;
	}
	public IEnumerator delayToInactive (float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.GetComponent<BoxCollider> ().enabled = false;
	}
}
