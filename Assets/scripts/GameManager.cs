using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private PlayerController _playerControler;
    [SerializeField] private float restartTime;

    private bool _isRestarting = false;

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
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartTime);
        Debug.Log("Ha muerto!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
