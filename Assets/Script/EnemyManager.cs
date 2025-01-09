using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] MaskManager maskManager;
    [SerializeField] Transform target; // Définissez le joueur comme cible
    public NavMeshAgent agent;
    [SerializeField] GameObject deadUI;
    [SerializeField] GameManager gameManager;
    public GameObject iconHunter;
    public Vector3 basePosition;
    [SerializeField] float timerActiveself, distanceTarget;

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

    void CheckDistance()
    {
        if(gameManager.timer >= timerActiveself || distanceTarget < 0.65f) iconHunter.SetActive(true);
    }

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

    void Chase()
    {
        
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == 3 && Time.timeScale != 0)
        {
            GameObject mask = other.gameObject.GetComponent<Transform>().GetChild(0).gameObject;
            mask.SetActive(false);
            target.GetComponent<MaskManager>().isdead = true;
            deadUI.SetActive(true);
            gameManager.gameAudioSource.Stop();
            gameManager.gameAudioSource.loop = false;
            gameManager.gameAudioSource.PlayOneShot(gameManager.audioClips[2]);
            gameManager.SaveTentative();
            Time.timeScale = 0;
        }
    }
}
