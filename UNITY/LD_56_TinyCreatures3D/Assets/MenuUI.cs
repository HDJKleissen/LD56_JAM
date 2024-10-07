using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject VictoryMenu;
    public BossController boss;
    public bool checkForBoss;

    public void SetPaused(bool pause)
    {
        PauseMenu.SetActive(pause);

        Time.timeScale = pause ? 0 : 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(PauseMenu != null)
        {
            SetPaused(Time.timeScale == 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu != null && Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!PauseMenu.activeInHierarchy);
        }

        if (checkForBoss)
        {
            if(boss == null)
            {
                Time.timeScale = 1;
                VictoryMenu.SetActive(true);
            }
        }
    }

    public void LoadScene(string name)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }

    public void PlayClickSound()
    {

    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
