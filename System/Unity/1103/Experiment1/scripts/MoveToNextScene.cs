﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MoveToNextScene : MonoBehaviour
{
    public SerialHandler serialHandler;
    float totalTime;
    int TestID;
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

        //TestIDの更新
        ExperimentID.addNumberOfTestID();
        TestID = ExperimentID.getTestID();
        TrialID = ExperimentID.getTrialID();

        //36試行まで終わったら、一度休憩
        if (TrialID == 36)
        {
            //ペルチェ素子にかかる電圧を0にする
            serialHandler.Write("o");
            Debug.Log("Off Command!");
            Debug.Log("Scene Change");
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
            if (TestID > 2)
            {
                ExperimentID.addNumberOfTrialID();
            }
            TrialID = ExperimentID.getTrialID();
            //準備する香料の表示、鼻部皮膚温度制御の信号をArduinoへ送信
            string Condition = ExperimentID.getCondition();
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

            int AromaID = ExperimentID.getAromaID();
            Debug.Log("Aroma ID : " + AromaID.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        if (TestID <= 2 && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            SceneManager.LoadScene("Explanation_Scene");
        }

        if (GoFromArduino == true && totalTime <= 0)
        {
            Debug.Log("Scene Change");
            //serialHandler.GetComponent<SerialHandler>().enabled = false;
            if (TrialID == 1)
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
            if (TrialID == 1)
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
                    if (TrialID == 1)
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
