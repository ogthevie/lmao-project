using UnityEngine;

public abstract class StageManager : MonoBehaviour
{
    [SerializeField] protected MaskManager maskManager;
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected DeadManager deadManager;
    [SerializeField] public AudioSource stageAudiosource;
    public int stageIndex;
    public bool wallOnMove, wallOnFire;

    protected virtual void Awake()
    {
        maskManager = FindFirstObjectByType<MaskManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        stageAudiosource = GetComponent<AudioSource>();        
    }

    protected virtual void OnEnable()
    {
        maskManager.stageManager = this;
    }

    protected virtual void OnDisable()
    {
        //Tous les stages n-1 se désactive quand nous sommes sur le stage n;
    }

    protected abstract void Update();
}
