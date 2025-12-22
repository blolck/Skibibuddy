using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Avalanche : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement Settings")]
    public float initialSpeed = 10f;
    public float acceleration = 0.5f;
    public float maxSpeed = 50f;
    
    private float currentSpeed;

    [Header("Game Over Settings")]
    public string gameOverSceneName = "GameOver"; // Or use active scene to restart
    public GameObject gameOverPanel; // UI Panel to show

    public bool isGameOver = false;

    void Start()
    {
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (isGameOver) return;

        MoveAvalanche();

        CheckGameOver();
    }

    void MoveAvalanche()
    {
        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
    }

    void CheckGameOver()
    {
        if (player == null) return;

        if (transform.position.z >= player.position.z)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
        }
        
    }
}
