using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;

public class ExperimentID : MonoBehaviour
{
    public static int TrialID = 0;
    public static int TestID = 1; 
    public static int AromaID = 0;
    public static string Condition = "T";
    public static List<string> AromaCondition = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 24; i++)
        {
            AromaCondition.Add((i + 1).ToString() + "A");
            AromaCondition.Add((i + 1).ToString() + "B");
            AromaCondition.Add((i + 1).ToString() + "C");
        }

        System.Random r = new System.Random();

        AromaCondition = AromaCondition.OrderBy(a => r.Next(AromaCondition.Count)).ToList();

        //データ保存用csvファイルの作成

        StreamWriter sw;
        sw = new StreamWriter(Application.dataPath + "/Data/" + "Experiment1.csv", true);
        string[] s1 = { "TrialID", "AromaID", "Condition", "Evaluation" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Flush();
        sw.Close();
    }

   
    public static int getTrialID()
    {
        return TrialID;
    }

    public static int getTestID()
    {
        return TestID;
    }

    //試行の最後に必要
    public static void addNumberOfTrialID()
    {
        TrialID += 1;
    }

    //試行の最後に必要
    public static void addNumberOfTestID()
    {
        TestID += 1;
    }

    public static int getAromaID()
    {
        if (TrialID > 0)
        {
            string line = AromaCondition[TrialID - 1];
            AromaID = int.Parse(line.Substring(0, line.Length - 1));
        }

        else
        {
            if (TestID == 1)
            {
                AromaID = 1;
            }
            else if (TestID == 2)
            {
                AromaID = 2;
            }
        }
        return AromaID;
    }

    public static string getCondition()
    {
        if (TrialID > 0)
        {
            string line = AromaCondition[TrialID - 1];
            Condition = line.Substring(line.Length - 1);
        }
        else
        {
            Condition = "T";
        }
        return Condition;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
