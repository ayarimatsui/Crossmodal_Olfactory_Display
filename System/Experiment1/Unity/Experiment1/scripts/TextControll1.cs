using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class TextControll1 : MonoBehaviour
{
    public SerialHandler serialHandler;
    public Text ExplainText; //説明文
    public Text CountText; //カウントダウンテキスト
    public Text EndText; //終わりのテキスト

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
                serialHandler.Write("1");
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
                // 空気を噴出して、前の匂いを飛ばす
                serialHandler.Write("2");
                mode_changed = false;
            }

            totalTime -= Time.deltaTime;
            // 5秒経ったら、待機シーンに遷移
            if (totalTime <= 0)
            {
                serialHandler.Write("0"); // すべてのエアポンプをOFFにする
                SceneManager.LoadScene("Waiting_Scene1");
            }
        }

    }
}

