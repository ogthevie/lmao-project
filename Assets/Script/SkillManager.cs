using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    #region  variables

    // Référence au Slider pour la compétence Thunder
    public Slider skillThunderSlider;
    public Camera mainCamera;

    // Référence au GameObject button représentant le Thunder volant
    public GameObject flyingThunder, chain;
    public Button horusButton;
    public Image spark;

    // Référence au système de particules pour l'effet visuel du Thunder volant
    [SerializeField] ParticleSystem flyThunderFx;
    [SerializeField] GameManager gameManager;
    [SerializeField] MaskManager maskManager;
    [SerializeField] FXUIManager fXUIManager;

    // Variables pour gérer le temps de recharge de la compétence et les coordonnées du tonnerre volant
    public float currentTime, maxTime, xPos, zPos, horusTimer, baseSize = 0.4f, horusSize = 1f;
    float velocity = 1f;
    public bool isHorusActive;

    #endregion

    private void Awake()
    {
        skillThunderSlider = GetComponent<Slider>();
        maskManager = FindFirstObjectByType<MaskManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        mainCamera = FindFirstObjectByType<CameraManager>().GetComponent<Camera>();
        fXUIManager = FindFirstObjectByType<FXUIManager>();
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
        isHorusActive = false;
        chain.SetActive(false);
        horusButton.interactable = true;
        horusTimer = 10f;
    }

    private void Update() 
    {
        if(!gameManager.canPlay) return;
        
        float delta = Time.deltaTime;
        HandleTimerThunder(delta);
        CloseHorusCapacity(delta);
        horusInterpolation(delta);
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

    public void HorusCapacity()
    {
        isHorusActive = true;
        horusButton.interactable = false;
        chain.SetActive(true);
        fXUIManager.HorusEnter();
    }

    public void CloseHorusCapacity(float delta)
    {
        if(!isHorusActive) return;

        horusTimer -= 1*delta;

        if(horusTimer <= 0)
        {
            isHorusActive = false;
            fXUIManager.HorusExit();
        } 
    }

    public void horusInterpolation(float delta)
    {
        float smoothTime = 0.25f; // Ajustez cette valeur selon vos besoins

        if (isHorusActive && mainCamera.orthographicSize < 1f)
        {
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, horusSize, ref velocity, smoothTime);
        }
        else if (!isHorusActive && mainCamera.orthographicSize != 0.4f)
        {
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, baseSize, ref velocity, smoothTime);
        }
    }

    public void ResetHorusCapacity()
    {
        isHorusActive = false;
        mainCamera.orthographicSize = 0.4f;
        horusTimer = 10f;
        horusButton.interactable = true;
        chain.SetActive(false);
        fXUIManager.horusFx.SetActive(false);
    }
}
