using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _slide1;

    public void CloseMenu()
    {
        _menu.SetActive(false);
    }

    public void SkipSlide()
    {
        _slide1.SetActive(false);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToStart()
    {
        SceneManager.LoadScene ("Intro");
    }

}
