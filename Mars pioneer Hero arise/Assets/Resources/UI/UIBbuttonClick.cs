using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBbuttonClick : MonoBehaviour
{

    public void changeScene()
    {
        SceneManager.LoadScene("SpaceStop");
    }

    public void QuitApplacation()
    {
        Application.Quit();
    }

    public void setGameSetting()
    {
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
