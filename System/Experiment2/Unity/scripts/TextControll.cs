using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class TextControll : MonoBehaviour
{
    public SerialHandler serialHandler;
    public Text ExplainText; //説明文
    public Text CountText; //カウントダウンテキスト
    public Text EndText; //終わりのテキスト

    int PumpID;  //使用するエアポンプの番号

    float totalTime;
    int sec;
    int mode;
    bool mode_changed = false;

    // Start is called before the first frame update
    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        //serialHandler.OnDataReceived += OnDataReceived;

        // 説明文のみを表示
        ExplainText.enabled = true;
        CountText.enabled = false;
        EndText.enabled = false;
        mode = 0; //説明文表示モード
        totalTime = 5;

        // 実験の順番、現在の試行番号・状態等を取得
        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment2_order.csv");
        // CSVファイルの一行を読み込む
        string orderLine = sr.ReadLine();
        // 読み込んだ一行をカンマ毎に分けて配列に格納する
        string[] orderString = orderLine.Split(',');
        // 各試行の香料のIDと条件(R, H, C)をリストに格納
        List<string> AromaCondition = new List<string>();
        AromaCondition.AddRange(orderString);
        string trialStateLine = sr.ReadLine();
        string[] trialState = trialStateLine.Split(',');
        string Test_or_Trial = trialState[0];
        int TrialID = int.Parse(trialState[1]);
        sr.Close();

        string line = AromaCondition[TrialID - 1];
        int AromaID = int.Parse(line.Substring(0, line.Length - 1));
        PumpID = AromaID % 3;
        if (PumpID == 0)
        {
            PumpID = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == 0 && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            // カウントのみを表示
            ExplainText.enabled = false;
            CountText.enabled = true;
            EndText.enabled = false;
            mode = 1; //カウント表示モード
            mode_changed = true;
        }

        else if (mode == 1)
        {
            if (mode_changed == true)
            {
                // 匂い噴出を開始
                serialHandler.Write(PumpID.ToString()); 
                mode_changed = false;
            }

            // カウントダウンを表示
            totalTime -= Time.deltaTime;
            sec = (int)totalTime;
            CountText.text = sec.ToString();

            if (totalTime <= 0)
            {
                // 終わりのテキストのみを表示
                ExplainText.enabled = false;
                CountText.enabled = false;
                EndText.enabled = true;
                mode = 2; //終わりのテキスト表示モード
                mode_changed = true;
                totalTime = 5;
            }
        }

        else if (mode == 2)
        {
            if (mode_changed == true)
            {
                // 空気を噴出して、前の匂いを飛ばす  様子見
                //serialHandler.Write("4");
                serialHandler.Write("0"); // すべてのエアポンプをOFFにする
                serialHandler.Write("o"); // 鼻部皮膚温度制御を止める
                mode_changed = false;
            }

            totalTime -= Time.deltaTime;
            // 5秒経ったら、待機シーンに遷移
            if (totalTime <= 0)
            {
                //serialHandler.Write("0"); // すべてのエアポンプをOFFにする
                SceneManager.LoadScene("Rating_Scene");
            }
        }

    }
}
