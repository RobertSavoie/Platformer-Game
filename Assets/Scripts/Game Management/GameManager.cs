using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Visislbe In Editor
    public GameObject player;
    public GameObject[] entrances;
    public GameObject[] exits;
    public GameObject[] bonfires;

    // Not Visible In Editor

    // Start is called before the first frame update
    private void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            SetGameObjects();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            SetGameObjects();
        }
    }

    public void LoadGame()
    {
        player.SetActive(true);
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Button Clicked");
        Application.Quit();
    }

    public void SetGameObjects()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        entrances = GameObject.FindGameObjectsWithTag("Entrance");
        exits = GameObject.FindGameObjectsWithTag("Exit");
        bonfires = GameObject.FindGameObjectsWithTag("Bonfire");
    }
}
