using UnityEngine;

public class FirstGameManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public CredentialsManager credentialsManager;


    public async void FirstStartGame()
    {
        await credentialsManager.SignIn();

        gameManager.primeScore = 0;
        gameManager.SaveGame();;
        
        gameManager.mainMenu.SetActive(true);
        Destroy(gameManager.unityLogin, 1.5f);
    }
}
