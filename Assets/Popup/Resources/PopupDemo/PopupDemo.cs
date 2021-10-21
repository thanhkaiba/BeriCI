using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDemo : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void RunScene (string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
