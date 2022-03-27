using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 public enum ControllState 
    { 
        InMenu, 
        InGame
    }; //записувати так, бажано виносити в окремий публічний клас і окремий файл
public class MainMenu : MonoBehaviour
{
    public void TestLVL()
    {
        SceneManager.LoadScene("TestRoom");
    }

    public void QuitButton()
    {
        Debug.Log("Pressed Quit Button! App. will be closed!");
        Application.Quit();
    }
}
