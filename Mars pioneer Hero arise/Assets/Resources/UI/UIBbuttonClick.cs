using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBbuttonClick : MonoBehaviour
{

    public void changeScene(int index)
    {
        SceneManager.LoadScene(index);
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
