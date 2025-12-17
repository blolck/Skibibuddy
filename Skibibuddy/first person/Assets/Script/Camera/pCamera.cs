using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position+Vector3.up;
    }
}
