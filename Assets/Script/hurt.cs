using UnityEngine;
using System.Collections;

public class hurt : MonoBehaviour {
	player playerScript;
	GameObject playerObj;
	GameObject Cam;
	GameObject wholeCam;
	public int damage = 1;
	float hurt_time;
	// Use this for initialization
	void Start () {
		playerObj = GameObject.Find ("Player");
		Cam = GameObject.Find ("Main Camera");
		wholeCam = GameObject.Find("Camera");
		playerScript = playerObj.GetComponent<player> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// 主角进入火焰范围
	void OnTriggerEnter(Collider collider)
	{
		if (collider.name == "Player")
		{
			hurt_time = 0;
			playerScript.Losslife (damage);
			if (Cam.GetComponent<Camera> ().enabled == true) {
				Cam.GetComponent<shakeCameraAnimate> ().shakeCamera (0.4f, 0.02f);//震动屏幕
			} else {
				wholeCam.GetComponent<shakeCameraAnimate>().shakeCamera (0.4f,0.02f);//震动屏幕
			}

			//player.damageValue = damage;
			//player.hurt = true;
		}
	}


	//角色在火焰中
	void OnTriggerStay(Collider collider){

		if (collider.name == "Player") {
			// 持续扣血,如果帧率很低，会发现掉血不能较严格
			hurt_time += Time.deltaTime;
			if (hurt_time > 1) {
				playerScript.Losslife (damage);
				if (Cam.GetComponent<Camera> ().enabled == true) {
					Cam.GetComponent<shakeCameraAnimate> ().shakeCamera (0.4f, 0.02f);//震动屏幕
				} else {
					wholeCam.GetComponent<shakeCameraAnimate>().shakeCamera (0.4f,0.02f);//震动屏幕
				}

				hurt_time -= 1;
			}
		}

	}

	/* 主角离开火焰范围
	void OnTriggerExit(Collider collider)
	{
		if (collider.name == "Player")
		{
			player.hurt = false;
		}
	}*/
}
