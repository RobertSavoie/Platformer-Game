using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            cinemachineVirtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
