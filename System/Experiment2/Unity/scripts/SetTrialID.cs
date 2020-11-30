using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class SetTrialID : MonoBehaviour
{
    List<string> AromaCondition = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
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
        string State = trialState[0];
        int TrialID = int.Parse(trialState[1]);

        if (State == "Test")
        {
            Debug.Log("Test : " + TrialID.ToString());
        }
        else
        {
            Debug.Log("Trial : " + TrialID.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
