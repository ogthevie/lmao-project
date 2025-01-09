using System.Collections;
using UnityEngine;

public class CompleteManager : MonoBehaviour
{
    [SerializeField] GameObject success;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            StartCoroutine (CompleteGame());
        }
    }

    IEnumerator CompleteGame()
    {
        Time.timeScale = 0;
        success.SetActive(true);
        yield return new WaitForSeconds (30f);
        Application.Quit();
    }
}
