using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ChangeToWaitingScene : MonoBehaviour
{
    Slider _slider;
    int slider_val;
    
    // Start is called before the first frame update
    void Start()
    {
        // スライダーを取得する
        _slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // 右手人差し指のトリガーを引くと、値を決定
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            slider_val = (int)_slider.value;
            // csvファイルに保存
            CSVSave(slider_val, "Subject_1");
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
        /*
        FileInfo fi;
        // Aplication.dataPath で プロジェクトファイルがある絶対パスが取り込める
        fi = new FileInfo(Application.dataPath + "/Data/" + fileName + ".csv");
        sw = fi.AppendText();*/
        sw = new StreamWriter(Application.dataPath + "/Data/" + fileName + ".csv", true);
        sw.WriteLine(x.ToString());
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
