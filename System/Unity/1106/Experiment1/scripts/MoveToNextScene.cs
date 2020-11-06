using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class MoveToNextScene : MonoBehaviour
{
    public SerialHandler serialHandler;
    float totalTime;
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
        GoFromArduino = false;

        // 実験の順番、現在の試行番号・状態等を取得
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment1_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のIDと条件(A, B, C)をリストに格納
        AromaCondition.AddRange(orderString);
        // 試行が練習なのか本番なのか、試行番号を読み込み
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        Test_or_Trial = trialState[0];
        TrialID = int.Parse(trialState[1]);
        sr.Close();

        //36試行まで終わったら、一度休憩
        if (TrialID == 36)
        {
            //ペルチェ素子にかかる電圧を0にする
            serialHandler.Write("o");
            Debug.Log("Off Command!");
            Debug.Log("Scene Change");
            // 新しい情報をcsvファイルに書き込み
            TrialID += 1;
            StreamWriter sw1;
            sw1 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment1_order.csv", false);
            string orderLinesw = string.Join(",", AromaCondition);
            sw1.WriteLine(orderLinesw);
            string[] ids = { Test_or_Trial, TrialID.ToString() };
            string idsLine = string.Join(",", ids);
            sw1.WriteLine(idsLine);
            sw1.Flush();
            sw1.Close();
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            SceneManager.LoadScene("Intermission_Scene");
        }
        //72試行で実験終了
        else if (TrialID == 72)
        {
            //ペルチェ素子にかかる電圧を0にする
            serialHandler.Write("o");
            Debug.Log("Off Command!");
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            SceneManager.LoadScene("End_Scene");
        }
        else
        {
            // 試行番号(TrialID)の更新
            if ((Test_or_Trial == "Trial" && TrialID != 37) || (Test_or_Trial == "Test" && TrialID < 2))
            {
                TrialID += 1;
            }
            else if (Test_or_Trial == "Test" && TrialID == 2)
            {
                Test_or_Trial = "Trial";
                TrialID = 1;
            }
            
            if (Test_or_Trial == "Test")
            {
                Debug.Log(Test_or_Trial);
                int AromaID = TrialID;
                Debug.Log("Aroma ID : " + AromaID.ToString());
            }
            else
            {
                string line = AromaCondition[TrialID - 1];
                int AromaID = int.Parse(line.Substring(0, line.Length - 1));
                string Condition = line.Substring(line.Length - 1);

                Debug.Log(Condition);
                if (Condition == "A" || Condition == "B")
                {
                    serialHandler.Write("r");
                    Debug.Log("Room Temperature command!");  // ログを出力
                }
                else if (Condition == "C")
                {
                    serialHandler.Write("h");
                    Debug.Log("Hot command!");  // ログを出力
                }

                Debug.Log("Aroma ID : " + AromaID.ToString());
            }

            // 新しい情報をcsvファイルに書き込み
            StreamWriter sw1;
            sw1 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment1_order.csv", false);
            string orderLinesw = string.Join(",", AromaCondition);
            sw1.WriteLine(orderLinesw);
            string[] ids = { Test_or_Trial, TrialID.ToString() };
            string idsLine = string.Join(",", ids);
            sw1.WriteLine(idsLine);
            sw1.Flush();
            sw1.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        if (Test_or_Trial == "Test" && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            SceneManager.LoadScene("Explanation_Scene");
        }

        if (GoFromArduino == true && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            if (Test_or_Trial == "Trial" && TrialID == 1)
            {
                SceneManager.LoadScene("Real_Scene");
            }
            else
            {
                SceneManager.LoadScene("Explanation_Scene");
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            if (Test_or_Trial == "Trial" && TrialID == 1)
            {
                SceneManager.LoadScene("Real_Scene");
            }
            else
            {
                SceneManager.LoadScene("Explanation_Scene");
            }
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
                    //serialHandler.GetComponent<SerialHandler>().enabled = false;
                    if (Test_or_Trial == "Test" && TrialID == 1)
                    {
                        SceneManager.LoadScene("Real_Scene");
                    }
                    else
                    {
                        SceneManager.LoadScene("Explanation_Scene");
                    }
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
