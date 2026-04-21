using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private PlayerController _playerControler;
    [SerializeField] private float restartTime;
    [SerializeField] private float skipTime;

    private bool _isRestarting = false;
        private bool _isSkipping = false;

    IEnumerator Start()
    {
        yield return null; // espera 1 frame

        _playerControler = FindAnyObjectByType<PlayerController>();

        if (_playerControler == null)
        {
            Debug.LogError("PlayerController no encontrado");
        }
    }
    void Update()
    {
        if (_playerControler == null)
        {
            _playerControler = FindAnyObjectByType<PlayerController>();
            return;
        }

        if (_playerControler.touchedWater && !_isRestarting)
        {
            _isRestarting = true;
            StartCoroutine(RestartAfterDelay());
        }

        if (_playerControler.touchedFly && !_isSkipping)
        {
            _isSkipping = true;
            StartCoroutine(NextLevel());
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartTime);
        Debug.Log("Ha muerto!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(skipTime);
        Debug.Log("Ha ganado!");
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

}
