using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class TextUI : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    private TextMeshProUGUI tmpText;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }
        tmpText = GetComponent<TextMeshProUGUI>();
     
    }

    void Update()
    {
     
        if (player == null) return;
        float speed = player.moveSpeed;
        string textContent = "Speed: " + speed.ToString("F1");

        if (tmpText != null)
        {
            tmpText.text = textContent;
        }
    }
}
