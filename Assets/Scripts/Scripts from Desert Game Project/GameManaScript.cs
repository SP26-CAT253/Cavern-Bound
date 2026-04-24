using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManaScript : MonoBehaviour
{
    public GameObject GameOverUI;
    public GameObject GameWinUI;

    public void GameOver()
    {
        GameOverUI.SetActive(true);
    }
    public void GameWin()
    {
        GameWinUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}