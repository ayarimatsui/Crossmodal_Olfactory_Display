using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextControll : MonoBehaviour
{
    public Text ExplainText; //説明文
    public Text CountText; //カウントダウンテキスト
    public Text EndText; //終わりのテキスト

    float totalTime;
    int sec;
    int mode;

    // Start is called before the first frame update
    void Start()
    {
        // 説明文のみを表示
        ExplainText.enabled = true;
        CountText.enabled = false;
        EndText.enabled = false;
        mode = 0; //説明文表示モード
        totalTime = 10;
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
        }
        
        else if (mode == 1)
        {
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
                totalTime = 5;
            }
        }

        else if (mode == 2)
        {
            totalTime -= Time.deltaTime;
            // 5秒経ったら、待機シーンに遷移
            if (totalTime <= 0)
            {
                SceneManager.LoadScene("Waiting_Scene");
            }
        }

    }
}
