using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    public GameObject playerNameError;
    public TextMeshProUGUI playerNameInputField;
    public CredentialsManager credentialsManager;

    [SerializeField] GameManager gameManager;
    [SerializeField] Animation anim;
    [SerializeField] GameObject mainMenu;

    public async void FirstStartGame()
    {
        if(playerNameInputField.text.Length <= 2)
        {
            await DisplayErrorMessageAsync();
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

    private async Task DisplayErrorMessageAsync()
    {
        playerNameError.SetActive(true);

        await Task.Delay(5000);

        playerNameError.SetActive(false);
    }
}
