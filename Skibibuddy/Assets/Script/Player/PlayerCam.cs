using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    [SerializeField]public Avalanche avalanche;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Add a method to reset camera rotation
    public void ResetCameraRotation()
    {
        xRotation = 0f;
        yRotation = 0f; // 0 degrees on Y axis points to Z-forward in Unity world space
        transform.rotation = Quaternion.Euler(0, 0, 0);
        orientation.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (avalanche == null)
        {
            avalanche = FindObjectOfType<Avalanche>();
        }
    }
    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        
        if(avalanche != null && avalanche.isGameOver)
        {
            mouseVisible();
        }
    }
    public void mouseVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
