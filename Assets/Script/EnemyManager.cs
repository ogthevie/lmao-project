using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    #region variables
    [SerializeField] MaskManager maskManager;

    // Référence à la cible (joueur)
    [SerializeField] Transform target;

    // Référence au NavMeshAgent pour le déplacement de l'ennemi
    public NavMeshAgent agent;

    // Référence à l'interface utilisateur de mort
    [SerializeField] GameObject deadUI;
    [SerializeField] GameManager gameManager;

    // Référence à l'icône du chasseur
    public GameObject iconHunter;

    // Position de base de l'ennemi
    public Vector3 basePosition;

    // Timer d'activation de l'ennemi et distance à la cible
    [SerializeField] float timerActiveself, distanceTarget;

    #endregion

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        maskManager = FindFirstObjectByType<MaskManager>();
        basePosition = transform.position;
        target = FindFirstObjectByType<MaskManager>().GetComponent<Transform>();
        iconHunter.SetActive(false);
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Calcule la distance entre l'ennemi et la cible
        distanceTarget = Vector3.Distance(this.transform.position, target.transform.position);
        
        if(!maskManager.isdead && gameManager.canPlay)
        {
            if(!iconHunter.activeSelf) CheckDistance();
            else
            {
                AjustSPeed();
                Chase();
            }
        }
    }

    // Vérifie la distance entre l'ennemi et la cible pour activer l'icône du chasseur
    void CheckDistance()
    {
        if(gameManager.timer >= timerActiveself || distanceTarget < 0.65f)
        {
            gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[6]);
            iconHunter.SetActive(true);
        }
    }

    // Ajuste la vitesse de l'agent en fonction du temps écoulé
    void AjustSPeed()
    {
        if(distanceTarget < 0.65f)
        {
            if(gameManager.timer > 60f) agent.speed = 0.25f;
            else if(gameManager.timer > 100f) agent.speed = 0.3f;
        }
        else
        {
            if(gameManager.timer > 30f) agent.speed = 0.25f;
            else if(gameManager.timer > 60f) agent.speed = 0.3f;
            else if(gameManager.timer > 100f) agent.speed = 0.35f; 
        }

    }

    // Méthode pour poursuivre la cible
    void Chase()
    {
        
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Vérifie si l'objet entrant appartient à la couche 3 (joueur) et que le temps n'est pas arrêté
        if(other.gameObject.layer == 3 && Time.timeScale != 0)
        {
            // Désactive l'icone du joueur
            GameObject mask = other.gameObject.GetComponent<Transform>().GetChild(0).gameObject;
            mask.SetActive(false);

            // Met à jour l'état de mort du joueur
            target.GetComponent<MaskManager>().isdead = true;

            // Affiche l'interface utilisateur de mort
            deadUI.SetActive(true);

            // Arrête et joue un son de mort
            gameManager.gameAudioSource.Stop();
            gameManager.gameAudioSource.loop = false;
            gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[2]);
            gameManager.runs += 1;
            
            //sauvegarde jeu
            gameManager.leaderboardManager.SaveOnline();
            gameManager.SaveGame();


            // Arrête le temps dans le jeu
            Time.timeScale = 0;
        }
    }
}
