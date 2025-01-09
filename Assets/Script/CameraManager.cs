using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    void Awake()
    {
        playerTransform = FindFirstObjectByType<MaskManager>().transform;
    }

    void Update()
    {
        updateCameraPosition();
    }

    void updateCameraPosition()
    {
        transform.position = new Vector3 (playerTransform.position.x, transform.position.y, playerTransform.position.z); 
    }
}
