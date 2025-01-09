using UnityEngine;
using TMPro;

public class DeadManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] MaskManager maskManager;
    [SerializeField] SkillManager skillManager;
    [SerializeField] string[] texts = new string[5];
    [SerializeField] TextMeshProUGUI trashTalkDead;
    [SerializeField] EnemyManager[] enemyManagers = new EnemyManager[4];
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        skillManager = FindFirstObjectByType<SkillManager>();
    }
    private void OnEnable()
    {   
        maskManager.canCount = false;
        if(gameManager.timer < 10f) trashTalkDead.text = texts[0];
        else if(gameManager.timer >= 10f && gameManager.timer < 30f) trashTalkDead.text = texts[1];
        else if(gameManager.timer >= 30f && gameManager.timer < 60f) trashTalkDead.text = texts[2];
        else if(gameManager.timer >= 60f && gameManager.timer < 90f) trashTalkDead.text = texts[3];
        else if(gameManager.timer >= 90f && gameManager.timer < 120f) trashTalkDead.text = texts[4];
        else if(gameManager.timer >= 120 ) trashTalkDead.text = texts[5];
    }
    private void OnDisable() 
    {
        maskManager.canCount = true;
    }

    void Start()
    {
        maskManager = FindFirstObjectByType<MaskManager>();
    }

    public void DeadReload()
    {
            foreach(EnemyManager var in enemyManagers)
            {
                var.GetComponent<Transform>().position = var.basePosition;
                var.iconHunter.SetActive(false);
                var.agent.speed = 0.2f;
            }
            gameManager.gameAudioSource.loop = false;
            gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[3]);
            maskManager.HandlePosition();
            maskManager.isdead = false;
            gameManager.timer = 0;
            gameManager.chronoVisual.text = "0";
            skillManager.currentTime = 0f;
            skillManager.skillThunderSlider.maxValue = skillManager.maxTime = 15f;
            skillManager.flyingThunder.SetActive(false);
            Time.timeScale = 1;
            GameObject mask = maskManager.transform.GetChild(0).gameObject;
            mask.SetActive(true);
            gameManager.LoadTentative();
            gameManager.ReloadMainTheme();
            this.gameObject.SetActive(false);
    }
}
