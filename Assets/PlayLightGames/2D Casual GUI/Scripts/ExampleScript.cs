using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExampleScript : MonoBehaviour
{
    [SerializeField]int sceneCount = 5;

    public void Advance()
    {
        if(SceneManager.GetActiveScene().buildIndex < sceneCount)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(0);

    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            Advance();
        }
    }
}
