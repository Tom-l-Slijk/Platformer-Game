using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject selectedGameObject;

    public void OnEnable()
    {
        eventSystem.SetSelectedGameObject(selectedGameObject);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");
    }


    public void QuitGame()
    {
        Debug.Log("Game has Quit");
        Application.Quit();
    }
}
