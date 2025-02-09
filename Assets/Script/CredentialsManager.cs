using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class CredentialsManager : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn +=  SignInWithUnity;

        leaderboardManager.servicesInitialized = true;

        await SignInCachedUser();
    }

    async void SignInWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
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