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
    private string bonfire;
    private int climbingGloves;

    // Start is called before the first frame update
    private void Start()
    {
        bonfire = PlayerPrefs.GetString("BONFIRE");
        climbingGloves = PlayerPrefs.GetInt("CLIMBING_GLOVES");
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
        if(climbingGloves == 1)
        {
            player.GetComponent<Player>().climbingGloves = true;
        }
        if (bonfire != string.Empty)
        {
            SceneManager.LoadScene(bonfire);
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
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
