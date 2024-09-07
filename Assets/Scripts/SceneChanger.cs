using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void GoGameScene()
    {
        SceneManager.LoadScene("01_Game_Boss1");
    }
}
