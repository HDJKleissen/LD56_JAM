using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject PauseMenu;

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
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
            GameManager.Instance.QuitGame();
#endif
    }
}
