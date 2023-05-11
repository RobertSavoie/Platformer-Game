using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Visible In Editor
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject resumeButton;

    // Not Visible In Editor
    InputSystem action;

    private void Awake()
    {
        action = new InputSystem();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void Start()
    {
        action.Player.PauseGame.performed += _ => DeterminePause();
    }

    private void DeterminePause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        paused = true;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        paused = false;
        pauseMenu.SetActive(false);
    }

    IEnumerable Test()
    {
        yield return new WaitForSecondsRealtime(3);
    }
}
