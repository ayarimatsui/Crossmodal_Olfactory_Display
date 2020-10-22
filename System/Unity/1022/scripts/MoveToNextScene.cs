using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToNextScene : MonoBehaviour
{
    public GameObject SerialHandler;
    public GameObject ButtonStop;
    float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        totalTime = 15;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        // 15秒経ったら、説明のシーンに遷移
        if (totalTime <= 0)
        {
            SerialHandler.GetComponent<SerialHandler>().enabled = false;
            ButtonStop.GetComponent<StopClicked>().enabled = false;
            SceneManager.LoadScene("Explanation_Scene");
        }
    }
}
