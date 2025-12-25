using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel; 
    public GameObject mainMenuButton;
    public GameObject quitButton;
    public Avalanche avalanche;

    private bool isPaused = false;
    private bool didHandleGameOver = false;

    void Start()
    {
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (avalanche != null && avalanche.isGameOver)
        {
            if (!didHandleGameOver)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (pausePanel != null) pausePanel.SetActive(true);
                if (mainMenuButton != null) mainMenuButton.SetActive(true);
                if (quitButton != null) quitButton.SetActive(true);

                didHandleGameOver = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (avalanche != null && avalanche.isGameOver)
            {
                if (pausePanel != null)
                {pausePanel.SetActive(true);}
                Debug.Log("gameover");
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}
