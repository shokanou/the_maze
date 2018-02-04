using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.IO;
using System.Collections.Generic;

public class control : MonoBehaviour {

    // 自定义的二维向量，因为Vector2是float型
    public class V2
    {
        // 变量
        public int x;
        public int y;
        // 构造函数
        public V2()
        {
            x = 0;
            y = 0;
        }
        public V2(int ix, int iy)
        {
            x = ix;
            y = iy;
        }
        // 减法重载
        public static V2 operator - (V2 a, V2 b)
        {
            return new V2(a.x-b.x, a.y-b.y);
        }
    }

    public static ArrayList list = new ArrayList();          // 存放操作指令
    int count;                                 // 存放list元素个数
    V2 position_last;                          // 存放最近的有效的位置
    V2 position_now;                           // 存放当前踩下的位置
    V2 position_diff;                          // 存放位置差
    string con;                                // 存放取出的指令

    //Setup parameters to connect to Arduino
    public static SerialPort sp = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
    int value;
    public string str = string.Empty;
    public string a= string.Empty;
    

    // Use this for initialization
    void Start () {
        position_last = new V2(0, -1);         // 假设初始位置是中下角


        OpenConnection();//connect port

        StartCoroutine(DataReceiveFunction());
    }

    public void OpenConnection()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                print("Closing port, because it was already open!");
            }
            else
            {
                sp.Open();  // opens the connection
                //sp.ReadTimeout = 50000;  // sets the timeout value before reporting error
                print("Port Opened!");
            }
        }
        else
        {
            if (sp.IsOpen)
            {
                print("Port is already open");
            }
            else
            {
                print("Port == null");
            }
        }
    }

    IEnumerator DataReceiveFunction()
    {
        int bytesToRead = 0;
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
                if (a.Length == 1)
                {
                    Debug.Log(a);

                    value = Int32.Parse(a);


                    // 输入的指令，也就是玩家踩了跳舞毯的哪个位置
                    // 这里用键盘输入1、2、3……代替
                    // 这和三个开门键有重复，但是太难找到合适键了……
                    // 如果能让arduino直接传过来(整数型)坐标那就可以省去这一步了
                    if (value == 7) //9
                    {
                        position_now = new V2(-1, -1);
                    }
                    else if (value == 6) //8
                    {
                        position_now = new V2(0, -1);
                    }
                    else if (value == 3) //5
                    {
                        position_now = new V2(1, -1);
                    }
                    else if (value == 8)  //10
                    {
                        position_now = new V2(-1, 0);
                    }
                    else if (value == 5)  //7
                    {
                        position_now = new V2(0, 0);
                    }
                    else if (value == 2)  //4
                    {
                        position_now = new V2(1, 0);
                    }
                    else if (value == 9) //11
                    {
                        position_now = new V2(-1, 1);
                    }
                    else if (value == 4)  //6
                    {
                        position_now = new V2(0, 1);
                    }
                    else if (value == 1)  //3
                    {
                        position_now = new V2(1, 1);
                    }
                    else
                        //return;
                        ;

                    // 根据差值确定应该归属于什么方向按键
                    position_diff = position_now - position_last;
                    count = list.Count;
                    // print(position_diff.x.ToString() + ", " + position_diff.y.ToString());
                    if (position_diff.x == 0)
                    {   // 竖直方向移动
                        switch (position_diff.y)
                        {
                            case -1:  // 向下
                                if (count == 0)
                                {
                                    list.Add("s");
                                }
                                else if (list[count - 1] == "w")
                                {
                                    list.RemoveAt(count - 1);
                                }
                                else
                                    list.Add("s");
                                position_last = position_now;
                                break;
                            case 1:    // 向前
                                if (count == 0)
                                    list.Add("w");
                                else if (list[count - 1] == "s")
                                    list.RemoveAt(count - 1);
                                else
                                    list.Add("w");
                                position_last = position_now;
                                break;
                            case 0:    // 位于原地
                                break;
                            default:   // 跨度太大
                                print("跨度太大！");
                                break;
                        }
                    }
                    else if (position_diff.y == 0)
                    {   // 水平方向移动
                        switch (position_diff.x)
                        {
                            case -1:  // 向左
                                if (count == 0)
                                    list.Add("a");
                                else if (list[count - 1] == "d")
                                    list.RemoveAt(count - 1);
                                else
                                    list.Add("a");
                                position_last = position_now;
                                break;
                            case 1:    // 向右
                                if (count == 0)
                                    list.Add("d");
                                else if (list[count - 1] == "a")
                                    list.RemoveAt(count - 1);
                                else
                                    list.Add("d");
                                position_last = position_now;
                                break;
                            case 0:    // 位于原地，事实上这种情况在前一个if中已包括
                                break;
                            default:   // 跨度太大
                                print("跨度太大！");
                                break;
                        }
                    }
                    else
                    {   // 斜方向移动
                        print("不允许斜向移动");
                    }
                    a = string.Empty;
                }
            }
            yield return new WaitForSeconds(0.001f);
        }

    }


        void OnApplicationQuit()
    {
        sp.Close();
    }

    // Update is called once per frame
    void Update()
    {
        // 用于检查存入表中的内容以确认代码是否正确
        if (Input.GetKeyUp("j"))
        {
            string temp = "内容为： ";
            for (int i = 0; i < list.Count; i++)
            {
                temp = temp + list[i].ToString() + " ";
            }
            print(temp);
        }
    }



}
