using UnityEngine;
using TMPro;

public class DeadManager : MonoBehaviour
{
    #region variables
    GameManager gameManager;
    [SerializeField] MaskManager maskManager;
    [SerializeField] SkillManager skillManager;

    // Textes pour les messages de trash talk
    [SerializeField] string[] texts = new string[5];

    // Référence au composant TextMeshPro pour afficher le trash talk
    [SerializeField] TextMeshProUGUI trashTalkDead;
    [SerializeField] EnemyManager[] enemyManagers = new EnemyManager[4];

    #endregion
    
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        skillManager = FindFirstObjectByType<SkillManager>();
    }

    // Méthode native appelée lorsque l'objet devient actif, ne nécessite pas de réferencement
    private void OnEnable()
    {   

        // Désactive le comptage de score dans MaskManager
        maskManager.canCount = false;
        maskManager.dotroidThemeAudioSource.enabled = false;

        // Affiche le message de trash talk en fonction du temps écoulé
        if(gameManager.timer < 20f)
        {
            int index = Random.Range(0,9);
            trashTalkDead.text = texts[index];
        }
    }

    // Méthode native appelée lorsque l'objet devient inactif, ne nécessite pas de réferencement
    private void OnDisable() 
    {
        // Reactive le comptage de score dans MaskManager
        maskManager.canCount = true;
        maskManager.dotroidThemeAudioSource.enabled = true;
    }

    void Start()
    {
        maskManager = FindFirstObjectByType<MaskManager>();
    }

    // Méthode pour réinitialiser les ennemis après la mort, elle est appelée dans le component Button d'un GameObject de l'UI
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
            maskManager.HandleStartGamePosition();
            maskManager.isdead = false;
            gameManager.timer = 0;
            gameManager.chronoVisual.text = "0";
            skillManager.currentTime = 0f;
            skillManager.skillThunderSlider.maxValue = skillManager.maxTime = 15f;
            skillManager.flyingThunder.SetActive(false);
            skillManager.spark.enabled = false;
            Time.timeScale = 1;
            GameObject mask = maskManager.transform.GetChild(0).gameObject;
            gameManager.ResetWall();
            mask.SetActive(true);
            gameManager.LoadGame();
            gameManager.UpdateRanked();
            gameManager.phase.SetActive(false);
            this.gameObject.SetActive(false);
    }
}
