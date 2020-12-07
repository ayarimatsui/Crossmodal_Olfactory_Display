using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class SliderControll : MonoBehaviour
{
    public SerialHandler serialHandler;
    public Slider _slider;
    int val;

    // Start is called before the first frame update
    void Start()
    {
        val = 3; //スライダーの値
        _slider.value = val;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft))
        {
            if (val <= 0)
            {
                val = 0;
            }
            else
            {
                val -= 1;
            }
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight))
        {
            if (val >= 6)
            {
                val = 6;
            }
            else
            {
                val += 1;
            }
        }
        //スライダーの値を変更
        _slider.value = val;

        // 右手人差し指のトリガーを引くと、値を決定し、次のシーンに移る
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            // csvファイルに保存
            CSVSave(val, "Experiment3_evaluation");
            // Waiting Sceneに遷移
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Waiting_Scene");
    }

    public void CSVSave(int x, string fileName)
    {
        StreamWriter sw;
        sw = new StreamWriter(Application.dataPath + "/Data/" + fileName + ".csv", true);

        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment3_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のIDと条件(R, H, C)をリストに格納
        List<string> AromaCondition = new List<string>();
        AromaCondition.AddRange(orderString);
        // 試行が練習なのか本番なのか、試行番号を読み込み
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        string Test_or_Trial = trialState[0];
        int TrialID = int.Parse(trialState[1]);
        int AromaID;
        int VisualStimuliID;
        string Condition;

        if (Test_or_Trial == "Test")
        {
            AromaID = TrialID;
            VisualStimuliID = 5;
            Condition = "T";
        }
        else
        {
            string line = AromaCondition[TrialID - 1];
            AromaID = int.Parse(line.Substring(0, line.Length - 1)) / 100;
            VisualStimuliID = int.Parse(line.Substring(0, line.Length - 1)) % 100;
            Condition = line.Substring(line.Length - 1);
        }

        string[] s1 = { TrialID.ToString(), AromaID.ToString(), VisualStimuliID.ToString(),Condition, x.ToString() };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Flush();
        sw.Close();
    }
}
