using UnityEngine;

public class OscillationManager : MonoBehaviour
{
    //choisir l'axe de deplacement
    //stocker la position de départ
    //boucler les oscillations
        //en fonction de l'axe, deplacer de -1.3 sur 0.5f
        //revenir sur la position de départ

    [SerializeField] GameManager gameManager;
    [SerializeField] bool zAxis, xAxis;
    public Vector3 basePosition;
    [SerializeField] float oscillationsSpeed;
    [SerializeField] float amplitude = -0.12f;

    private void Start()
    {
        // Définir la position de départ
        basePosition = transform.position;  
        oscillationsSpeed = Random.Range(2, 3.5f);
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    private void Update()
    {
        if(!gameManager.wallOnMove) return;

        // Calculer la nouvelle position en fonction du temps et de l'oscillation
        float oscillation = (Mathf.Sin(Time.time * oscillationsSpeed) + 1) / 2 * amplitude;

        // Appliquer la nouvelle position en fonction de l'axe choisi
        if (xAxis)
        {
            transform.position = new Vector3(basePosition.x + oscillation, basePosition.y, basePosition.z);
        }
        else if (zAxis)
        {
            transform.position = new Vector3(basePosition.x, basePosition.y, basePosition.z + oscillation);
        }
    }

}
