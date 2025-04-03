using UnityEngine;

public class OscillationManager : MonoBehaviour
{
    //choisir l'axe de deplacement
    //stocker la position de départ
    //boucler les oscillations
        //en fonction de l'axe, deplacer de -1.3 sur 0.5f
        //revenir sur la position de départ

    [SerializeField] bool zAxis, xAxis;
    public Vector3 basePosition;
    [SerializeField] float oscillationsSpeed;
    [SerializeField] float amplitude = -0.12f;
    private bool isOscillating = false;

    private void Start()
    {
        // Définir la position de départ
        basePosition = transform.position;  
        oscillationsSpeed = Random.Range(2, 3.5f);
    }
    
    private void Update()
    {
        if(isOscillating)
        {
            float oscillation = (Mathf.Sin(Time.time * oscillationsSpeed) + 1) / 2 * amplitude;

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
    public void StartOscillating()
    {
        isOscillating = true;
    }

    public void StopOscillating()
    {
        isOscillating = false;
        transform.position = basePosition;
    }
}
