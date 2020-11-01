using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class MoveTo2ndScent : MonoBehaviour
{
    public SerialHandler serialHandler;

    // Start is called before the first frame update
    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
        //テスト
        serialHandler.Write("h");
        Debug.Log("Hot command!");  // ログを出力
    }

    // Update is called once per frame
    void Update()
    {

    }

    //ArduinoからOKサインを受け取ったら、次のシーンに遷移する
    void OnDataReceived(string message)
    {
        try
        {
            Debug.Log("sign recieved");
            Debug.Log(message);
            string[] sign = message.Split(',');
            if (int.Parse(sign[0]) == 1)
            {
                Debug.Log("Scene Change");
                serialHandler.GetComponent<SerialHandler>().enabled = false;
                SceneManager.LoadScene("2nd_Scent");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
