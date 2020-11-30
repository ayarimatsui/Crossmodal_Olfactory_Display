using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class MoveToNextSceneFromIntermission : MonoBehaviour
{
    public SerialHandler serialHandler;
    float totalTime;
    float totalTimeLimit;
    List<string> AromaCondition = new List<string>();
    string Test_or_Trial;
    int TrialID;
    bool GoFromArduino;

    // Start is called before the first frame update
    void Start()
    {
        //serialHandler.GetComponent<SerialHandler>().enabled = true;
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;

        totalTime = 15;
        totalTimeLimit = 30; //30秒後には必ず次のシーンに遷移する
        GoFromArduino = false;

        // 実験の順番、現在の試行番号・状態等を取得
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment2_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のIDと条件(R, H, C)をリストに格納
        AromaCondition.AddRange(orderString);
        // 試行が練習なのか本番なのか、試行番号を読み込み
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        Test_or_Trial = trialState[0];
        TrialID = int.Parse(trialState[1]);
        sr.Close();

        string line = AromaCondition[TrialID - 1];
        int AromaID = int.Parse(line.Substring(0, line.Length - 1));
        string Condition = line.Substring(line.Length - 1);

        Debug.Log(Condition);
        if (Condition == "R")
        {
            serialHandler.Write("r");
            Debug.Log("Room Temperature command!");  // ログを出力
        }
        else if (Condition == "H")
        {
            serialHandler.Write("h");
            Debug.Log("Hot command!");  // ログを出力
        }
        else if (Condition == "C")
        {
            serialHandler.Write("c");
            Debug.Log("Cool command!");  // ログを出力
        }

        Debug.Log("Aroma ID : " + AromaID.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;
        totalTimeLimit -= Time.deltaTime;

        if (GoFromArduino == true && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            SceneManager.LoadScene("Explanation_Scene");
        }

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            SceneManager.LoadScene("Explanation_Scene");
        }

        if (totalTimeLimit <= 0)
        {
            Debug.Log("Scene Change");
            SceneManager.LoadScene("Explanation_Scene");
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
                if (totalTime <= 0)
                {
                    Debug.Log("Scene Change");
                    SceneManager.LoadScene("Explanation_Scene");
                }
                else
                {
                    GoFromArduino = true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
