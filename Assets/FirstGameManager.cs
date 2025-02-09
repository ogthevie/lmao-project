using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
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

            UpdatePlayerName();

            mainMenu.SetActive(true);

            Destroy(this.gameObject, 5f);
        }
    }

    private async void UpdatePlayerName()
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerNameInputField.text);
            Debug.Log("Player name has been updated successfully.");
        }
        catch (RequestFailedException)
        {
            Debug.Log("Error updating Player name.");
            await DisplayErrorMessageAsync();
            return;
        }
    }

    private async Task DisplayErrorMessageAsync()
    {
        playerNameError.SetActive(true);

        await Task.Delay(5000);

        playerNameError.SetActive(false);
    }
}
