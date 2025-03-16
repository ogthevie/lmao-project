using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class CredentialsManager : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;
    public GameObject unityLogin, mainMenu;
    public TMP_InputField nameInput;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;
    }

    async void SignInWithUnity()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            #if UNITY_EDITOR
            Debug.Log("L'utilisateur est déjà connecté.");
            #endif

            Destroy(unityLogin, 0.01f);
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);

            Destroy(unityLogin, 0.01f);
            #if UNITY_EDITOR
            Debug.Log("Connexion réussie avec Unity.");
            #endif
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task SignInCachedUser()
    {
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            Debug.Log("Session token doesn't exist.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            mainMenu.SetActive(true);
            Destroy(unityLogin, 0.01f);

            await GetPlayerName();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex + ": Sign in failed!");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError(ex);
        }
    }

    public async Task SignIn()
    {
        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    string RemoveAfterHash(string input)
    {
        int hashIndex = input.IndexOf('#');
        
        if (hashIndex == -1)
            return input;
        
        return input[..hashIndex];
    }

    public async Task GetPlayerName()
    {
        try
        {
            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

            nameInput.text = RemoveAfterHash(playerName);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error getting PlayerName: " + ex);
        }
    }
}