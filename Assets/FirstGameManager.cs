using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FirstGameManager : MonoBehaviour
{
    public TextMeshProUGUI playerNameInputField;
    [SerializeField] GameManager gameManager;
    [SerializeField] Animation anim;
    [SerializeField] GameObject mainMenu;
    bool onClick;

    public void FirstStartGame()
    {
        if(playerNameInputField.text.Length > 2 && !onClick)
        {
            onClick = true;
            gameManager.thiefPlayerName = playerNameInputField.text;
            gameManager.primeScore = 0;
            gameManager.SaveGame();
            gameManager.LoadGame();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            anim.enabled = true;
            mainMenu.SetActive(true);
            Destroy(this.gameObject, 5f);
        }
    }
}
