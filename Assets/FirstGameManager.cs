using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    public GameObject playerNameError;
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
        if(playerNameInputField.text.Length <= 2)
        {
        } else {
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

    //L'input field a été configuré de telle sorte qu'on ne puisse pas mettre d'espace et la longeur maximal de caractere soit 14
    /*private async Task DisplayErrorMessageAsync()
    {
        playerNameError.SetActive(true);

        await Task.Delay(5000);

        playerNameError.SetActive(false);
    }*/
}
