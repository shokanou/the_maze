using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System;
using System.IO;
using System.Collections.Generic;

public class player : MonoBehaviour {

	public UnityEngine.UI.Text text_floor;      // 层数
	public UnityEngine.UI.Text text_life;       // 生命
	public UnityEngine.UI.Text text_magic;      // 法力
	public UnityEngine.UI.Text text_key;        // 钥匙
	public GameObject text_win;                 // 胜利
	public GameObject text_dead;                // 死亡
	public GameObject Cam;    // 摄像机
	public GameObject WholeCam;    // 全景摄像机
	public GameObject bossContainer;    // Boss
	Boss bossScript;    // Boss
	public GameObject princess;    // Princess
	//public static int damageValue;//持续伤害值

	//人物动画部分
	Animator playerAnimator;
	//动画参数
	float trickStep = 0.04f;

	public GameObject lightBeam;    // 光束特效
	GameObject lightRing;
	public GameObject lightRing1;    // 光环特效1
	public GameObject lightRing2;    // 光环特效2
	GameObject gather;
	public GameObject gather1;       // 聚气特效1
	public GameObject gather2;       // 聚气特效2
	bool lightFlag = false;//控制光束只激活一次
	GameObject obj;//魔法阵
	GameObject doorG; //门的光效
	GameObject doorG2; //门的光效(另一面)
	Transform sparks;//光斑光效

	static int step = 10;      // 在这个迷宫中，移动一步的长度为3
	RaycastHit hit;           // 这个是碰撞检测的目标
	int jump;                 // 记录跳跃几层楼
	int floor;                // 记录当前层
	bool over;                // 是否通关,现在改为是否结束，可能是死亡导致结束
	float rayLenth = 6.2f;
	// 角色状态
	int life;                            // 生命值
	public int life_start = 10;          // 游戏开始时生命值
	int magic;                           // 法力值
	public int magic_start = 1;          // 游戏开始时法力值
	bool drop, invincible;               // 坠落、无敌（不受伤害），此类状态在关卡初始默认为无
	public static int floating;          // 浮空步数
	int key1, key2, key3;                // 红、黄、蓝钥匙的数量
	public bool playerAble = true;       //角色可以移动
	public static bool seal;             // 封印
	//public static bool hurt;             // 持续扣血状态
	//public static float hurt_time;       // 持续扣血时间

	// 游戏参数
	ArrayList Doors = new ArrayList();       // 记录哪些门消失了
	ArrayList Keys = new ArrayList();        // 记录哪些钥匙消失了
	Vector3 floatingheight = new Vector3(0,2,0);    // 悬浮高度
	public static float smoothTime = 0.1F;
	private Vector3 velocity = Vector3.zero;
	float smoothTimeCam = 0.3F;
	private Vector3 velocityCam= Vector3.zero;
	private Vector3 velocityWholeCam= Vector3.zero;
	public static Vector3 targetPosition;
	public static Transform TG;
	float transitSpeed = 0.025f;//介于0到1之间，最好能被0.2整除
	float transitTimeCurrent = 0;//一个计时用的中间变量

	Vector3 targetCamPositionNear = new Vector3(0,12.0f,-8.0f);//近景摄像机位置
	Vector3 camPosition = new Vector3(0,18.0f,-12.0f);//摄像机位置
	Vector3 camPositionStandard = new Vector3(0,18.0f,-12.0f);//摄像机位置标准

	Vector3 targetWholeCamPositionNear = new Vector3(0,-9,2);//景摄像机位置
	Vector3 wholeCamPosition = new Vector3(0,0,0);//摄像机位置
	Vector3 wholeCamPositionStandard = new Vector3(0,0,0);//摄像机位置标准
	bool trans = false;            // 在传送


    /* 平移相关 */
    string con = "";                               // 存放取出的指令
    bool move;                                     // 是否在移动
	bool move_rotate;                              // 是否要转身
	int move_direction;                            // 移动当前方向
	int move_target_dire;                          // 移动目标方向
	float rotate_time = 0.2f;                      // 旋转耗时
	public static float move_smoothTime = 0.5f;    // 移动耗时
	float move_timer;                              // 行走时的计数器
	float move_proportion;                         // 行动时的比例(旋转/平移)
	Vector3 start_position;                        // 移动起始位置
	/* 蒸汽吹飞相关 */
	public static float steam_height = 38;                            // 从下往上能检测到蒸汽的collider的距离
	int fly;                                             // 吹飞状态，暂定上升2s，下降1s
	float flytime;
	Vector3 flyup_start = new Vector3(24, 0, 0);         // 吹飞
	Vector3 flyup_start2 = new Vector3(23, 0, 0);
	Vector3 flyup_center;
	Vector3 flyup_end = new Vector3(0, 24, 0);
	Vector3 flyup_end2 = new Vector3(0, 23, 0);
	Vector3 dropdown_start = new Vector3(3, 0, 0);       // 吹飞后降落
	Vector3 dropdown_center;
	Vector3 dropdown_end = new Vector3(0, 3, 0);
	/* 冰冻魔法相关 */
	// 三个射线起点
	Vector3 ray_start1 = new Vector3(-15, 0, 10);
	Vector3 ray_start2 = new Vector3(-15, 0, 0);
	Vector3 ray_start3 = new Vector3(-15, 0, -10);
	Vector3 ray_forward = new Vector3(1, 0, 0);    // 射线方向
	float ray_length = 25f;                        // 射线长度
	RaycastHit[] hits1, hits2, hits3;              // 三个碰撞检测结果
	Vector3 offset;                                // 需要增加的偏移量
	bool ice = false;                              // 是否在冰冻魔法起作用时间
	GameObject steam;                              // 存储一个被冰冻的物体
	public GameObject ice_m;                       // 冰冻魔法影响范围
	/* 火焰魔法相关 */
	// 无


	public Animator doorController; //门的动画控制器


    public string portName = "COM3";
    public int baudRate = 9600;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;
    SerialPort sp = null;




    // 触发检测，捡道具，触碰陷阱
    void OnTriggerEnter(Collider collider)
	{
		//在传送过程中不进行响应
		if (trans == true)
			return;
		//触碰MP道具
		if (collider.tag == "MpPlus") 
		{
			magic++;
			text_magic.text = "法力：" + magic.ToString ();
			collider.gameObject.SetActive (false);
		}
		//触碰HP道具
		if (collider.tag == "HpPlus") {
			life += 2;
			text_life.text = "生命：" + life.ToString ();
			collider.gameObject.SetActive (false);
			print (trans);
		}
		// 捡到钥匙
		else if (collider.tag.Substring (0, 3) == "Key") {
			int n = int.Parse (collider.tag.Remove (0, 4));
			switch (n) {
			case 1:
				key1++;
				break;
			case 2:
				key2++;
				break;
			case 3:
				key3++;
				break;
			default:
				break;
			}
			Keys.Add (collider.gameObject);
			collider.gameObject.SetActive (false);
			text_key.text = "1:" + key1 + " 2:" + key2 + " 3:" + key3;
		}
		//触碰针刺陷阱
		else if (collider.tag == "spike") {
			Losslife (1);
			if (Cam.GetComponent<Camera> ().enabled == true) {
				Cam.GetComponent<shakeCameraAnimate> ().shakeCamera (0.4f, 0.01f);//震动屏幕
			} else {
				WholeCam.GetComponent<shakeCameraAnimate> ().shakeCamera (0.4f, 0.01f);//震动屏幕
			}
		}
		//雷电陷阱在另外的脚本中
		//触碰boss水晶
		else if (collider.tag == "bossKey") {
			collider.gameObject.SetActive (false);
			//
			//
			//进行判断
			//
			//
		}
	}

	void doorAnimation(GameObject obj)
	{
		doorG = obj.transform.FindChild("doorG").gameObject;
		doorG.SetActive(true);
		doorG2 = obj.transform.FindChild("doorG2").gameObject;
		doorG2.SetActive(true);
		if (obj.tag == "Door_1") {
			gather = gather1;
		}
		else if (obj.tag == "Door_2") {
			gather = gather2;
		}
		GameObject gatherCurrent = MonoBehaviour.Instantiate(gather, doorG.transform.position, Quaternion.identity) as GameObject;
		gatherCurrent.transform.parent = doorG.transform;
		StartCoroutine(delayToOpenDoor(obj, 0.8f));
		playerAble = false;//角色操作禁用
		StartCoroutine(delayToActivePlayer(2.1f));//等待一定时间后激活角色

	}

	// 开门
	void Openthedoor(string door, ref int key)
	{
		if (key > 0)
		{
			if (Physics.Raycast(targetPosition, new Vector3(-1, 0, 0), out hit, rayLenth))
			{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == door)
				{
					Doors.Add(obj);
					//obj.SetActive(false);
					doorAnimation(obj);//门的动画相关函数
					key--;
				}
				else { }
			}
			if (Physics.Raycast(targetPosition, new Vector3(1, 0, 0), out hit, rayLenth))
			{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == door)
				{
					Doors.Add(obj);
					//obj.SetActive(false);
					doorAnimation(obj);
					key--;
				}
				else { }
			}
			if (Physics.Raycast(targetPosition, new Vector3(0, 0, 1), out hit, rayLenth))
			{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == door)
				{
					Doors.Add(obj);
					//obj.SetActive(false);
					doorAnimation(obj);
					key--;
				}
				else { }
			}
			if (Physics.Raycast(targetPosition, new Vector3(0, 0, -1), out hit, rayLenth))
			{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == door)
				{
					Doors.Add(obj);
					//obj.SetActive(false);
					doorAnimation(obj);
					key--;
				}
				else { }
			}
		}
		else
			print("你想空手套白狼？");
		text_key.text = "1:" + key1 + " 2:" + key2 + " 3:" + key3;
	}

	// 生命扣除
	public void Losslife(int damage)
	{
		if(invincible == true)
		{ 
			print("哈哈哈，我是无敌的！");
			invincible = false;
		}
		else
		{
			life = life - damage;
			if (life < 1)
			{
				over = true;
				text_life.text = "生命：0";
				text_dead.SetActive(true);
			}
			else
			{ text_life.text = "生命：" + life.ToString(); }
		}
	}
	// 强制生命扣除,破盾
	public void MustLossLife(int damage)
	{
		invincible = false;
		life = life - damage;
		if (life < 1)
		{
			over = true;
			text_life.text = "生命：0";
			text_dead.SetActive(true);
		}
		else
		{ text_life.text = "生命：" + life.ToString(); }
	}
	// 雷电陷阱的扣血问题
	public void ThunderLossLife(int damage)
	{
		if (invincible == false)
		{
			life = life - damage;
			if (life < 1)
			{
				over = true;
				text_life.text = "生命：0";
				text_dead.SetActive(true);
			}
			else
			{ text_life.text = "生命：" + life.ToString(); }
			if (Cam.GetComponent<Camera>().enabled == true)
			{
				Cam.GetComponent<shakeCameraAnimate>().shakeCamera(0.4f, 0.01f);     //震动屏幕
			}
			else
			{
				WholeCam.GetComponent<shakeCameraAnimate>().shakeCamera(0.4f, 0.01f);//震动屏幕
			}
		}
		else { }
	}

	// 关于悬浮
	void Floatonthefloor()
	{
		if (floating == 0)
		{ return; }
		else
		{
			if (floating == 1)
			{
				targetPosition = targetPosition - floatingheight;
				//人物动画部分
				playerAnimator.SetBool("endWindMagic",true);
				StartCoroutine (delayToSetFalse ("endWindMagic", 0.2f));
				playerAnimator.SetBool("inWindMagic",false);
			}
			floating--;
		}
	}

	// 初始化函数
	void Init()
	{
		//人物动画部分
		playerAnimator = transform.FindChild("princess_del").GetComponent<Animator>();
		bossScript = bossContainer.transform.FindChild ("boss").GetComponent<Boss> ();
		// 重启主角状态

		life = life_start;
		magic = magic_start;
		floor = 1;
		drop = invincible = false;
		floating = 0;
		fly = 0;
		trans = false; 
		seal = false;
		move = false;
		move_rotate = false;
		move_direction = 0;
		//		hurt = false;
		// 重启界面状态
		over = false;
		ice = false;
		text_life.text = "生命：" + life.ToString();
		text_magic.text = "法力：" + magic.ToString();
		text_floor.text = "当前层数：1层";
		text_dead.SetActive(false);
		text_win.SetActive(false);
		key1 = key2 = key3 = 0;
		text_key.text = "1:" + key1 + " 2:" + key2 + " 3:" + key3;
		// 初始化门
		if (Doors.Count > 0)
		{
			foreach(GameObject door in Doors)
			{
				//door.SetActive(true);
				door.transform.GetComponent<Animator>().SetBool("doorOpen",false);
				door.transform.GetComponent<Animator>().SetBool("isRestart",true);//复原状态动画
				door.transform.GetComponent<BoxCollider>().enabled = true;//复原碰撞器
			}
			Doors.Clear();
		}
		// 初始化钥匙
		if (Keys.Count > 0)
		{
			foreach (GameObject key in Keys)
			{
				key.SetActive(true);
			}
			Doors.Clear();
		}
		targetPosition = transform.position;
		sparks = transform.FindChild("Sparks");
		sparks.GetComponent<ParticleSystem> ().Stop ();
	}

	// Use this for initialization
	void Start () {
		Init();
        OpenPort();
        StartCoroutine(DataReceiveFunction());
    }

	// Update is called once per frame
	void Update () {

		playerAnimator.SetFloat("trickTime", playerAnimator.GetFloat("trickTime")+trickStep);
		if (playerAnimator.GetFloat ("trickTime") > 10.1f) {
			playerAnimator.SetFloat("trickTime", 0);
		}


		// 冰冻时间
		if (ice == true)
		{
			if (steam.GetComponent<ParticleSystem>().isStopped)
			{
				ice = false;
			}
			return;
		}

		// 检测属于吹飞状态的哪一步
		switch (fly)
		{
		case 0: {
				if (move == false)
				{   // 掉落、传送等
					transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime); break;
				}
				else {   // 平移
					if (move_rotate == true) // 先旋转
					{
						move_timer += Time.deltaTime;
						move_proportion = move_timer / rotate_time;
						if (move_proportion >= 1)
						{
							move_timer = 0;
							move_proportion = 1.0f;
							move_rotate = false;
							if (move_target_dire == -180)
								move_target_dire = 180;
							move_direction = move_target_dire;
						}
						princess.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Lerp(move_direction, move_target_dire, move_proportion), 0));
					}
					else
					{
						move_timer += Time.deltaTime;
						move_proportion = move_timer / move_smoothTime;
						if (move_proportion >= 1)
						{
							//人物行走动画部分
							playerAnimator.SetBool("beginWalk",false);

							move_proportion = 1.0f;
							move = false;
						}
						transform.position = Vector3.Lerp(start_position, targetPosition, move_proportion);
					}
				}
				break;
			}
		case 1:
			{
				flytime += Time.deltaTime + Time.deltaTime;
				if (flytime > 1)
				{
					flytime = 1;
					fly = 0;
					floating = 1;
					playerAble = true;
				}
				transform.position = dropdown_center - new Vector3(0, Vector3.Slerp(dropdown_start, dropdown_end, flytime).y, 0);
				return;
			}
		case 2:
			{
				flytime += Time.deltaTime + Time.deltaTime;
				if (flytime > 1)
				{
					flytime = 1;
					fly = 1;
				}
				transform.position = dropdown_center + new Vector3(0, Vector3.Slerp(dropdown_end, dropdown_start, flytime).y, 0);
				if (flytime == 1)
				{
					flytime = 0;
				}
				return;
			}
		case 3:
			{
				flytime += Time.deltaTime;
				if (flytime > 1)
				{
					flytime = 1;
					fly = 2;
				}
				if(floating == 0)
					transform.position = flyup_center + new Vector3(0, Vector3.Slerp(flyup_start, flyup_end, flytime).y, 0);
				else
					transform.position = flyup_center + new Vector3(0, Vector3.Slerp(flyup_start2, flyup_end2, flytime).y, 0);
				if (flytime == 1)
				{
					dropdown_center = transform.position - dropdown_end;
					flytime = 0;
					floor++;
					text_floor.text = "当前层数：" + floor.ToString() + "层";
				}
				return;
			}
		case 4:
			{
				flytime += Time.deltaTime;
				if (flytime > 1)
				{
					flytime = 1;
					fly = 3;
				}
				if(floating == 0)
					transform.position = flyup_center - new Vector3(0, Vector3.Slerp(flyup_end, flyup_start, flytime).y, 0);
				else
					transform.position = flyup_center - new Vector3(0, Vector3.Slerp(flyup_end2, flyup_start2, flytime).y, 0);
				if (flytime == 1)
				{
					flytime = 0;
				}
				return;
			}
		}

		if (Input.GetKeyDown("r") && playerAble)
		{
			SceneManager.LoadScene("boss");
			//Init();
			return;
		}
		else if (move == false && transform.position.x - targetPosition.x < 0.05f && transform.position.x - targetPosition.x > -0.05f &&
			transform.position.y - targetPosition.y < 0.05f && transform.position.y - targetPosition.y > -0.05f &&
			transform.position.z - targetPosition.z < 0.05f && transform.position.z - targetPosition.z > -0.05f )
		{
            // 如果没有未执行的操作，读取一个新的操作
            if (playerAble && con == "" && control.list.Count != 0)
            {
                con = control.list[0].ToString();
                control.list.RemoveAt(0);
                print(con);      // 这句用于检验，实际使用时删除
            }
            else
            { }

            trans = false;
			transform.position = targetPosition;
			if(over == true)
			{
				return;
			}
			if (drop == true)
			{
				drop = false;
				floor -= 1;
				text_floor.text = "当前层数：" + floor.ToString() + "层";
				// 生命值变化
				if (Physics.Raycast (transform.position, new Vector3 (0, -1, 0), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					GameObject obj = hit.transform.gameObject;
					print (obj.tag);
					if (obj.tag != "Steam")
						Losslife (1);
				}
				else {
					Losslife (1);
				}
			}
			if (Input.GetKeyUp ("1") && playerAble) {
				Openthedoor ("Door_1", ref key1);
			} else if (Input.GetKeyUp ("2") && playerAble) {
				Openthedoor ("Door_2", ref key2);
			} else if (Input.GetKeyUp ("3") && playerAble) {
				Openthedoor ("Door_3", ref key3);
			}
			// 使用无敌魔法
			else if (Input.GetKeyUp ("v") && invincible == false && playerAble) {
				if (magic > 0) {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					invincible = true;
					transform.FindChild ("EarthMagic").gameObject.SetActive (true);

					//人物动画部分
					playerAnimator.SetBool("beginEarthMagic",true);
					StartCoroutine (delayToSetFalse ("beginEarthMagic", 0.2f));

					print ("开启无敌");
					magic--;
					text_magic.text = "法力：" + magic.ToString ();
				}
			}			
			// 使用漂浮魔法
			else if (Input.GetKeyUp ("f") && playerAble) {
				if (floating <= 0) {
					if (magic > 0) {

						playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
						magic--;
						text_magic.text = "法力：" + magic.ToString ();
						floating = 2;
						print ("开启悬浮状态");
						transform.FindChild ("WindMagic").gameObject.SetActive (true);

						//人物动画部分
						playerAnimator.SetBool("beginWindMagic",true);
						StartCoroutine (delayToSetFalse ("beginWindMagic", 0.2f));
						playerAnimator.SetBool("inWindMagic",true);

						targetPosition = transform.position + floatingheight;
					}
				} else {
					magic--;
					text_magic.text = "法力：" + magic.ToString ();
					floating = 2;
					print ("开启悬浮状态2");
					transform.FindChild ("WindMagic").gameObject.SetActive (true);

					//人物动画部分
					playerAnimator.SetBool("beginWindMagic",true);
					StartCoroutine (delayToSetFalse ("beginWindMagic", 0.2f));
					playerAnimator.SetBool("inWindMagic",true);
				}


			}
			// 使用冰冻魔法
			else if (Input.GetKeyUp ("i") && playerAble) {
				if (magic > 0) {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					magic--;
					text_magic.text = "法力：" + magic.ToString ();
					if (floating != 0)
						offset = new Vector3 (0, transform.position.y + steam_height - 2, 0);
					else
						offset = new Vector3 (0, transform.position.y + steam_height, 0);
					hits1 = Physics.RaycastAll (ray_start1 + offset, ray_forward, ray_length);
					hits2 = Physics.RaycastAll (ray_start2 + offset, ray_forward, ray_length);
					hits3 = Physics.RaycastAll (ray_start3 + offset, ray_forward, ray_length);
					int i;
					print ("冰冻魔法开启了");
					ice_m.transform.position = new Vector3 (0, 40 * floor - 38.5f, 0);
					transform.FindChild ("IceMagic").gameObject.SetActive (true);

					//人物动画部分
					playerAnimator.SetBool("beginIceMagic",true);
					StartCoroutine (delayToSetFalse ("beginIceMagic", 0.2f));

					/* 此时可以播放动画 */
					steam = null;
					for (i = 0; i < hits1.Length; i++) {
						steam = hits1 [i].transform.gameObject;
						steam.GetComponent<ParticleSystem> ().Stop ();
						//此处增加显示相应冰冻层     
						steam.transform.FindChild ("icefloor").gameObject.SetActive (true);
					}
					for (i = 0; i < hits2.Length; i++) {
						steam = hits2 [i].transform.gameObject;
						steam.GetComponent<ParticleSystem> ().Stop ();
						//此处增加显示相应冰冻层
						steam.transform.FindChild ("icefloor").gameObject.SetActive (true);
					}
					for (i = 0; i < hits3.Length; i++) {
						steam = hits3 [i].transform.gameObject;
						steam.GetComponent<ParticleSystem> ().Stop ();
						//此处增加显示相应冰冻层
						steam.transform.FindChild ("icefloor").gameObject.SetActive (true);
					}
					if (steam != null)
						ice = true;
				}
			}
			// 使用火焰魔法
			else if (Input.GetKeyUp ("o") && playerAble) {
				if (magic > 0) {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					magic--;
					text_magic.text = "法力：" + magic.ToString ();
					print ("让烈焰净化世界吧！");
					transform.FindChild ("FireMagic").gameObject.SetActive (true);

					//人物动画部分
					playerAnimator.SetBool("beginFireMagic",true);
					StartCoroutine (delayToSetFalse ("beginFireMagic", 0.2f));

					// 融化冰块，注意，在漂浮时射线会达到上层的地板     
					float temp_height;    // 消除浮动带来的影响
					if (floating != 0)
						temp_height = steam_height - 2;
					else
						temp_height = steam_height;
					if (seal == false && Physics.Raycast (targetPosition, new Vector3 (0, 1, 0), out hit, temp_height)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
						GameObject obj = hit.transform.gameObject;
						obj.transform.FindChild ("icefloor").GetComponent<melt> ().enabled = true;
						// 此处加冰块融化动画（如果有的话）以及消除冰块
						StartCoroutine (delayToFly (obj, 2.0f));// 既然能站上来，此时粒子一定已经停止
					}
					// 如果四周有木头就烧掉，物品烧毁的动画跟在"SetActive(false);"后面。
					if (Physics.Raycast (targetPosition, new Vector3 (-1, 0, 0), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
						GameObject obj = hit.transform.gameObject;
						if (obj.tag == "Wood") {
							obj.GetComponent<woodFire> ().enabled = true;
							//obj.SetActive(false);
						} 
					}
					if (Physics.Raycast (targetPosition, new Vector3 (1, 0, 0), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
						GameObject obj = hit.transform.gameObject;
						if (obj.tag == "Wood") {
							obj.GetComponent<woodFire> ().enabled = true;
							//obj.SetActive(false);
						}
					}
					if (Physics.Raycast (targetPosition, new Vector3 (0, 0, 1), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
						GameObject obj = hit.transform.gameObject;
						if (obj.tag == "Wood") {
							obj.GetComponent<woodFire> ().enabled = true;
							//obj.SetActive(false);
						}
					}
					if (Physics.Raycast (targetPosition, new Vector3 (0, 0, -1), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
						GameObject obj = hit.transform.gameObject;
						if (obj.tag == "Wood") {
							obj.GetComponent<woodFire> ().enabled = true;
							//obj.SetActive(false);
						}
					}
				}
			}
			// 四方向移动
			else if (playerAble && (Input.GetKeyUp("a") || con == "a"))
            {
				print ("A");
				if (Physics.Raycast (targetPosition, new Vector3 (-1, 0, 0), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					GameObject obj = hit.transform.gameObject;
					if (obj.tag == "Wall" || obj.tag.Substring (0, 4) == "Door" || obj.tag == "Wood") {
						print ("不能撞墙");
					} else {

						playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
						targetPosition = transform.position + (new Vector3 (-step, 0, 0));
						move = true;
						start_position = transform.position;
						if (move_direction != -90)
						{
							if (move_direction == 180)
								move_direction = -180;
							move_target_dire = -90;
							move_rotate = true;
						}
						move_timer = 0;
						Floatonthefloor ();
						//人物行走动画部分
						playerAnimator.SetBool("beginWalk",true);
                        con = "";
                    }
				} else {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					targetPosition = transform.position + (new Vector3 (-step, 0, 0));
					move = true;
					start_position = transform.position;
					if (move_direction != -90)
					{
						if (move_direction == 180)
							move_direction = -180;
						move_target_dire = -90;
						move_rotate = true;
					}
					move_timer = 0;
					Floatonthefloor ();
					//人物行走动画部分
					playerAnimator.SetBool("beginWalk",true);
                    con = "";
                }
			} else if (playerAble && (Input.GetKeyUp("s") || con == "s"))
            {
				print ("S");
				if (Physics.Raycast (targetPosition, new Vector3 (0, 0, -1), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					GameObject obj = hit.transform.gameObject;
					if (obj.tag == "Wall" || obj.tag.Substring (0, 4) == "Door" || obj.tag == "Wood") {
						print ("不能撞墙");
					} else {

						playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
						targetPosition = transform.position + (new Vector3 (0, 0, -step));
						move = true;
						start_position = transform.position;
						move_timer = 0;
						if (move_direction != 180)
						{
							if (move_direction == -90)
								move_target_dire = -180;
							else
								move_target_dire = 180;
							move_rotate = true;
						}
						Floatonthefloor ();
						//人物行走动画部分
						playerAnimator.SetBool("beginWalk",true);
                        con = "";
                    }
				} else {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					targetPosition = transform.position + (new Vector3 (0, 0, -step));
					move = true;
					start_position = transform.position;
					move_timer = 0;
					if (move_direction != 180)
					{
						if (move_direction == -90)
							move_target_dire = -180;
						else
							move_target_dire = 180;
						move_rotate = true;
					}
					Floatonthefloor ();
					//人物行走动画部分
					playerAnimator.SetBool("beginWalk",true);
                    con = "";
                }
			} else if (playerAble && (Input.GetKeyUp("d") || con == "d"))
            {
				print ("D");
				if (Physics.Raycast (targetPosition, new Vector3 (1, 0, 0), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					obj = hit.transform.gameObject;
					if (obj.tag == "Wall" || obj.tag.Substring (0, 4) == "Door" || obj.tag == "Wood") {
						print ("不能撞墙");
					} else {

						playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
						targetPosition = transform.position + (new Vector3 (step, 0, 0));
						move = true;
						start_position = transform.position;
						move_timer = 0;
						if (move_direction != 90)
						{
							move_target_dire = 90;
							move_rotate = true;
						}
						Floatonthefloor ();
						//人物行走动画部分
						playerAnimator.SetBool("beginWalk",true);
                        con = "";
                    }
				} else {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					targetPosition = transform.position + (new Vector3 (step, 0, 0));
					move = true;
					start_position = transform.position;
					move_timer = 0;
					if (move_direction != 90)
					{
						move_target_dire = 90;
						move_rotate = true;
					}
					Floatonthefloor ();
					//人物行走动画部分
					playerAnimator.SetBool("beginWalk",true);
                    con = "";
                }
			} else if (playerAble && (Input.GetKeyUp("w") || con == "w"))
            {
				print ("W");
				if (Physics.Raycast (targetPosition, new Vector3 (0, 0, 1), out hit, rayLenth)) {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					GameObject obj = hit.transform.gameObject;
					if (obj.tag == "Wall" || obj.tag.Substring (0, 4) == "Door" || obj.tag == "Wood") {
						print ("不能撞墙");
					} else {

						playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
						targetPosition = transform.position + (new Vector3 (0, 0, step));
						move = true;
						start_position = transform.position;
						move_timer = 0;
						if (move_direction != 0)
						{
							move_target_dire = 0;
							move_rotate = true;
						}
						Floatonthefloor ();
						//人物行走动画部分
						playerAnimator.SetBool("beginWalk",true);
                        con = "";
                    }
				} else {

					playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
					targetPosition = transform.position + (new Vector3 (0, 0, step));
					move = true;
					start_position = transform.position;
					move_timer = 0;
					if (move_direction != 0)
					{
						move_target_dire = 0;
						move_rotate = true;
					}
					Floatonthefloor ();
					//人物行走动画部分
					playerAnimator.SetBool("beginWalk",true);
                    con = "";
                }
			} 
			//切换摄像机
			else if (Input.GetKeyUp ("e") && playerAble) 
			{
				if (Cam.GetComponent<Camera> ().enabled == true) {
					WholeCam.GetComponent<Camera> ().enabled = true;
					Cam.GetComponent<Camera> ().enabled = false;
				} else 
				{
					Cam.GetComponent<Camera> ().enabled = true;
					WholeCam.GetComponent<Camera> ().enabled = false;
				}


			}


			else if (Input.GetKeyUp("space")&&playerAble && seal == false) // 开启魔法阵
			{
				if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, rayLenth))
				{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
					GameObject obj = hit.transform.gameObject;
					if (obj.tag == "Magic")
					{
						if (floating > 0) {
							print ("你够不到魔法阵");
						}  
						else 
						{
							playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
							print ("开始传送");

							TG = obj.transform.FindChild ("TG");//获取魔法阵光效
							lightFlag = true;


							/*           int temp = 40 * (int.Parse(obj.name.Remove(0, 6)) - floor);
                       // targetPosition = transform.position + new Vector3(0, temp, 0);         // 移动主角
                        //Cam.transform.localPosition = Cam.transform.localPosition + new Vector3(0, temp, 0); // 移动摄像机
                        //print(obj.name + "  " + obj.name.Remove(0, 5));
                        floor = int.Parse(obj.name.Remove(0, 6));
                        text_floor.text = "当前层数：" + floor.ToString() + "层";
                        // 以下这一段激活出口的代码在实际游戏中大概不会有或者说要改位置加条件

这部分我移动到了下面
              */
						}
					}
					else
					{
						print("未检测到可用魔法阵");
					}
				}
				else
				{
					print("未检测到可用魔法阵");
				}
			}
			// 检测水蒸气,能碰到就有蒸汽
			else if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out hit, steam_height) && seal == false)
			{   // 检测蒸汽是否开启
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == "Steam" && obj.GetComponent<ParticleSystem>().isPlaying)
				{
					print("准备起飞");
					bossScript.teleportFloor(new Vector3 (0, 40, 0));          //移动Boss
					fly = 4;
					flytime = 0;
					if (floating == 0) {
						targetPosition += new Vector3 (0, 42, 0);
						flyup_center = transform.position + flyup_end;
						print (flyup_center);
					} 
					else {
						targetPosition += new Vector3 (0, 40, 0);
						flyup_center = transform.position + flyup_end - new Vector3 (0, 1, 0);
						print (flyup_center);
					}
					playerAble = false;
				}
			}
			// 检测脚下是否有洞
			else if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, rayLenth) && seal == false)
			{   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
				GameObject obj = hit.transform.gameObject;
				if (obj.tag == "Hole" || (obj.tag == "Steam" && obj.GetComponent<ParticleSystem>().isStopped))
				{
					if (floating > 0) {
						print ("漂浮状态怎么可能掉下去");
					} else {
						print ("掉下去了");
						drop = true;
						targetPosition = transform.position + new Vector3 (0, -40, 0);         // 移动主角
						bossScript.teleportFloor(new Vector3 (0, -40, 0));          //移动Boss
						// Cam.transform.localPosition = Cam.transform.localPosition + new Vector3(0, -15, 0); // 移动摄像机
						//floor -= 1;
						text_floor.text = "当前层数：" + floor.ToString () + "层";
					}
				}
				else if (obj.tag == "Out")  // 检测是否到达出口
				{
					text_win.SetActive(true);
					over = true;
				}
				else if (obj.tag == "Steam") 
				{
					if (floating == 0) {
						floating = 1;
						targetPosition = targetPosition + new Vector3 (0, 2, 0);
					} else { }
				}
			}
			else
			{ }

		}

		if (TG != null) {//魔法阵被激活
			camPosition = targetCamPositionNear;
			wholeCamPosition = targetWholeCamPositionNear;
			playerAble = false;
			TG.GetComponent <Renderer> ().material.color = Color.Lerp (new Color (1, 1, 1, 0), new Color (2, 2, 2, 1), transitTimeCurrent);
			transitTimeCurrent += transitSpeed;
			if (transitTimeCurrent >= 0.2f) {
				sparks.GetComponent<ParticleSystem> ().Play ();

			}
			if (transitTimeCurrent >= 0.9f&&lightFlag) {
				GameObject lightBeamCurrent = MonoBehaviour.Instantiate(lightBeam, sparks.position, Quaternion.identity) as GameObject;
				lightBeamCurrent.transform.parent =sparks;
				lightFlag = false;
			}
			if (transitTimeCurrent >= 2.0f) {

				transitTimeCurrent = 0;
				int temp = 40 * (int.Parse (TG.parent.name.Remove (0, 6)) - floor);
				targetPosition = transform.position + new Vector3 (0, temp, 0);         // 移动主角
				bossScript.teleportFloor(new Vector3 (0, temp, 0));            //移动Boss
				trans = true;//传送中
				floor = int.Parse (TG.parent.name.Remove (0, 6));
				text_floor.text = "当前层数：" + floor.ToString () + "层";
				// 以下这一段激活出口的代码在实际游戏中大概不会有或者说要改位置加条件

				sparks.GetComponent<ParticleSystem> ().Stop ();
				StartCoroutine (delayToDisppear (TG, 1.0f));
				TG = null;
				camPosition = camPositionStandard;
				wholeCamPosition = wholeCamPositionStandard;
				playerAble = true;

			}
		} 
		else {

		}

		Cam.transform.localPosition = Vector3.SmoothDamp(Cam.transform.localPosition, camPosition, ref velocityCam, smoothTimeCam);
		WholeCam.transform.localPosition = Vector3.SmoothDamp(WholeCam.transform.localPosition, wholeCamPosition, ref velocityWholeCam, smoothTimeCam);
	}

	public static IEnumerator delayToDisppear(Transform TGcurrent, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		TGcurrent.GetComponent <Renderer> ().material.color = new Color(1,1,1,0);
	}

	public IEnumerator delayToActivePlayer(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		playerAble = true;

	}

	public IEnumerator delayToOpenDoor(GameObject obj, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		obj.transform.GetComponent<Animator>().SetBool("isRestart",false);//防止动画循环
		obj.transform.GetComponent<Animator>().SetBool("doorOpen",true);//开门动画
		obj.transform.GetComponent<BoxCollider>().enabled = false;//取消碰撞器

		if (obj.tag == "Door_1") {
			lightRing = lightRing1;
		}
		else if (obj.tag == "Door_2") {
			lightRing = lightRing2;
		}

		GameObject lightRing1Current = MonoBehaviour.Instantiate(lightRing, doorG.transform.position, Quaternion.Euler(90.0f,obj.transform.rotation.eulerAngles.y,0)) as GameObject;
		lightRing1Current.transform.parent = doorG.transform;
		GameObject lightRing1Current2 = MonoBehaviour.Instantiate(lightRing, doorG2.transform.position, Quaternion.Euler(90.0f,obj.transform.rotation.eulerAngles.y,0)) as GameObject;
		lightRing1Current2.transform.parent = doorG2.transform;
	}

	public IEnumerator delayToFly(GameObject obj, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		if(seal == false)
			obj.GetComponent<ParticleSystem>().Play();
	}


	//人物动画部分
	public IEnumerator delayToSetFalse(string magicName, float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		playerAnimator.SetBool (magicName, false);
		print ("delay");
	}

    public void OpenPort()
    {
        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        //sp.ReadTimeout = 400;
        try
        {
            sp.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    //关闭串口
    public void ClosePort()
    {
        try
        {
            sp.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    /*
    若串口数据速度慢，数量小，可以在Update函数中使用sp.ReadByte等函数，该协程可以不用调用
    若串口数据速度较快，数量较多，就调用该协程（该程序就是这一种，在Start函数中已经被调用）
    若串口数据速度非常快，数量非常多，建议使用Thread
    */
    IEnumerator DataReceiveFunction()
    {
        //byte[] dataBytes = new byte[128];//存储数据
        int bytesToRead = 0;//记录获取的数据长度
        int count = 0;
        int state = 0;
        int laststate = 0;
        int bstate1 = 0;
        int lastbstate1 = 0;
        int bstate2 = 0;
        int lastbstate2 = 0;
        int bstate3 = 0;
        int lastbstate3 = 0;
        int kaiguan = 0;
        string a = string.Empty;
        string str = string.Empty;
        int x = 0, y = 0, z = 0;
        int x1 = 335, y1 = 350, z1 = 365;
        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    //通过read函数获取串口数据
                    byte[] dataBytes = new byte[1];
                    bytesToRead = sp.Read(dataBytes, 0, dataBytes.Length);
                    //Debug.Log(bytesToRead);
                    str = System.Text.Encoding.Default.GetString(dataBytes);
                    //Debug.Log(str);
                    //Debug.Log("aaa");
                    //串口数据已经被存入dataBytes中
                    //往下进行自己的数据处理
                    //比如将你的数据显示出来，可以利用guiText.text = ....之类
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                if (str == "1" || str == "2" || str == "3" || str == "4" || str == "5" || str == "6" || str == "7" || str == "8" || str == "9" || str == "0")
                {

                    a = a + str;
                }
                if (a.Length == 13)
                {
                    //Debug.Log(a);
                    string str2 = a.Substring(0, 4);
                    string str3 = a.Substring(4, 9);
                    //Debug.Log(str3);

                    int i1 = Convert.ToInt32(str3);
                    z = i1 % 1000;
                    i1 = (i1 - z) / 1000;
                    y = i1 % 1000;
                    i1 = (i1 - y) / 1000;
                    x = i1 % 1000;
                    //i = (i - x) / 1000;
                    //Debug.Log(z);
                    int j = Convert.ToInt32(str2);
                    int b1 = j / 1000;
                    int b2 = j % 10;
                    j = (j - b2) / 10;
                    int b3 = j % 10;
                    lastbstate1 = bstate1;
                    bstate1 = b1;
                    lastbstate2 = bstate2;
                    bstate2 = b2;
                    lastbstate3 = bstate3;
                    bstate3 = b3;
                    laststate = state;
                    if (bstate1 == 1 && lastbstate1 == 0)
                    {
                        kaiguan = 1 - kaiguan;
                    }
                    if (bstate2 == 1 && lastbstate2 == 0 && playerAble)
                    {                      
                            Openthedoor("Door_1", ref key1);
                            Openthedoor("Door_2", ref key2);
                            Openthedoor("Door_3", ref key3);
                    }
                    if (bstate3 == 1 && lastbstate3 == 0 && playerAble && seal == false)
                    {
                         // 开启魔法阵                
                            if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, rayLenth))
                            {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                GameObject obj = hit.transform.gameObject;
                                if (obj.tag == "Magic")
                                {
                                    if (floating > 0)
                                    {
                                        print("你够不到魔法阵");
                                    }
                                    else
                                    {
                                        playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
                                        print("开始传送");

                                        TG = obj.transform.FindChild("TG");//获取魔法阵光效
                                        lightFlag = true;


                                        /*           int temp = 40 * (int.Parse(obj.name.Remove(0, 6)) - floor);
                                   // targetPosition = transform.position + new Vector3(0, temp, 0);         // 移动主角
                                    //Cam.transform.localPosition = Cam.transform.localPosition + new Vector3(0, temp, 0); // 移动摄像机
                                    //print(obj.name + "  " + obj.name.Remove(0, 5));
                                    floor = int.Parse(obj.name.Remove(0, 6));
                                    text_floor.text = "当前层数：" + floor.ToString() + "层";
                                    // 以下这一段激活出口的代码在实际游戏中大概不会有或者说要改位置加条件

            这部分我移动到了下面
                          */
                                    }
                                }
                                else
                                {
                                    print("未检测到可用魔法阵");
                                }
                            }
                            else
                            {
                                print("未检测到可用魔法阵");
                            }                        
                    }
                    if (Math.Abs(x - x1) < 10 && Math.Abs(y - y1) < 10 && Math.Abs(z - z1) < 10)
                    {
                        state = 0;
                    }

                    if (x - x1 < -25)
                    {
                        state = 1;//transform.Translate(0, 0, -1);                     
                    }
                    if (x - x1 > 25)
                    {
                        state = 2;//transform.Translate(0, 0, 1);                                         
                    }
                    if (y - y1 < -25)
                    {
                        state = 3;//transform.Translate(-1, 0, 0);
                    }
                    if (y - y1 > 25)
                    {
                        state = 4;//transform.Translate(1, 0, 0);
                    }
                    if (state == 1 && laststate == 0 && kaiguan == 1 && playerAble)
                    {                                               
                           if (floating <= 0)
                            {
                                if (magic > 0)
                                {

                                    playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
                                    magic--;
                                    text_magic.text = "法力：" + magic.ToString();
                                    floating = 2;
                                    print("开启悬浮状态");
                                    transform.FindChild("WindMagic").gameObject.SetActive(true);

                                    //人物动画部分
                                    playerAnimator.SetBool("beginWindMagic", true);
                                    StartCoroutine(delayToSetFalse("beginWindMagic", 0.2f));
                                    playerAnimator.SetBool("inWindMagic", true);

                                    targetPosition = transform.position + floatingheight;
                                }
                            }
                            else
                            {
                                magic--;
                                text_magic.text = "法力：" + magic.ToString();
                                floating = 2;
                                print("开启悬浮状态2");
                                transform.FindChild("WindMagic").gameObject.SetActive(true);

                                //人物动画部分
                                playerAnimator.SetBool("beginWindMagic", true);
                                StartCoroutine(delayToSetFalse("beginWindMagic", 0.2f));
                                playerAnimator.SetBool("inWindMagic", true);
                            }                       
                    }
                    if (state == 2 && laststate == 0 && kaiguan == 1 && playerAble)
                    {
                            if (magic > 0)
                            {

                                playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
                                magic--;
                                text_magic.text = "法力：" + magic.ToString();
                                if (floating != 0)
                                    offset = new Vector3(0, transform.position.y + steam_height - 2, 0);
                                else
                                    offset = new Vector3(0, transform.position.y + steam_height, 0);
                                hits1 = Physics.RaycastAll(ray_start1 + offset, ray_forward, ray_length);
                                hits2 = Physics.RaycastAll(ray_start2 + offset, ray_forward, ray_length);
                                hits3 = Physics.RaycastAll(ray_start3 + offset, ray_forward, ray_length);
                                int i;
                                print("冰冻魔法开启了");
                                ice_m.transform.position = new Vector3(0, 40 * floor - 38.5f, 0);
                                transform.FindChild("IceMagic").gameObject.SetActive(true);

                                //人物动画部分
                                playerAnimator.SetBool("beginIceMagic", true);
                                StartCoroutine(delayToSetFalse("beginIceMagic", 0.2f));

                                /* 此时可以播放动画 */
                                steam = null;
                                for (i = 0; i < hits1.Length; i++)
                                {
                                    steam = hits1[i].transform.gameObject;
                                    steam.GetComponent<ParticleSystem>().Stop();
                                    //此处增加显示相应冰冻层     
                                    steam.transform.FindChild("icefloor").gameObject.SetActive(true);
                                }
                                for (i = 0; i < hits2.Length; i++)
                                {
                                    steam = hits2[i].transform.gameObject;
                                    steam.GetComponent<ParticleSystem>().Stop();
                                    //此处增加显示相应冰冻层
                                    steam.transform.FindChild("icefloor").gameObject.SetActive(true);
                                }
                                for (i = 0; i < hits3.Length; i++)
                                {
                                    steam = hits3[i].transform.gameObject;
                                    steam.GetComponent<ParticleSystem>().Stop();
                                    //此处增加显示相应冰冻层
                                    steam.transform.FindChild("icefloor").gameObject.SetActive(true);
                                }
                                if (steam != null)
                                    ice = true;
                            }                        
                    }
                    if (state == 3 && laststate == 0 && kaiguan == 1 && invincible == false && playerAble)
                    {
                            if (magic > 0)
                            {

                                playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
                                invincible = true;
                                transform.FindChild("EarthMagic").gameObject.SetActive(true);

                                //人物动画部分
                                playerAnimator.SetBool("beginEarthMagic", true);
                                StartCoroutine(delayToSetFalse("beginEarthMagic", 0.2f));

                                print("开启无敌");
                                magic--;
                                text_magic.text = "法力：" + magic.ToString();
                            }                        
                    }
                    if (state == 4 && laststate == 0 && kaiguan == 1 && playerAble)
                    {
                            if (magic > 0)
                            {

                                playerAnimator.SetFloat("trickTime", 0);//小动作计时器归零
                                magic--;
                                text_magic.text = "法力：" + magic.ToString();
                                print("让烈焰净化世界吧！");
                                transform.FindChild("FireMagic").gameObject.SetActive(true);

                                //人物动画部分
                                playerAnimator.SetBool("beginFireMagic", true);
                                StartCoroutine(delayToSetFalse("beginFireMagic", 0.2f));

                                // 融化冰块，注意，在漂浮时射线会达到上层的地板     
                                float temp_height;    // 消除浮动带来的影响
                                if (floating != 0)
                                    temp_height = steam_height - 2;
                                else
                                    temp_height = steam_height;
                                if (seal == false && Physics.Raycast(targetPosition, new Vector3(0, 1, 0), out hit, temp_height))
                                {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                    GameObject obj = hit.transform.gameObject;
                                    obj.transform.FindChild("icefloor").GetComponent<melt>().enabled = true;
                                    // 此处加冰块融化动画（如果有的话）以及消除冰块
                                    StartCoroutine(delayToFly(obj, 2.0f));// 既然能站上来，此时粒子一定已经停止
                                }
                                // 如果四周有木头就烧掉，物品烧毁的动画跟在"SetActive(false);"后面。
                                if (Physics.Raycast(targetPosition, new Vector3(-1, 0, 0), out hit, rayLenth))
                                {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                    GameObject obj = hit.transform.gameObject;
                                    if (obj.tag == "Wood")
                                    {
                                        obj.GetComponent<woodFire>().enabled = true;
                                        //obj.SetActive(false);
                                    }
                                }
                                if (Physics.Raycast(targetPosition, new Vector3(1, 0, 0), out hit, rayLenth))
                                {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                    GameObject obj = hit.transform.gameObject;
                                    if (obj.tag == "Wood")
                                    {
                                        obj.GetComponent<woodFire>().enabled = true;
                                        //obj.SetActive(false);
                                    }
                                }
                                if (Physics.Raycast(targetPosition, new Vector3(0, 0, 1), out hit, rayLenth))
                                {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                    GameObject obj = hit.transform.gameObject;
                                    if (obj.tag == "Wood")
                                    {
                                        obj.GetComponent<woodFire>().enabled = true;
                                        //obj.SetActive(false);
                                    }
                                }
                                if (Physics.Raycast(targetPosition, new Vector3(0, 0, -1), out hit, rayLenth))
                                {   //传入的参数依次是：射线起点、射线方向、射线碰撞物、射线检测距离
                                    GameObject obj = hit.transform.gameObject;
                                    if (obj.tag == "Wood")
                                    {
                                        obj.GetComponent<woodFire>().enabled = true;
                                        //obj.SetActive(false);
                                    }
                                }
                        }
                    }
                    a = string.Empty;
                }

            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    void OnApplicationQuit()
    {
        ClosePort();
    }

}
