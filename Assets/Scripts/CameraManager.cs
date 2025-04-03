using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Référence à la transformation du joueur
    [SerializeField] Transform playerTransform;

    void Awake()
    {
        playerTransform = FindFirstObjectByType<MaskManager>().transform;
    }

    void Update()
    {
        updateCameraPosition();
    }

    // Mise à jour la position de la caméra pour suivre le joueur
    void updateCameraPosition()
    {
        // La caméra suit la position x et z du joueur, mais garde sa propre position y
        transform.position = new Vector3 (playerTransform.position.x, transform.position.y, playerTransform.position.z); 
    }
}
