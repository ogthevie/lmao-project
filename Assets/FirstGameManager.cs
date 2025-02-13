using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerNameError, notifWriteName;
    public TMP_InputField playerNameInputField;
    public CredentialsManager credentialsManager;

    [SerializeField] GameManager gameManager;
    [SerializeField] Animation anim;
    [SerializeField] GameObject mainMenu;

    private void Start()
    {
        playerNameInputField.onValueChanged.AddListener(delegate { RemoveSpaces(); });    
    }

    void RemoveSpaces()
    {
        playerNameInputField.text = playerNameInputField.text.Replace(" ", "");
    }

    public void FirstStartGame()
    {
        if(playerNameError.activeSelf) return;

        if(playerNameInputField.text.Length <= 2) StartCoroutine (DisplayErrorMessage());
        else
        {
            gameManager.thiefPlayerName = playerNameInputField.text;
            gameManager.primeScore = 0;

            gameManager.SaveGame();
            gameManager.LoadGame();

            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);

            anim.enabled = true;

            credentialsManager.SignIn();

            mainMenu.SetActive(true);

            Destroy(this.gameObject, 5f);
        }
    }

    
    IEnumerator DisplayErrorMessage()
    {
        playerNameError.SetActive(true);
        notifWriteName.SetActive(false);

        yield return new WaitForSeconds(5f);

        notifWriteName.SetActive(true);
        playerNameError.SetActive(false);
    }
}
