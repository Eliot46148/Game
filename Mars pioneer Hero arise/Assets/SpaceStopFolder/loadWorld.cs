using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadWorld : MonoBehaviour
{
    public static string fileName;
    public static string SceneType;
    public int fileindex = 0;
    public int type = 0;
    private float timer_f = 0f;
    private int timer_i = 0;

    public void LoadVideo()
    {
        if(fileindex == 1)
        {
            fileName = "saveData.save";
        }
        else if(fileindex == 2)
        {
            fileName = "saveData2.save";
        }
        SceneType = "SpaceStop";
        SceneManager.LoadScene("startindvideo");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
