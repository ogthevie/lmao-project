using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] Vector3[] Position = new Vector3 [4];
    float moveSpeed = 0.6f;
    [SerializeField] GameManager gameManager;
    public FixedJoystick joystick;
    public bool canCount, isdead;
    private Rigidbody rb;

    void Awake()
    {
        HandlePosition();
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
        canCount = true;
        isdead = false;
    }

    void Update()
    {
        if(!gameManager.canPlay) return;
        Walk();
    }

    void Walk()
    {

        Vector3 movement = new Vector3(joystick.Horizontal, 0f, joystick.Vertical) * moveSpeed;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    /*void LateUpdate()
    {
        if(Input.GetKey(KeyCode.Escape))  Application.Quit();
        else if(Input.GetKey(KeyCode.F))  gameManager.ClearAllSaves();
    }*/

    public void HandlePosition()
    {
        transform.position = Position[Random.Range(0,4)];
    }
}
