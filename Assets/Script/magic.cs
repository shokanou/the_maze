using UnityEngine;
using System.Collections;

public class magic : MonoBehaviour {

    Boss boss_script;                           // Boss脚本

    bool stop;                                  // 是否停止释放技能

    /* 时间 */
    float timer;                                // 计时器
    float time_interval;                        // 时间间隔
    public float time_base = 5.0f;              // 基础技能间隔时间
    public float time_fluctuation = 2.0f;       // 技能间隔波动时间

    /* 概率 */
    float random;                                    // 生成的随机数
    float probability;                               // 剩余事件总概率
    // 俯冲0.1   激光炮0.1   火球    木墙    致盲     减速    封印楼层
    public float probability_slow = 0.05f;           // 减速魔法概率
    public float probability_seal = 0.1f;            // 封印楼层概率
    public float probability_blind = 0.15f;          // 致盲魔法概率
    public float probability_laser = 0.15f;          // 激光炮魔法概率
    public float probability_wwall = 0.15f;          // 木墙魔法概率
    public float probability_sprint = 0.1f;          // 魔王冲击波概率
    public float probability_fireball = 0.15f;       // 火球（陨石）魔法概率
    // 概率边界
    float[] boundary = new float[7];

    // 决定使用哪个魔法
    void which_magic()
    {
        int i;
        random = Random.value;
        for (i = 0; i < 7; i++)
        {
            if (random < boundary[i])
                break;
        }
        switch (i)
        {
            case 0:
                {
                    boss_script.magic_slow();
                    print("时间就像海绵里的水,被我挤一下就改变了.");
                    break;
                }
            case 1:
                {
                    boss_script.magic_seal();
                    print("我不让你走,你又能去哪?");
                    break;
                }
            case 2:
                {
                    boss_script.magic_blind();
                    print("天要黑了,你带灯了吗?");
                    break;
                }
            case 3:
                {
                    boss_script.magic_laser();
                    print("要有光!");
                    break;
                }
            case 4:
                {
                    boss_script.magic_woodenwall();
                    print("画地即可为牢.");
                    break;
                }
            case 5:
                {
                    boss_script.magic_sprint();
                    print("让我看看这地板够不够结实.");
                    break;
                }
            case 6:
                {
                    boss_script.magic_fireball();
                    print("你喜欢烟花吗?");
                    break;
                }
            default:
                {
                    print("唉,看你这样子，我都不好意思出手了.");
                    break;
                }
        }
    }

	// Use this for initialization
	void Start () {
        boss_script = GetComponent<Boss>();
        stop = true;     // 开始时不释放技能
        // 时间
        timer = 0;
        random = Random.value;
        time_interval = time_base + time_fluctuation * (random + random - 1);
        // 初始化概率边界
        boundary[0] = probability_slow;
        boundary[1] = probability_seal + boundary[0];
        boundary[2] = probability_blind + boundary[1];
        boundary[3] = probability_laser + boundary[2];
        boundary[4] = probability_wwall + boundary[3];
        boundary[5] = probability_sprint + boundary[4];
        boundary[6] = probability_fireball + boundary[5];
        // 先来一个魔法
        //which_magic();
	}
	
	// Update is called once per frame
	void Update () {
         // 停止/开启技能自动释放
        if (Input.GetKeyUp("q"))
        {
            if (stop == false)
            {
                print("停止技能自动释放");
                stop = true;
            }
            else
            {
                print("开始技能自动释放");
                stop = false;
                timer = time_interval;
            }
        }

        // 技能的释放
        if (stop == true)
            return;

        timer += Time.deltaTime;
        if (timer > time_interval)
        {
            which_magic();
            timer = 0;
            random = Random.value;
            time_interval = time_base + time_fluctuation * (random + random - 1);
        }
	}
}
