using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.IO;
using System.Collections.Generic;
public class PortControl : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;
    public Parity parity = Parity.None;
    public int dataBits = 8;
    public StopBits stopBits = StopBits.One;
    SerialPort sp = null;

    void Start()
    {
        OpenPort();
        StartCoroutine(DataReceiveFunction());
    }

    //打开串口
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
		int state =0;
		int laststate=0;
		int bstate1 = 0;
		int lastbstate1 = 0;
		int bstate2 = 0;
		int lastbstate2 = 0;
		int bstate3 = 0;
		int lastbstate3 = 0;
		int kaiguan = 0;
        string a = string.Empty;
        string str = string.Empty;
        int x=0, y=0, z=0;
        int x1 = 335, y1 = 350, z1 =365;
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
                    Debug.Log(a);
					string str2= a.Substring(0,4); 
					string str3 = a.Substring(4,9);
					//Debug.Log(str3);
                    
                    int i = Convert.ToInt32(str3);				
                    z = i % 1000;
                    i = (i - z) / 1000;
                    y = i % 1000;
                    i = (i - y) / 1000;
					x = i % 1000;
					//i = (i - x) / 1000;
                    //Debug.Log(z);
					int j = Convert.ToInt32(str2);
					int b1 = j / 1000;
					int b2 = j % 10;
					j = (j - b2)/10;
					int b3 = j % 10;
					lastbstate1 = bstate1;
					bstate1 = b1;
					lastbstate2 = bstate2;
					bstate2 = b2;
					lastbstate3 = bstate3;
					bstate3 = b3;
					laststate = state;
					if(bstate1==1&&lastbstate1==0)
					{
						kaiguan = 1 - kaiguan;
					}
					if(bstate2==1&&lastbstate2==0)
					{
						transform.Translate(0, 1, 0);
					}
					if(bstate3==1&&lastbstate3==0)
					{
						transform.Translate(0, -1, 0);
					}
					if(Math.Abs(x-x1)<10 && Math.Abs(y-y1)<10 && Math.Abs(z-z1)<10)
					{
						state = 0;
					}
                                             
                    if (x-x1 < -25)
                    {
                        state = 1;//transform.Translate(0, 0, -1);                     
                    }
                    if (x-x1 > 25)
                    {
                        state = 2;//transform.Translate(0, 0, 1);                                         
                    }
                    if (y-y1 < -25)
                    {
                        state = 3;//transform.Translate(-1, 0, 0);
                    }
                    if (y-y1 > 25)
                    {
                        state = 4;//transform.Translate(1, 0, 0);
                    }
                    if(state==1&&laststate==0&&kaiguan == 1)
					{
						transform.Translate(0, 0, -1); 
					}
					if(state==2&&laststate==0&&kaiguan == 1)
					{
						transform.Translate(0, 0, 1); 
					} 
					if(state==3&&laststate==0&&kaiguan == 1)
					{
						transform.Translate(-1, 0, 0);
					} 
					if(state==4&&laststate==0&&kaiguan == 1)
					{
						transform.Translate(1, 0, 0);
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