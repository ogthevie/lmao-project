using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerNameError, notifWriteName;
    public TMP_InputField playerNameInputField;
    public CredentialsManager credentialsManager;

    [SerializeField] GameManager gameManager;
    [SerializeField] Animation anim;

    private readonly string leaderboardID = "Thief_Leaderboard_Dev";

    private void Start()
    {
        playerNameInputField.onValueChanged.AddListener(delegate { RemoveSpaces(); });    
    }

    void RemoveSpaces()
    {
        playerNameInputField.text = playerNameInputField.text.Replace(" ", "");
    }

    public async void FirstStartGame()
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

            try
            {
                await credentialsManager.SignIn();

                UpdatePlayerNameInLeadeboard(gameManager.thiefPlayerName);

                gameManager.mainMenu.SetActive(true);
                Destroy(this.gameObject, 5f);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }

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

    private async void UpdatePlayerNameInLeadeboard(string playerId)
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance
                .GetPlayerScoreAsync(
                    leaderboardID,
                    new GetPlayerScoreOptions { IncludeMetadata = true }
                );

            var metadata = new Dictionary<string, object>
            {
                { "playerId", playerId }
            };
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, scoreResponse.Score, new AddPlayerScoreOptions { Metadata = metadata });
            Debug.Log("Score added to leaderboard successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occured. " + ex);
        }
    }
}
