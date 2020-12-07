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
        bool list_OK = false;
        while (list_OK == false)
        {
            List<string> AromaCondition1 = new List<string>();
            List<string> AromaCondition2 = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                AromaCondition1.Add("1" + (i + 1).ToString("00") + "R");
                AromaCondition1.Add("1" + (i + 1).ToString("00") + "H");
                AromaCondition1.Add("1" + (i + 1).ToString("00") + "C");
                AromaCondition2.Add("2" + (i + 1).ToString("00") + "R");
                AromaCondition2.Add("2" + (i + 1).ToString("00") + "H");
                AromaCondition2.Add("2" + (i + 1).ToString("00") + "C");
            }

            AromaCondition1 = AromaCondition1.OrderBy(a => Guid.NewGuid()).ToList();
            AromaCondition2 = AromaCondition2.OrderBy(a => Guid.NewGuid()).ToList();
            List<string> Condition_cand = new List<string>();
            for (int i = 0; i < 72; i++)
            {
                Condition_cand.Add(AromaCondition1[i]);
                Condition_cand.Add(AromaCondition2[i]);
            }

            list_OK = true;
            for (int i = 0; i < 143; i++)
            {
                int VisualStimuliID_i = int.Parse(Condition_cand[i].Substring(0, Condition_cand[i].Length - 1)) % 100;
                int VisualStimuliID_inext = int.Parse(Condition_cand[i + 1].Substring(0, Condition_cand[i + 1].Length - 1)) % 100;

                if (VisualStimuliID_i == VisualStimuliID_inext)
                {
                    list_OK = false;
                    break;
                }
            }
            if (list_OK == true)
            {
                AromaCondition = Condition_cand;
                break;
            }
        }

        //実験の順番＆現在のTestIDとTrialIDを保存しておく用csvファイルの作成 (実験を中断＆途中再開できるように)
        StreamWriter sw1;
        sw1 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment3_order.csv", true);
        string orderLine = string.Join(",", AromaCondition);
        sw1.WriteLine(orderLine);
        string[] ids = { "Test", TestID.ToString()};
        string idsLine = string.Join(",", ids);
        sw1.WriteLine(idsLine);
        sw1.Flush();
        sw1.Close();

        //評価データ保存用csvファイルの作成
        StreamWriter sw2;
        sw2 = new StreamWriter(Application.dataPath + "/Data/" + "Experiment3_evaluation.csv", true);
        string[] s1 = { "TrialID", "AromaID", "VisualID", "Temp Condition", "evaluation" };
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
