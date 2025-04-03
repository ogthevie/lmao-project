using Unity.Services.Core;
using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public CredentialsManager credentialsManager;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        await credentialsManager.SignInCachedUser();
    }
    
    public async void FirstStartGame()
    {
        await credentialsManager.SignIn();

        gameManager.primeScore = 0;
        gameManager.SaveGame();;
    }
}
