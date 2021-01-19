using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    public EventSystem eventSystem;
    public GameObject selectedGameObject;

    public void OnEnable()
    {
        eventSystem.SetSelectedGameObject(selectedGameObject);
    }
}
