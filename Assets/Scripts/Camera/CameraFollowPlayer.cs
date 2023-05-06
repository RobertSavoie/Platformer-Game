using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineConfiner cinemachineConfiner;

    // Start is called before the first frame update
    private void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            cinemachineVirtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
            cinemachineConfiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraStop").GetComponent<PolygonCollider2D>();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            cinemachineVirtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
            cinemachineConfiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraStop").GetComponent<PolygonCollider2D>();
        }
    }
}
