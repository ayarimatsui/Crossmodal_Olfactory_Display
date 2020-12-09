using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;


public class ExperimentID : MonoBehaviour
{
    //外部のcsvファイルに書き込み＆読み込みでグローバル変数を共有に書き直す
    public int TrialID = 0;
    public int TestID = 1; 
    public int AromaID = 0;
    public string Condition = "T";
    public List<string> AromaCondition = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        List<string> AromaCondition1 = new List<string>();
        List<string> AromaCondition2 = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            AromaCondition1.Add((i + 1).ToString() + "R");
            AromaCondition1.Add((i + 1).ToString() + "H");
            AromaCondition1.Add((i + 1).ToString() + "C");
        }
        for (int i = 3; i < 6; i++)
        {
            AromaCondition2.Add((i + 1).ToString() + "R");
            AromaCondition2.Add((i + 1).ToString() + "H");
            AromaCondition2.Add((i + 1).ToString() + "C");
        }

        AromaCondition1 = AromaCondition1.OrderBy(a => Guid.NewGuid()).ToList();
        AromaCondition2 = AromaCondition2.OrderBy(a => Guid.NewGuid()).ToList();

        AromaCondition = AromaCondition1.Concat(AromaCondition2).ToList();

        //実験の順番＆現在のTestIDとTrialIDを保存しておく用csvファイルの作成 (実験を中断＆途中再開できるように)
        StreamWriter sw1;
        sw1 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment2_order.csv", true);
        string orderLine = string.Join(",", AromaCondition);
        sw1.WriteLine(orderLine);
        string[] ids = { "Test", TestID.ToString()};
        string idsLine = string.Join(",", ids);
        sw1.WriteLine(idsLine);
        sw1.Flush();
        sw1.Close();

        //評価データ保存用csvファイルの作成
        StreamWriter sw2;
        sw2 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment2_evaluation.csv", true);
        string[] s1 = { "TrialID", "Aroma", "Condition", "pleasant-unpleasant", "sweet", "sour", "bitter", "light", "fresh", "sticky" };
        string s2 = string.Join(",", s1);
        sw2.WriteLine(s2);
        sw2.Flush();
        sw2.Close();
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
