using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class TextUI : MonoBehaviour
{
    public PlayerController player;
    private TextMeshProUGUI tmpText;
    private Rigidbody rb;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        if (player != null)
        {
            rb = player.GetComponent<Rigidbody>();
        }
    }
    void Update()
    {
     
        if (player == null) return;

        if (rb == null)
        {
            rb = player.GetComponent<Rigidbody>();
        }

        float speed = 0f;
        if (rb != null)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            speed = flatVel.magnitude;
        }

        string textContent = "Speed: " + speed.ToString("F1");

        if (tmpText != null)
        {
            tmpText.text = textContent;
        }
    }
}
