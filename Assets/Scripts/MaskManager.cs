using UnityEngine;

public class MaskManager : MonoBehaviour
{
    #region variables
    // Positions pour différents états ou points de contrôle
    [SerializeField] Vector3[] Position = new Vector3 [4];
    [SerializeField] GameObject maze, deadUI;

    // Vitesse de déplacement du GameObject
    float moveSpeed = 0.6f;

    // Référence au script GameManager
    [SerializeField] GameManager gameManager;

    // Référence au joystick pour les entrées du joueur
    public FloatingJoystick joystick;

    // Indicateurs pour contrôler l'activation du compteur de score et l'état de mort
    public bool canCount, isdead;

    // Référence au composant Rigidbody
    private Rigidbody rb;

    // Référence au GameObject pour l'effet de traînée
    public AudioSource dotroidThemeAudioSource;
    public StageManager stageManager;

    #endregion

    // Méthode native appelée lorsque l'instance du script est chargée, ne necessite pas de réferencement
    void Awake()
    {
        HandleStartGamePosition();
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
        canCount = true;
        isdead = false;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.layer == 6 && stageManager.wallOnFire) DeadProtocolCollisionLayer();
        else if(other.gameObject.layer == 9) DeadProtocolCollisionLayer();
    
    }

    // Méthode native appelée à chaque frame, ne necessite pas de réferencement
    void Update()
    {
        if(!gameManager.canPlay) return;
        
        VirtualJoystickWalk();
    }

    void DeadProtocolCollisionLayer()
    {
        transform.GetChild(0).gameObject.SetActive(false);
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

    // Méthode native appelée à des intervalles de temps fixes, ne necessite pas de réferencement

    // Gère le déplacement du joueur
    void VirtualJoystickWalk()
    {
        Vector3 movement = new Vector3(joystick.Horizontal, 0f, joystick.Vertical) * moveSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    //Gestion de l'apparition sur les différentes positions
    public void HandleStartGamePosition()
    {
        //int rotCoef = Random.Range(-3,4);
        //maze.transform.rotation = Quaternion.Euler(0, 90 * rotCoef, 0);
        transform.position = Position[Random.Range(0,4)];
    }


}
