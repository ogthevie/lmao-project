using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class CredentialsManager : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;
    public bool isLoginSuccessful = false;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;
        await SignInCachedUser();
        leaderboardManager.InitializeLeaderboards();
    }

    async void SignInWithUnity()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("L'utilisateur est déjà connecté.");
            isLoginSuccessful = true;
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            Debug.Log("Connexion réussie avec Unity.");
            isLoginSuccessful = true;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            isLoginSuccessful = false;
        }
    }

    void SignOutWithUnity()
    {
        PlayerAccountService.Instance.SignOut();
    }

    async Task SignInCachedUser()
    {
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
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

    public async void SignIn()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public void SignOut()
    {
        SignOutWithUnity();
    }
}