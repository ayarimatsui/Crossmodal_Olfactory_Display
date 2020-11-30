using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class SliderControll : MonoBehaviour
{
    public Canvas Canvas1;
    public Canvas Canvas2;
    public Canvas Canvas3;
    public Canvas Canvas4;
    public Canvas Canvas5;
    public Canvas Canvas6;
    public Canvas Canvas7;
    public SerialHandler serialHandler;
    Slider _slider;
    int evaluation_mode;
    int[] slider_val = new int[7];

    // Start is called before the first frame update
    void Start()
    {
        // 最初の評価ポイントの画面のみを表示
        Canvas1.enabled = true;
        Canvas2.enabled = false;
        Canvas3.enabled = false;
        Canvas4.enabled = false;
        Canvas5.enabled = false;
        Canvas6.enabled = false;
        Canvas7.enabled = false;
        evaluation_mode = 1; //最初の評価ポイント表示モード
        // スライダーを取得する
        _slider = GameObject.Find("Slider" + evaluation_mode.ToString()).GetComponent<Slider>();
    }

    //スライダーの値
    int val = 0;
    // Update is called once per frame
    void Update()
    {
        if (evaluation_mode == 1)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft))
            {
                if (val <= -3)
                {
                    val = -3;
                }
                else
                {
                    val -= 1;
                }
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight))
            {
                if (val >= 3)
                {
                    val = 3;
                }
                else
                {
                    val += 1;
                }
            }
            //スライダーの値を変更
            _slider.value = val;
        }
        
        else
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
        }

        // 右手人差し指のトリガーを引くと、値を決定し、次の評価ポイントに移る
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            //配列に値を格納
            slider_val[evaluation_mode - 1] = (int)_slider.value;

            if(evaluation_mode <= 6)
            {
                //今表示されている画面を消す
                GameObject.Find("Canvas" + evaluation_mode.ToString()).GetComponent<Canvas>().enabled = false;
                evaluation_mode += 1;
                //次の画面を表示する
                GameObject.Find("Canvas" + evaluation_mode.ToString()).GetComponent<Canvas>().enabled = true;
                // スライダーを取得し直す
                _slider = GameObject.Find("Slider" + evaluation_mode.ToString()).GetComponent<Slider>();

                //valの値を初期に直す
                val = 3;
                //スライダーの値を変更
                _slider.value = val;
            }
            else if (evaluation_mode >= 7)
            {
                // csvファイルに保存
                CSVSave(slider_val, "Experiment2_evaluation");
                // Waiting Sceneに遷移
                ChangeScene();
            }
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Waiting_Scene");
    }

    public void CSVSave(int[] x, string fileName)
    {
        StreamWriter sw;
        sw = new StreamWriter(Application.dataPath + "/Data/" + fileName + ".csv", true);

        // 読み込みたいCSVファイルのパスを指定して開く
        StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + "Experiment2_order.csv");
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

        string[] s1 = { TrialID.ToString(), AromaID.ToString(), Condition, x[0].ToString(), x[1].ToString(), x[2].ToString(), x[3].ToString(), x[4].ToString(), x[5].ToString(), x[6].ToString()};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        sw.Flush();
        sw.Close();
    }
}
