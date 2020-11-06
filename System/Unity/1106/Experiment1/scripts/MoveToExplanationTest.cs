using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToExplanationTest : MonoBehaviour
{
    float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        totalTime = 3;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        if (totalTime <= 0)
        {
            SceneManager.LoadScene("Explanation_Scene");
        }
    }
}
