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
            CSVSave(slider_val, "Experiment1");
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

        int TestID = ExperimentID.getTestID();
        int TrialID = TestID;
        if (TestID <= 2)
        {
            TrialID = TestID;
        }
        else
        {
            TrialID = ExperimentID.getTrialID();
        }
        int AromaID = ExperimentID.getAromaID();
        string Condition = ExperimentID.getCondition();
        string[] s1 = {TrialID.ToString(), AromaID.ToString(), Condition, x.ToString()};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
        //sw.WriteLine(x.ToString());
        // 書き込みデータが配列の場合
        /*
        for (int i = 0; i < x.Length; i++)
        {
            sw.WriteLine(x[i].ToString());
        }*/
        sw.Flush();
        sw.Close();
    }
}
