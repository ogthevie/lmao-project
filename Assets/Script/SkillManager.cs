using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    #region  variables

    // Référence au Slider pour la compétence Thunder
    public Slider skillThunderSlider;

    // Référence au GameObject button représentant le Thunder volant
    public GameObject flyingThunder;
    public Image spark;

    // Référence au système de particules pour l'effet visuel du Thunder volant
    [SerializeField] ParticleSystem flyThunderFx;
    [SerializeField] GameManager gameManager;
    [SerializeField] MaskManager maskManager;

    // Variables pour gérer le temps de recharge de la compétence et les coordonnées du tonnerre volant
    public float currentTime, maxTime, xPos, zPos;

    #endregion

    private void Awake()
    {
        skillThunderSlider = GetComponent<Slider>();
        maskManager = FindFirstObjectByType<MaskManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        // Définit le temps maximum et initialise le temps actuel
        maxTime = skillThunderSlider.maxValue = 15f; // Peut être ajusté
        currentTime = skillThunderSlider.value = 0f; // Peut être ajusté

        // Désactive le Thunder volant et l'effet d'étincelle
        flyingThunder.SetActive(false);
        spark.enabled = false;
        spark.enabled = false;
    }

    private void Update() 
    {
        if(!gameManager.canPlay) return;
        
        float delta = Time.deltaTime;
        HandleTimerThunder(delta);
    }

    // Gère le timer pour la compétence Thunder
    void HandleTimerThunder(float delta)
    {
        if(maskManager.isdead) return;

        if(currentTime < maxTime)
        {
            currentTime += 1 *delta;
            skillThunderSlider.value = currentTime;
        }
        else if(currentTime >= maxTime )
        {
            flyingThunder.SetActive(true);
            spark.enabled = true;
            xPos = Random.Range(-3.49f, 3.49f);
            zPos = Random.Range(-3.49f, 3.49f);
        }
    }

    //Logique de la compétence du Thunder volant
    public void HandleFlyingThunder()
    { 
        maskManager.transform.position = new Vector3(xPos, maskManager.transform.position.y, zPos);
        gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[7]);
        xPos = zPos = 0f;
        currentTime = 0f;
        flyThunderFx.Play();
        maxTime += 10f;
        skillThunderSlider.maxValue = maxTime;
        flyingThunder.SetActive(false);
        spark.enabled = false;
    }
}
