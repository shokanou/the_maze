using UnityEngine;
using System.Collections;
//萨拉曼之怒
//葛罗姆之盾
//希尔芙之翼
//温蒂妮之息
public class IceMagic : MonoBehaviour {

	Transform air;
	GameObject iceMagic;
	player playerScript;
	// Use this for initialization
	void Start () {

	}

	void OnEnable(){
		playerScript = gameObject.GetComponentInParent<player> ();
		playerScript.playerAble = false;
		air = transform.FindChild ("Air");
		iceMagic = transform.FindChild ("IceMagic").gameObject;
		air.GetComponent<ParticleEmitter> ().emit = true;
		StartCoroutine (delayToActiveIceMagic (1.0f));
		StartCoroutine (delayToStopEmit (2.0f));
		StartCoroutine (delayToDisactiveIceMagic (3.0f));
		StartCoroutine (delayToDisactiveAll (4.0f));
	}
	
	// Update is called once per frame
	void Update () {

	}

	public IEnumerator delayToActiveIceMagic(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		iceMagic.SetActive (true);
	}
	public IEnumerator delayToStopEmit(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		air.GetComponent<ParticleEmitter> ().emit = false;
	}
	public IEnumerator delayToDisactiveIceMagic(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		iceMagic.SetActive (false);
	}
	public IEnumerator delayToDisactiveAll(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		gameObject.SetActive (false);
		playerScript.playerAble = true;
	}
}
