using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class kills : MonoBehaviour
{
    public TextMeshProUGUI killText;
    private int killCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateKillText();
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillText();
    }

    void UpdateKillText()
    {
        if (killText != null)
        {
            killText.text = "Kills: " + killCount;
        }
    }
}
