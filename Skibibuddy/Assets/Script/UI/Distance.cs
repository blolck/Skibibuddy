using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public PlayerController player;
    [SerializeField] public Avalanche avalanche;

    private TextMeshProUGUI tmpText;
    public float distance; 

    void Awake()
    {
   
        tmpText = GetComponent<TextMeshProUGUI>();
    }
    
    void Start()
    {
        if (player == null) 
            player = FindObjectOfType<PlayerController>();
        
        if (avalanche == null) 
            avalanche = FindObjectOfType<Avalanche>();
    }

    void Update()
    {
        if (player != null && avalanche != null && tmpText != null)
        {
            distance = player.transform.position.z - avalanche.transform.position.z;
            tmpText.text = " Distance to Avalanche: " + distance.ToString("F0") + "m";
        }
    }
}
