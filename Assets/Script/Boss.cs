using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	Animator bossAnimator;				//Boss动画控制器
    Vector3 position;                    // 位置
    float timer;                         // 时间
    public GameObject player_obj;        // 玩家这个物件
    public GameObject wall;              // 木墙
    player player_script;                // 玩家这个脚本
	Transform bossContainer;  //boss父物体
	public GameObject fireStay;  //持续火焰
	GameObject wholeCam;//远景摄像机
	GameObject Cam;//近景摄像机



	/*魔法列表*/
	public GameObject lightMagic;
	public GameObject darkMagic;
	public GameObject meteoMagic;
	public GameObject woodMagic;
	public GameObject blindMagic;
	public GameObject timeMagic;
	public GameObject sealMagic;
	public GameObject teleportBegin;
	public GameObject teleportEnd;

    /* 平静状态 */
    bool stand;                          // 是否处于平静状态
    float time_float;                    // 浮动循环时间
    float floating_time;                 // 一个循环内的时间
    float floating_ratio;                // 静止循环时间转化为角度的比例
    //Vector3 floating_offset;             // 浮动的偏移量
    //Vector3 floating_radius = new Vector3(0, 1, 0);       // 半径
    //Vector3 floating_radius2 = new Vector3(0, 1, 0);

    /* 砸地大招 */
    bool sprint;                                // 使用砸地技能
    int sprint_stage;                           // 技能阶段（四个）
    public float time_up = 0.5f;                // 向上飞行时间
    public float time_storage = 2;              // 蓄力时间
    public float time_down = 0.1f;              // 向下冲刺时间
    float pi2 = Mathf.PI / 2;                   // Pi/2
    float sprint_ratio;                         // 向下冲刺时间转化为角度的比例
    public float time_stiff_sprint = 1;         // 硬直时间
    Vector3 sprint_targetposition;              // 目标位置
    Vector3 up_position = new Vector3(0, 8, -16.5f);     // 向上位置偏移
    Vector3 down_position = new Vector3(0, -14.5f, 0);   // 向下位置偏移
    Vector3 velocity = Vector3.zero;

    /* 激光炮 */
    bool laser;                                 // 使用激光炮技能
    public float time_laser = 1;                // 发射激光炮时间
    int laser_position;                         // 发射激光炮位置标号
    Vector3[] laser_positions = new Vector3[9]; // 发射激光炮可能位置

    /* 火球 */
    // bool fireball;                              // 使用火球技能
    int[] fireball_position = new int[4];          // 火球目标位置
    Vector3[] fireball_positions = new Vector3[9]; // 火球可能位置

    /* 木墙 */
    RaycastHit hit;                                // 检测无墙位置
    float raylength = 6;                           // 射线长度
    Vector3[] woodwall_position = new Vector3[4];  // 木墙目标位置偏移
    Quaternion wood_q = new Quaternion(0, -0.7f, 0, 0.7f);   // 绕Y轴-90°

    /* 减慢移动速度 */
    bool slow;                                   // 标记是否减缓
    public float slow_time = 2;                  // 减速持续时间
    float slow_timer;                            // 减速计时器

    /* 封印楼层 */
    public float seal_time = 5;                  // 楼层被封印的时间
    float seal_timer;                            // 封印计时器
    // 封印楼层中的蒸汽
    Vector3 ray_start1 = new Vector3(-15, 0, 10);
    Vector3 ray_start2 = new Vector3(-15, 0, 0);
    Vector3 ray_start3 = new Vector3(-15, 0, -10);
    Vector3 ray_forward = new Vector3(1, 0, 0);    // 射线方向
    float ray_length = 25f;                        // 射线长度
    RaycastHit[] hits1, hits2, hits3;              // 三个碰撞检测结果
    Vector3 offset;                                // 需要增加的偏移量
    GameObject steam;                              // 存储一个被冰冻的物体

    // 俯冲技能
    public void magic_sprint()
    {
		bossAnimator.SetBool ("beginDarkMagic", true);
		Instantiate (teleportBegin,  bossContainer.position + position,Quaternion.Euler(Vector3.zero));
		StartCoroutine(delayToSetFalse ("beginDarkMagic", 0.5f));
		darkMagic.transform.position = bossContainer.position;
		darkMagic.SetActive (true);
		sprint_stage = 0;
		stand = false;
        sprint = true;
        timer = 0;
        sprint_targetposition = position + up_position;
    }

	// 激光炮技能
	public void magic_laser()
	{
		//动画与特效部分
		bossAnimator.SetBool ("beginLightMagic", true);
		Instantiate (teleportBegin, bossContainer.position + position,Quaternion.Euler(Vector3.zero));
		StartCoroutine (delayToTeleportLightAnimateBegin (0.3f));
		StartCoroutine (delayToTeleportLightBegin (0.5f));
		StartCoroutine(delayToSetFalse ("beginLightMagic", 0.7f));
		//动画与特效部分结束
		stand = false;
		laser = true;
		timer = 0;
		int direction = 0;      // 1为前后, 2为左右
		// laser_position = Random.Range(0, 9);
		if (player.targetPosition == player_obj.transform.position)
		{   // 如果主角不在移动
			GameObject[] obj = new GameObject[2];   // 对应前、后

			// 检测主角被困的情况
			if (Physics.Raycast(player.targetPosition, new Vector3(0, 0, 1), out hit, 6.2f))
			{   // 前方
				obj[0] = hit.transform.gameObject;
			}
			else obj[0] = null;
			if (Physics.Raycast(player.targetPosition, new Vector3(0, 0, -1), out hit, 6.2f))
			{   // 后方
				obj[1] = hit.transform.gameObject;
			}
			else obj[1] = null;

			// 猜测主角可能的前进方向
			if ((obj[0] != null && (obj[0].tag == "Wall" || obj[0].tag.Substring(0, 4) == "Door" || obj[0].tag == "Wood"))
				&& (obj[1] != null && (obj[1].tag == "Wall" || obj[1].tag.Substring(0, 4) == "Door" || obj[1].tag == "Wood")))
			{   // 前后都被挡住了,激光炮使用左右方向
				direction = 2;
			}
			else  // 前后没有都被挡住，激光炮使用前后方向
				direction = 1;
		}
		else   // 如果主角在移动
		{
			float temp = player.targetPosition.x - player_obj.transform.position.x;
			if (temp <= 0.01 && temp >= -0.01)
			{   // 此时主角一定是前后移动
				direction = 1;
			}
			else
			{   // 此时主角一定是左右移动
				direction = 2;
			}
		}
		// 判断激光炮具体位置
		if (direction == 1)
		{   // 前后方向的激光炮
			if (player.targetPosition.x > 5)
				laser_position = 2;
			else if (player.targetPosition.x < -5)
				laser_position = 1;
			else
				laser_position = 0;
		}    
		else
		{   // 左右方向的激光炮
			if (Random.value < 0.5)
			{   // 使用左边
				if (player.targetPosition.z > 5)
					laser_position = 3;
				else if (player.targetPosition.z < -5)
					laser_position = 5;
				else
					laser_position = 4;
			}
			else 
			{   // 使用右边
				if (player.targetPosition.z > 5)
					laser_position = 6;
				else if (player.targetPosition.z < -5)
					laser_position = 8;
				else
					laser_position = 7;
			}
		}





		// print (laser_position);
		switch (laser_position)//激光炮位置
		{
		case 0:
			lightMagic.transform.position = bossContainer.position;
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			break;
		case 1:
			lightMagic.transform.position = bossContainer.position + new Vector3 (-10, 0, 0);
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			break;
		case 2:
			lightMagic.transform.position = bossContainer.position + new Vector3 (10, 0, 0);
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			break;
		case 3:
			lightMagic.transform.position = bossContainer.position + new Vector3 (0, 0, 10);
			lightMagic.transform.rotation = Quaternion.Euler (new Vector3 (0, -90, 0));
			break;
		case 4:
			lightMagic.transform.position = bossContainer.position;
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, -90, 0));
			break;
		case 5:
			lightMagic.transform.position = bossContainer.position + new Vector3 (0, 0, -10);
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, -90, 0));
			break;
		case 6:
			lightMagic.transform.position = bossContainer.position + new Vector3 (0, 0, 10);
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 90, 0));
			break;
		case 7:
			lightMagic.transform.position = bossContainer.position;
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 90, 0));
			break;
		case 8:
			lightMagic.transform.position = bossContainer.position + new Vector3 (0, 0,-10);
			lightMagic.transform.rotation = Quaternion.Euler(new Vector3 (0, 90, 0));
			break;
		}

		lightMagic.SetActive (true);
	}

    // 火球技能
    public void magic_fireball()
    {
		bossAnimator.SetBool ("beginMeteoMagic", true);
		StartCoroutine(delayToSetFalse ("beginMeteoMagic", 0.5f));
        // fireball = true;
        // stand = false;
        int num = 0;
        for (int i = 0; i < 9; i++)
        {
            if (Random.value <= (4.0 - num) / (9 - i))
            {

                fireball_position[num] = i;
                num++;
                if (num > 3)
                    break;
            }
        }
        // 将四个火球分别发射到fireball_positions[fireball_position[k]]位置，k为0-3.

        for (int i = 0; i < 4; i++)
        {
			Transform fireRock = meteoMagic.transform.FindChild ("fireRock" + (i+1).ToString ());
			fireRock.position = fireball_positions[fireball_position[i]] + bossContainer.position;
			StartCoroutine(delayFireBall (fireRock, i*0.5f));
			StartCoroutine(delayStayFire (fireRock, i*0.5f + 3.0f));
			StartCoroutine(delayFireBallOver (fireRock, i*0.5f + 4.0f));
            print(fireball_positions[fireball_position[i]]);
        }
    }

	public IEnumerator delayFireBall(Transform fireRock, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		fireRock.gameObject.SetActive(true);
	}
	public IEnumerator delayStayFire(Transform fireRock, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		Instantiate(fireStay, fireRock.position,Quaternion.Euler(Vector3.zero));
	}
	public IEnumerator delayFireBallOver(Transform fireRock, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		fireRock.gameObject.SetActive(false);
	}

    // 木墙技能
    public void magic_woodenwall()
    {
		bossAnimator.SetBool ("beginWoodMagic", true);
		StartCoroutine(delayToSetFalse ("beginWoodMagic", 0.5f));
		Vector3 center;
		if(player.floating ==0)
		{
			center = player.targetPosition - new Vector3(0,1.05f,0);

		}
		else
		{
			center = player.targetPosition - new Vector3(0,3.05f,0);

		}
		woodMagic.transform.position = bossContainer.position;
		woodMagic.SetActive (true);
        if (Physics.Raycast(center + new Vector3(0, 2, 0), new Vector3(1, 0, 0), out hit, raylength) && hit.transform.name != "Player")
        {   
            // 检测到物体说明有阻隔，不用加墙
            print(hit.transform.name);
        }
        else 
        {
            // 升起木墙
            //print("主角右侧应该升起木墙。");

			Instantiate(wall, center + woodwall_position[1], wall.transform.rotation);
        }
        if (Physics.Raycast(center + new Vector3(0, 2, 0), new Vector3(-1, 0, 0), out hit, raylength) && hit.transform.name != "Player")
        {   
            // 检测到物体说明有阻隔，不用加墙
            print(hit.transform.name);
        }
        else
        {
            // 升起木墙
            //print("主角左侧应该升起木墙。");

			Instantiate(wall, center + woodwall_position[0], wall.transform.rotation);
        }
        if (Physics.Raycast(center + new Vector3(0, 2, 0), new Vector3(0, 0, 1), out hit, raylength) && hit.transform.name != "Player")
        {   
            // 检测到物体说明有阻隔，不用加墙
            print(hit.transform.name);
        }
        else
        {
            // 升起木墙
            //print("主角前方应该升起木墙。");

			Instantiate(wall, center + woodwall_position[2], wood_q);
        }
        if (Physics.Raycast(center + new Vector3(0, 2, 0), new Vector3(0, 0, -1), out hit, raylength) && hit.transform.name != "Player")
        {   
            // 检测到物体说明有阻隔，不用加墙
            print(hit.transform.name);
        }
        else
        {
            // 升起木墙
            //print("主角后方应该升起木墙。");

			Instantiate(wall, center + woodwall_position[3], wood_q);
        }
    }


	// 致盲技能
	public void magic_blind()
	{
		bossAnimator.SetBool ("beginBlindMagic", true);
		StartCoroutine(delayToSetFalse ("beginBlindMagic", 0.5f));
		blindMagic.transform.position = bossContainer.position;
		blindMagic.SetActive (true);
	}

    // 减速技能
    public void magic_slow()
    {
		bossAnimator.SetBool ("beginTimeMagic", true);
		StartCoroutine(delayToSetFalse ("beginTimeMagic", 0.5f));
		timeMagic.transform.position = bossContainer.position;
		timeMagic.SetActive (true);
        if (slow == true)
        { }
        else
        { 
			player.smoothTime *= 2;
			player.move_smoothTime *= 2;
		}
        slow = true;
        slow_timer = 0;
    }

	// 封印楼层技能
	public void magic_seal()
	{
		bossAnimator.SetBool ("beginSealMagic", true);
		StartCoroutine(delayToSetFalse ("beginSealMagic", 0.5f));
		// 判断是否处于楼层间移动状态
		float difference;        // 差值
		difference = player.targetPosition.y - player_obj.transform.position.y;
		if (player.TG != null || difference > 2 || difference < -2) {   // 处于楼层间移动状态或者已经封印了则改为施放木墙魔法
			magic_woodenwall ();
			return;
		} else if (player.seal == true) {
			magic_laser ();
			return;
		}

		// 封印魔法的具体内容
		sealMagic.transform.position = bossContainer.position;
		sealMagic.SetActive (true);
		player.seal = true;
		seal_timer = 0;
		if (player.floating != 0)
			offset = new Vector3(0, player_obj.transform.position.y + player.steam_height - 2, 0);
		else
			offset = new Vector3(0, player_obj.transform.position.y + player.steam_height, 0);
		hits1 = Physics.RaycastAll(ray_start1 + offset, ray_forward, ray_length);
		hits2 = Physics.RaycastAll(ray_start2 + offset, ray_forward, ray_length);
		hits3 = Physics.RaycastAll(ray_start3 + offset, ray_forward, ray_length);
		int i;
		print("这层楼被隔离了！！");
		/* 此时可以播放动画 */
		steam = null;
		for (i = 0; i < hits1.Length; i++)
		{
			steam = hits1[i].transform.gameObject;
			steam.GetComponent<ParticleSystem>().Stop();        }
		for (i = 0; i < hits2.Length; i++)
		{
			steam = hits2[i].transform.gameObject;
			steam.GetComponent<ParticleSystem>().Stop();
		}
		for (i = 0; i < hits3.Length; i++)
		{
			steam = hits3[i].transform.gameObject;
			steam.GetComponent<ParticleSystem>().Stop();
		}
	}

	// Use this for initialization
	void Start () {
        // 初始化
		bossAnimator = transform.FindChild("boss_del").GetComponent<Animator>();
		wholeCam = GameObject.Find ("Camera");
		Cam = GameObject.Find("Main Camera");
        player_script = player_obj.GetComponent<player>();
        // 魔王基础
        position = new Vector3(0, 9, 18);
        time_float = 2;
        floating_time = 0.0f;
        floating_ratio = 2 * Mathf.PI / time_float;
        sprint_ratio = pi2 / time_down;
        //floating_offset = Vector3.zero;
        // 状态值
        stand = true;        
        sprint = false;
        laser = false;
        slow = false;
        sprint_stage = 0;
		bossContainer = transform.parent;
        // 激光炮发射可能位置
        laser_positions[0] = new Vector3(0, -5, 7);
		laser_positions[1] = new Vector3(-10, -5, 7);
        laser_positions[2] = new Vector3(10, 0, 0);
        laser_positions[3] = new Vector3(-26.5f, -8, -6.5f);
        laser_positions[4] = new Vector3(-26.5f, -8, -16.5f);
        laser_positions[5] = new Vector3(-26.5f, -8, -26.5f);
        laser_positions[6] = new Vector3(26.5f, -8, -6.5f);
        laser_positions[7] = new Vector3(26.5f, -8, -16.5f);
        laser_positions[8] = new Vector3(26.5f, -8, -26.5f);
        // 火球可能位置
        fireball_positions[0] = new Vector3(0, 0, 0);
        fireball_positions[1] = new Vector3(10, 0, 0);
        fireball_positions[2] = new Vector3(-10, 0, 0);
        fireball_positions[3] = new Vector3(0, 0, 10);
		fireball_positions[4] = new Vector3(0, 0, -10);
        fireball_positions[5] = new Vector3(10, 0, 10);
        fireball_positions[6] = new Vector3(10, 0, -10);
        fireball_positions[7] = new Vector3(-10, 0, 10);
        fireball_positions[8] = new Vector3(-10, 0, -10);
        // 木墙可能位置偏移
        woodwall_position[0] = new Vector3(-5, 0, 0);   // 左
        woodwall_position[1] = new Vector3(5, 0, 0);    // 右
        woodwall_position[2] = new Vector3(0, 0, 5);    // 前
        woodwall_position[3] = new Vector3(0, 0, -5);   // 后
	}
	
	// Update is called once per frame
	void Update () {
        // 上下浮动效果
        /*if (stand == true)
        {
            floating_time += Time.deltaTime;
            if (floating_time > time_float)
                floating_time = 0;
            floating_offset = floating_radius * Mathf.Sin(floating_time * floating_ratio);
            //transform.position = position + floating_offset;
			transform.localPosition = position + floating_offset;
        }*/


        // 俯冲冲击
        if (sprint == true)
        {
            timer += Time.deltaTime;
            switch (sprint_stage)
            {
                case 0: // 向上飞阶段
                    {
					    transform.localPosition = Vector3.SmoothDamp(transform.localPosition, sprint_targetposition, ref velocity, time_up);
					    if (transform.localPosition.y - sprint_targetposition.y > -0.0001)
                        {
						    transform.localPosition = sprint_targetposition;
                            timer = 0;
                            sprint_stage = 1;
                        }
                        break;
                    }
                case 1: // 蓄力阶段
                    {
                        if (timer > time_storage)
                        {
                            timer = 0;
                            sprint_stage = 2;
                            sprint_targetposition += down_position;
                        }
						break;
                    }
                case 2: // 向下冲击阶段
                    {
                        if (timer > time_down)
                        {
                            timer = time_down;
                            sprint_stage = 3;
                            // 玩家扣血
						if (Cam.GetComponent<Camera> ().enabled == true) {
							Cam.GetComponent<shakeCameraAnimate> ().shakeCamera (0.4f, 0.02f);//震动屏幕							
						} else {
							wholeCam.GetComponent<shakeCameraAnimate>().shakeCamera (0.4f,0.02f);//震动屏幕
						}


                            Vector3 player_p = player_obj.transform.position;
                            // 中心位置强制扣血
                            if (player_p.x>=-5 && player_p.x <= 5 && player_p.z >= -5 && player_p.z <= 5) 
                            { player_script.MustLossLife(20); }
                            // 非中心位置，如果悬浮，不扣血，否则强制扣血
                            else if (player.floating != 0)
                            { }
                            else { player_script.MustLossLife(5); }
                        }
					    transform.localPosition = sprint_targetposition + new Vector3(0, Mathf.Sin(timer * sprint_ratio + pi2) * 14.5f, 0);
                        
						break;
                    }
                case 3: // 硬直阶段
                    {
                        if (timer > time_stiff_sprint)
                        {
                            timer = 0;
						//动画与特效部分
						Instantiate (teleportEnd, bossContainer.position + position,Quaternion.Euler(Vector3.zero));
						StartCoroutine (delayToTeleportDark (position, 0.3f));
						StartCoroutine(delayToSetFalse ("returnDarkMagic", 0.4f));
						//动画与特效部分结束
                            sprint = false;
                            stand = true;
                        }
						break;
                    }
				default: break;
            }
        }
        else if (laser == true)
        {
            timer += Time.deltaTime;
            if (timer > time_laser)
            {
				//动画与特效部分
				Instantiate (teleportBegin, bossContainer.position + transform.localPosition,Quaternion.Euler(Vector3.zero));
				StartCoroutine (delayToTeleportLightAnimateEnd (0.3f));
				StartCoroutine (delayToTeleportLightBack (position, 0.5f));
				//动画与特效部分结束
                laser = false;
                stand = true;

            }
        }

        // 减速
        if (slow == true)
        {
            slow_timer += Time.deltaTime;
            if (slow_timer >= slow_time)
            {
                slow = false;
                player.smoothTime /= 2;
				player.move_smoothTime /= 2;
            }
        }

        // 封印
        if (player.seal == true)
        {
            seal_timer += Time.deltaTime;
            if (seal_timer >= seal_time)
            {
                player.seal = false;
                steam = null;
                int i;
                for (i = 0; i < hits1.Length; i++)
                {
                    steam = hits1[i].transform.gameObject;
					if(steam.transform.FindChild ("icefloor").gameObject.activeSelf == false)
                        steam.GetComponent<ParticleSystem>().Play();
                }
                for (i = 0; i < hits2.Length; i++)
                {
                    steam = hits2[i].transform.gameObject;
					if(steam.transform.FindChild ("icefloor").gameObject.activeSelf == false)
                        steam.GetComponent<ParticleSystem>().Play();
                }
                for (i = 0; i < hits3.Length; i++)
                {
                    steam = hits3[i].transform.gameObject;
					if(steam.transform.FindChild ("icefloor").gameObject.activeSelf == false)
                        steam.GetComponent<ParticleSystem>().Play();
                }
            }
        }

        // 砸地效果
        if (Input.GetKeyUp("z"))
        {

            magic_sprint();
        }
        // 激光炮
        else if (Input.GetKeyUp("x"))
        {

            magic_laser();
        }
        // 火球
        else if (Input.GetKeyUp("c"))
        {

            magic_fireball();
        }
        // 木墙
        else if (Input.GetKeyUp("b"))
        {

			magic_woodenwall ();
            /*if (player.floating == 0)
                magic_woodenwall(player.targetPosition - new Vector3(0, 1.05f, 0));
            else
                magic_woodenwall(player.targetPosition - new Vector3(0, 3.05f, 0));*/
        }
        // 减速
        else if (Input.GetKeyUp("m"))
        {

            magic_slow();

        }
        // 封印楼层
        else if (Input.GetKeyUp("n"))
        {

            magic_seal();
        }
		else if (Input.GetKeyUp("l"))
		{

			magic_blind();
		}
	}


	public IEnumerator delayToSetFalse(string magicName, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		bossAnimator.SetBool (magicName, false);
		print ("delay");
	}

	public IEnumerator delayToTeleportDark(Vector3 position, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		transform.localPosition = position;
		bossAnimator.SetBool ("returnDarkMagic", true);
	}

	public IEnumerator delayToTeleportLightBegin(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		transform.localPosition = position + laser_positions [laser_position];

		if (laser_position > 2 && laser_position < 6)
			transform.rotation = Quaternion.Euler(new Vector3 (0, -90, 0));
		else if (laser_position > 5 && laser_position < 9)
			transform.rotation = Quaternion.Euler(new Vector3 (0, 90, 0));
		else
			transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));

	}

	public IEnumerator delayToTeleportLightBack(Vector3 position, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		transform.localPosition = position;
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
		bossAnimator.SetBool ("returnLightMagic2", true);
	}

	public IEnumerator delayToTeleportLightAnimateEnd(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		Instantiate (teleportEnd, bossContainer.position + position,Quaternion.Euler(Vector3.zero));
	}

	public IEnumerator delayToTeleportLightAnimateBegin(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		Instantiate (teleportEnd, bossContainer.position + position + laser_positions [laser_position],Quaternion.Euler(Vector3.zero));

	}

	public void teleportFloor(Vector3 diff)
	{
		//
		Instantiate (teleportBegin, transform.position,Quaternion.Euler(Vector3.zero));
		StartCoroutine (delayToTeleportFloor(diff, 0.5f));
		StartCoroutine (delayToTeleportFloorAnimateEnd(diff, 0.2f));
		//

		// 中断所有和位移相关的技能
		sprint = false;
		sprint_stage = 0;
		laser = false;
		stand = true;
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
	}

	public IEnumerator delayToTeleportFloor(Vector3 diff, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		bossContainer.transform.position += diff;  
		transform.localPosition = position;
		bossAnimator.Play ("float");
	}

	public IEnumerator delayToTeleportFloorAnimateEnd(Vector3 diff, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		Instantiate (teleportEnd,bossContainer.transform.position + diff + position,Quaternion.Euler(Vector3.zero));
	}
}
