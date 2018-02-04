using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

public class control1 : MonoBehaviour {

    public string portName = "COM4";
    public int baudRate = 9600;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;

    SerialPort stream = null;

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

    ArrayList list = new ArrayList();          // 存放操作指令
    int count;                                 // 存放list元素个数
    V2 position_last;                          // 存放最近的有效的位置
    V2 position_now;                           // 存放当前踩下的位置
    V2 position_diff;                          // 存放位置差
    string con;                                // 存放取出的指令

    // Use this for initialization
	void Start () {
        position_last = new V2(-1, -1);// 假设初始位置是左下角
        OpenPort();
        StartCoroutine(DataReceiveFunction());
    }
	public void OpenPort()
    {
        stream = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        try
        {
            stream.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    public void ClosePort()
    {
        try
        {
            stream.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
        IEnumerator DataReceiveFunction()
    {
        string value = string.Empty;
        int bytesToRead = 0;
        string str = string.Empty;

        while (true)
        {
            if (stream != null && stream.IsOpen)
            {
                try
                {
                    byte[] dataBytes = new byte[1];
                    bytesToRead = stream.Read(dataBytes, 0, dataBytes.Length);
                    value = System.Text.Encoding.Default.GetString(dataBytes);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                Console.WriteLine(value);

            }


            yield return new WaitForSeconds(0.001f);
        }
    }


    // Update is called once per frame
    void Update () {




 // 用于检查存入表中的内容以确认代码是否正确
        if (Input.GetKeyUp("j"))
        {
            string temp = "内容为： ";
            for(int i = 0; i < list.Count; i++)
            {
                temp = temp + list[i].ToString() + " ";
            }
            print(temp);
        }

        // 用于取出表中的操作指令
        // 应把这段放到player.cs中（同时删除这一段）
        // 并把list改为可以被其它脚本（player.cs）读取的变量
        // 然后如果偷懒就将if(Input.GetKeyUp("w"))之类的改成if(con == "w")
        // 可能改成switch(con){case "w":……}这种更好一点
        // 别忘了在player.cs 中声明变量 string con;并把这里的声明去掉
        if (Input.GetKeyUp("k"))
        {
            if (list.Count != 0)
            {
                con = list[0].ToString();
                list.RemoveAt(0);
                print(con);      // 这句用于检验，实际使用时删除
            }
        }

        // 输入的指令，也就是玩家踩了跳舞毯的哪个位置
        // 这里用键盘输入1、2、3……代替
        // 这和三个开门键有重复，但是太难找到合适键了……
        // 如果能让arduino直接传过来(整数型)坐标那就可以省去这一步了
        if (Input.GetKeyUp("1"))
        {
            position_now = new V2(-1, -1);
        }
        else if (Input.GetKeyUp("2"))
        {
            position_now = new V2(0, -1);
        }
        else if (Input.GetKeyUp("3"))
        {
            position_now = new V2(1, -1);
        }
        else if (Input.GetKeyUp("4"))
        {
            position_now = new V2(-1, 0);
        }
        else if (Input.GetKeyUp("5"))
        {
            position_now = new V2(0, 0);
        }
        else if (Input.GetKeyUp("6"))
        {
            position_now = new V2(1, 0);
        }
        else if (Input.GetKeyUp("7"))
        {
            position_now = new V2(-1, 1);
        }
        else if (Input.GetKeyUp("8"))
        {
            position_now = new V2(0, 1);
        }
        else if (Input.GetKeyUp("9"))
        {
            position_now = new V2(1, 1);
        }
        else
            return;
        
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
	}

}


