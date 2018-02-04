using UnityEngine;
using System.Collections;

public class moveUp : MonoBehaviour {

	private Vector3 velocity = Vector3.zero;
	Vector3 targetPosition;
	float upTime = 0.03f;
	// Use this for initialization
	void Start () {
		targetPosition = transform.localPosition;
	}
	void OnDisable(){
		transform.localPosition = new Vector3 (0, -5, 0);
		targetPosition = transform.localPosition;
	}

	void OnEnable(){
		StartCoroutine (delayToUp (1.0f));
	}
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition,targetPosition, ref velocity, upTime);
	}

	public IEnumerator delayToUp(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		targetPosition = Vector3.zero;
	}
}

