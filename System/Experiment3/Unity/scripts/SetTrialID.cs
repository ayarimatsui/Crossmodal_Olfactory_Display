using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class SetTrialID : MonoBehaviour
{
    List<string> AromaCondition = new List<string>();
    int AromaID;
    int VisualStimuliID;
    string State;

    // Start is called before the first frame update
    void Start()
    {
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment3_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のID, 視覚情報のIDと条件(R, H, C)をリストに格納
        AromaCondition.AddRange(orderString);
        // 試行が練習なのか本番なのか、試行番号を読み込み
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        State = trialState[0];
        int TrialID = int.Parse(trialState[1]);

        if (State == "Test")
        {
            Debug.Log("Test : " + TrialID.ToString());
        }
        else
        {
            string line = AromaCondition[TrialID - 1];
            AromaID = int.Parse(line.Substring(0, line.Length - 1)) / 100;
            VisualStimuliID = int.Parse(line.Substring(0, line.Length - 1)) % 100;
            string Condition = line.Substring(line.Length - 1);
            Debug.Log("Trial : " + TrialID.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            if (State == "Test")
            {
                SceneManager.LoadScene("5_Scene"); //テストはリンゴの視覚刺激で行う
            }
            else
            {
                SceneManager.LoadScene(VisualStimuliID.ToString()+"_Scene");
            }
        }
    }
}
