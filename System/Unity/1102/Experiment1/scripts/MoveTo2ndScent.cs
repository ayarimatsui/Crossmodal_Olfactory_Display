using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class MoveTo2ndScent : MonoBehaviour
{
    public SerialHandler serialHandler;
    float totalTime;
    int TestID;

    // Start is called before the first frame update
    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;

        totalTime = 15;

        TestID = ExperimentID.getTestID();

        //準備する香料の表示、鼻部皮膚温度制御の信号をArduinoへ送信
        string Condition = ExperimentID.getCondition();
        if (Condition == "A")
        {
            serialHandler.Write("h");
            Debug.Log("Hot command!");  // ログを出力
        }
        else if (Condition == "B" || Condition == "C")
        {
            serialHandler.Write("c");
            Debug.Log("Cool command!");  // ログを出力
        }

        int AromaID = ExperimentID.getAromaID();
        Debug.Log("Aroma ID : " + AromaID.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        if (TestID <= 2 && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            serialHandler.GetComponent<SerialHandler>().enabled = false;
            SceneManager.LoadScene("2nd_Scent");
        }
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
