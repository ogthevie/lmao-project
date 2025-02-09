using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using System.Collections.Generic;

[DefaultExecutionOrder(-10)]
public class LeaderboardManager : MonoBehaviour
{
    private bool servicesInitialized = false;
    [SerializeField] GameManager gameManager;

    async void Awake()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            servicesInitialized = true;
            Debug.Log("Unity Services initialized successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'initialisation des services Unity ou de la connexion : " + e.Message);
        }
    }

    public async void AddScoreToLeaderboard(string playerName, int score)
    {
        if (!servicesInitialized)
        {
            Debug.LogError("Les services Unity ne sont pas initialisés.");
            return;
        }

        try
        {
            string playerId = playerName + "_" + System.DateTime.UtcNow.Ticks;
            var metadata = new Dictionary<string, object>
            {
                { "playerId", playerId }
            };
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Thief_Leaderboard_Dev", score, new AddPlayerScoreOptions { Metadata = metadata });
            Debug.Log("Score added to leaderboard successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'ajout du score au tableau de classement : " + e.Message);
        }
    }

    public async void GetPlayerRank(string playerName)
    {
        try
        {
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync("Thief_Leaderboard_Dev");
            int temprank = playerScore.Rank + 1;
            gameManager.rankOnline.text = temprank.ToString();
            Debug.Log($"{playerName} est classé {gameManager.rankOnline.text} ème.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la récupération du rang du joueur : " + e.Message);
        }
    }

    /*public async void DisplayLeaderboard()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync("highscore_leaderboard");
            foreach (var score in scores.Results)
            {
                // Afficher les scores
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'affichage du tableau de classement : " + e.Message);
        }
    }*/
}