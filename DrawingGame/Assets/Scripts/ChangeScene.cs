using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    [SerializeField] private string OtherSceneName;
    void OnEnable()
    {
        // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
        SceneManager.LoadScene(OtherSceneName, LoadSceneMode.Single);
    }
}
