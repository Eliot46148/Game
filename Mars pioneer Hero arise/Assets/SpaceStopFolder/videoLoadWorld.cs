using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class videoLoadWorld : MonoBehaviour
{
    private float timer_f = 0f;
    private int timer_i = 0;
    public GameObject StartCam, EndCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer_f += Time.deltaTime;
        timer_i = (int)timer_f;
        //Debug.Log(timer_i);
        if (timer_i == 53 || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("World");
        }
        else if (loadWorld.SceneType == "World")
        {
            StartCam.SetActive(true);
            EndCam.SetActive(false);
            Debug.Log(timer_i);
            if (timer_i == 20 || Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(1);
            }
        }
        
    }
}
