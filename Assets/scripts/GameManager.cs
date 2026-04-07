using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerControler;
    [SerializeField] private float restartTime = 5f;

    private bool _isRestarting = false;

    void Start()
    {

            if (_playerControler == null)
            {
                Debug.LogError("PlayerController no asignado en el inspector");
            }

    }

    void Update()
    {
        if (_playerControler.touchedWater && !_isRestarting)
        {
            _isRestarting = true;
            StartCoroutine(RestartAfterDelay());
        }

    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
