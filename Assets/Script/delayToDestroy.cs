using UnityEngine;
using System.Collections;

public class delayToDestroy : MonoBehaviour {

	public float delayTime = 1.0f;
	// Use this for initialization
	void Start () {
		
	}

	void OnEnable () {
		StartCoroutine (delayToDisappearAll (delayTime));
	}
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator delayToDisappearAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		Destroy (gameObject);
	}
}
