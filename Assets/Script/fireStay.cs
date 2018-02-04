using UnityEngine;
using System.Collections;

public class fireStay : MonoBehaviour {

	GameObject fire;
	GameObject ash;
	bool ashFlag = false;	//渐变淡入
	bool ashStop = false;	//自然停止
	bool ashBreak = false; //被打断
	bool ashBreakAlready = false; //已经被打断
	float lerp = 0;
	float lerpStop = 0;
	float step = 0.05f;
	RaycastHit hit;           // 这个是碰撞检测的目标
	player player_script;     // player脚本
	bool isExtinguish = false; //火焰熄灭


	// Use this for initialization
	void Start () {
		fire = transform.FindChild ("fire").gameObject;
		ash = transform.FindChild ("ash").gameObject;
		player_script = GameObject.Find ("Player").GetComponent<player> ();
	}

	void OnEnable(){
		ashBreakAlready = false; 
		lerp = 0;
		lerpStop = 0;
		ashFlag = false;
		ashStop = false;
		ashBreak = false;
		StartCoroutine (delayToAppear (0.0f));
		StartCoroutine (delayToDisappear (10.0f));


	}
	
	// Update is called once per frame
	void Update () {
		// 消除冰
		if (isExtinguish == false && player.seal == false && Physics.Raycast(transform.position, new Vector3(0, 1, 0), out hit, 40))
		{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
			GameObject obj = hit.transform.gameObject;
			if (obj.tag == "Steam" && obj.GetComponent<ParticleSystem>().isStopped) {
				obj.transform.FindChild ("icefloor").GetComponent<melt> ().enabled = true;
				// 此处加冰块融化动画（如果有的话）以及消除冰块
				StartCoroutine (player_script.delayToFly (obj, 2.0f));
			}
		}
		/*
		if (Input.GetKeyUp("i")&&!ashBreakAlready)
		{
			fire.GetComponent<ParticleSystem> ().Stop ();
			ashStop = true;
			lerpStop = 0;
			ashBreak = true;
			ashBreakAlready = true;
		}*/

		if (ashFlag == true) {
			ash.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (1.0f, 0.7f, 0.0f, 0), new Color (1.0f, 0.7f, 0.0f, 0.5f), lerp));
			lerp += step;
			if (lerp >= 1) {
				ashFlag = false;
			}

		}

		if (ashStop == true && !ashBreak) {
			ash.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (1.0f, 0.7f, 0.0f, 0.5f), new Color (1.0f, 0.7f, 0.0f, 0), lerpStop));
			lerpStop += step;

			if (lerpStop >= 1) {
				ashStop = false;
				gameObject.GetComponent<BoxCollider> ().enabled = false;
			}
		}
		if (ashBreak) {
			ash.GetComponent <Renderer> ().material.SetColor("_TintColor",Color.Lerp (new Color (1.0f, 0.7f, 0.0f, 0.5f), new Color (1.0f, 0.7f, 0.0f, 0), lerpStop));
			lerpStop += step;

			if (lerpStop >= 1) {
				ashStop = false;
				gameObject.GetComponent<BoxCollider> ().enabled = false;
			}
		}
	}

	// 冰冻魔法的影响
	void OnTriggerEnter(Collider collider)
	{
		if (collider.name == "Ice"&&!ashBreakAlready)
		{
			//print (collider.name);
			//Destroy(this.gameObject);
			 //这里改为你的销毁火焰的代码 
			StartCoroutine(delayToStopFire(0.2f));
			print (fire.GetComponent<ParticleSystem>().isStopped);
			ashStop = true;
			lerpStop = 0;
			ashBreak = true;
			ashBreakAlready = true;
			isExtinguish = true;
		}
	}

	public IEnumerator delayToStopFire (float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		fire.GetComponent<ParticleSystem> ().Stop ();
	}

	public IEnumerator delayToAppear (float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		fire.GetComponent<ParticleSystem> ().Play ();
		ashFlag = true;
		lerp = 0;
	}

	public IEnumerator delayToDisappear (float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		ashBreakAlready = true;//防止在自然消失时打断
		//print (ashBreak);
		if (!ashBreak) {
			fire.GetComponent<ParticleSystem> ().Stop ();
			ashStop = true;
			lerpStop = 0;
			isExtinguish = true;
		}
	}
}
