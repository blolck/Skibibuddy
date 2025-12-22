using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartButton : MonoBehaviour
{
    [SerializeField] public itemSpawn itemSpawner;
    void Start()
    {
        if(itemSpawner == null)
        {
            itemSpawner = FindObjectOfType<itemSpawn>();
        }
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
    public void Quit()
    {
          Application.Quit();
    }
}
