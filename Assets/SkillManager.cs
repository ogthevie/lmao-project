using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public Slider skillThunderSlider;
    public GameObject flyingThunder;
    [SerializeField] ParticleSystem flyThunderFx;
    [SerializeField] GameManager gameManager;
    [SerializeField] MaskManager maskManager;
    public float currentTime, maxTime, xPos, zPos;

    private void Awake()
    {
        skillThunderSlider = GetComponent<Slider>();
        maskManager = FindFirstObjectByType<MaskManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    private void Start()
    {
        maxTime = skillThunderSlider.maxValue = 15f; //Discutable
        currentTime = skillThunderSlider.value = 0f; //discutable
        flyingThunder.SetActive(false);
    }

    private void Update() 
    {
        if(!gameManager.canPlay) return;
        
        float delta = Time.deltaTime;
        HandleTimerThunder(delta);
    }
    
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
            xPos = Random.Range(-3.49f, 3.49f);
            zPos = Random.Range(-3.49f, 3.49f);
        }
    }

    public void HandleFlyingThunder()
    { 
        maskManager.transform.position = new Vector3(xPos, maskManager.transform.position.y, zPos);
        xPos = zPos = 0f;
        currentTime = 0f;
        flyThunderFx.Play();
        maxTime += 10f;
        skillThunderSlider.maxValue = maxTime;
        flyingThunder.SetActive(false);

    }
}
