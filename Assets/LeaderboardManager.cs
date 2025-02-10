using UnityEngine;
using Unity.Services.Leaderboards;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Leaderboards.Models;
using Newtonsoft.Json;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
    }

    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject leaderBoardUI;
    [SerializeField] private Transform leaderboardsItem;
    [SerializeField] private Transform leaderboardsContentParent;

    private readonly string leaderboardID = "Thief_Leaderboard_Dev";

    public async void InitializeLeaderboards()
    {
        await UnityServices.InitializeAsync();
    }

    public async void AddScoreToLeaderboard(string playerName, int score)
    {
        try
        {
            string playerId = playerName;
            var metadata = new Dictionary<string, object>
            {
                { "playerId", playerId }
            };
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, score, new AddPlayerScoreOptions { Metadata = metadata });
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
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID);
            int temprank = playerScore.Rank + 1;
            gameManager.rankOnline.text = temprank.ToString();
            gameManager.primeScore = (int)playerScore.Score;
            gameManager.primeScoreVisual.text = playerScore.Score.ToString();
            Debug.Log($"{playerName} est classé {gameManager.rankOnline.text} ème.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la récupération du rang du joueur : " + e.Message);
        }
    }


    public void OpenLeaderBoardMenuAsync()
    {
        ClearLeaderboardsItems();
        FetchLeaderboardsData();
        leaderBoardUI.SetActive(true);
    }

    public void CloseLeaderBoardMenu()
    {
        leaderBoardUI.SetActive(false);
    }

    public void ClearLeaderboardsItems()
    {
        foreach (Transform transform in leaderboardsContentParent)
        {
            if (transform != null && transform.gameObject != null)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    private async void FetchLeaderboardsData()
    {
        LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID, new GetScoresOptions { IncludeMetadata = true });

        foreach (LeaderboardEntry leaderboardEntry in leaderboardScoresPage.Results)
        {
            Transform leaderboardItem = Instantiate(leaderboardsItem, leaderboardsContentParent);

            if (!string.IsNullOrEmpty(leaderboardEntry.Metadata))
            {
                try
                {
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(leaderboardEntry.Metadata);                    
                    leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerData.playerId;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to parse Metadata JSON: " + e.Message);
                }
            }
            else
            {
                leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.PlayerName;
            }

            leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = (leaderboardEntry.Rank + 1).ToString();
            leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = leaderboardEntry.Score.ToString();
        }
    }
}