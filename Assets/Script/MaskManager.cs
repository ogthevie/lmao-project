using UnityEngine;

public class MaskManager : MonoBehaviour
{
    #region variables
    // Positions pour différents états ou points de contrôle
    [SerializeField] Vector3[] Position = new Vector3 [4];

    // Vitesse de déplacement du GameObject
    float moveSpeed = 0.6f;

    // Référence au script GameManager
    [SerializeField] GameManager gameManager;

    // Référence au joystick pour les entrées du joueur
    public FixedJoystick joystick;

    // Indicateurs pour contrôler l'activation du compteur de score et l'état de mort
    public bool canCount, isdead;

    // Référence au composant Rigidbody
    private Rigidbody rb;

    // Référence au GameObject pour l'effet de traînée
    [SerializeField] GameObject trailPlayer;

    #endregion

    // Méthode native appelée lorsque l'instance du script est chargée, ne necessite pas de réferencement
    void Awake()
    {
        HandlePosition();
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
        canCount = true;
        isdead = false;
    }

    // Méthode native appelée à chaque frame, ne necessite pas de réferencement
    void Update()
    {
        if(!gameManager.canPlay) return;
        Walk();
    }

    // Méthode native appelée à des intervalles de temps fixes, ne necessite pas de réferencement
    private void FixedUpdate()
    {
        if(rb.linearVelocity.magnitude > 0) trailPlayer.SetActive(true);
        else trailPlayer.SetActive(false);
    }

    // Gère le déplacement du joueur
    void Walk()
    {
        Vector3 movement = new Vector3(joystick.Horizontal, 0f, joystick.Vertical) * moveSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    //Gestion de l'apparition sur les différentes positions
    public void HandlePosition()
    {
        transform.position = Position[Random.Range(0,4)];
    }
}
