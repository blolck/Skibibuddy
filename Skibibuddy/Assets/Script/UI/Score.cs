using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Score : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; 
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Settings")]
    [SerializeField] private float scoreMultiplier = 10f; 
    [SerializeField] private float startZ = 0f;

    private float maxZ; 
    private float currentScore;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                PlayerController pc = FindObjectOfType<PlayerController>();
                if (pc != null) player = pc.transform;
            }
        }

        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }

        if (player != null)
        {
            if (startZ == 0f) startZ = player.position.z;
            maxZ = startZ;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z > maxZ)
        {
            maxZ = player.position.z;
        }
        currentScore = (maxZ - startZ) * scoreMultiplier;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("F0"); // F0 表示不保留小数
        }
    }
}
