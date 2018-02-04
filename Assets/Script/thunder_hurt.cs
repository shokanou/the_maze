using UnityEngine;
using System.Collections;

public class thunder_hurt : MonoBehaviour {

    player playerScript;
    GameObject playerObj;
    GameObject Cam;
    GameObject wholeCam;
    public int damage = 1;
    float hurt_time;

	// Use this for initialization
	void Start () {
        playerObj = GameObject.Find("Player");
        Cam = GameObject.Find("Main Camera");
        wholeCam = GameObject.Find("Camera");
        playerScript = playerObj.GetComponent<player>();
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
            playerScript.ThunderLossLife(damage);
        }
    }


    //角色在火焰中
    void OnTriggerStay(Collider collider)
    {

        if (collider.name == "Player")
        {
            // 持续扣血,如果帧率很低，会发现掉血不能较严格
            hurt_time += Time.deltaTime;
            if (hurt_time > 1)
            {
                playerScript.ThunderLossLife(damage);
                hurt_time -= 1;
            }
        }
    }
}
