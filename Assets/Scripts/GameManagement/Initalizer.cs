using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalizer : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        StartCoroutine("WaitForMainMenuLoad");
    }

    IEnumerator WaitForMainMenuLoad()
    {
        while (GameObject.FindGameObjectWithTag("MainMenuPos") == null)
        {
            yield return null;
        }

        // Do anything after main menu scene has been loaded
        InitForMainMenuLoaded();

    }

    void InitForMainMenuLoaded()
    {
        Camera.main.GetComponent<MainMenuCameraController>().enabled = true;
        GameObject.FindGameObjectWithTag("Menus").GetComponent<MenusManager>().InitForMainMenuLoaded();
    }
}
