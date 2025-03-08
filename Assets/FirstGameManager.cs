using Unity.Services.Leaderboards;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public CredentialsManager credentialsManager;


    public async void FirstStartGame()
    {
        gameManager.primeScore = 1;

        await credentialsManager.SignIn();

        gameManager.leaderboardManager.AddScoreToLeaderboard(gameManager.primeScore);
        var myPlayerData = await LeaderboardsService.Instance.GetPlayerScoreAsync(gameManager.leaderboardID);
        gameManager.dotroidPlayerName = myPlayerData.PlayerName.ToString();

        gameManager.SaveGame();
        gameManager.LoadGame();
        
        gameManager.mainMenu.SetActive(true);
        Destroy(gameManager.unityLogin, 1.5f);
    }
}
