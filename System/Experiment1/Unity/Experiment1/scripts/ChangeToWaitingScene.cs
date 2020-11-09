using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ChangeToWaitingScene : MonoBehaviour
{
    public SerialHandler serialHandler;
    Slider _slider;
    int slider_val;
    
    // Start is called before the first frame update
    void Start()
    {
        // スライダーを取得する
        _slider = GameObject.Find("Slider").GetComponent<Slider>();
        //ペルチェ素子の電圧を0にする
        serialHandler.Write("o");
        Debug.Log("OFF command!");  // ログを出力
    }

    // Update is called once per frame
    void Update()
    {
        // 右手人差し指のトリガーを引くと、値を決定
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            slider_val = (int)_slider.value;
            // csvファイルに保存
            CSVSave(slider_val, "Experiment1_evaluation");
            // Waiting Sceneに遷移
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Waiting_Scene0");
    }

    public void CSVSave(int x, string fileName)
    {
        StreamWriter sw;
        sw = new StreamWriter(Application.dataPath + "/Data/" + fileName + ".csv", true);

        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment1_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のIDと条件(A, B, C)をリストに格納
        List<string> AromaCondition = new List<string>();
        AromaCondition.AddRange(orderString);
        // 試行が練習なのか本番なのか、試行番号を読み込み
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        string Test_or_Trial = trialState[0];
        int TrialID = int.Parse(trialState[1]);
        int AromaID;
        string Condition;

        if (Test_or_Trial == "Test")
        {
            AromaID = TrialID;
            Condition = "T";
        }
        else
        {
            string line = AromaCondition[TrialID - 1];
            AromaID = int.Parse(line.Substring(0, line.Length - 1));
            Condition = line.Substring(line.Length - 1);
        }

        string[] s1 = {TrialID.ToString(), AromaID.ToString(), Condition, x.ToString()};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Flush();
        sw.Close();
    }
}
