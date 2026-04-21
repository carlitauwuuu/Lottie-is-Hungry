using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GridManager _gridManager;
    private GameObject _currentNenufar;
    private bool _ridingAutoMove;
    [SerializeField] private Color _deathmark;
    [SerializeField] private SpriteRenderer _renderer;

    [SerializeField] private float _maxNenufarIdleTime = 10f;

    private float _nenufarIdleTimer = 0f;
    private GameObject _lastNenufar;

    public bool touchedWater;
    public bool touchedFly;

    void Start()
    {
        touchedWater = false;
        _gridManager = FindAnyObjectByType<GridManager>();
    }

    void Update()
    {
        CheckVictory();

        Vector2 moveDir = Vector2.zero; // ------------------ SECCIÓN MOVIMIENTO


        if (Keyboard.current.wKey.wasPressedThisFrame)
            moveDir = Vector2.up;

        if (Keyboard.current.sKey.wasPressedThisFrame)
            moveDir = Vector2.down;

        if (Keyboard.current.aKey.wasPressedThisFrame)
            moveDir = Vector2.left;

        if (Keyboard.current.dKey.wasPressedThisFrame)
            moveDir = Vector2.right;

        if (moveDir != Vector2.zero)
        {
            ResetNenufarTimer();
            Move(moveDir);
        }

        Vector2 toungueDir = Vector2.zero; // ----------------- SECCIÓN LENGUA

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            toungueDir = Vector2.up;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            toungueDir = Vector2.down;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            toungueDir = Vector2.left;

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            toungueDir = Vector2.right;

        if (toungueDir != Vector2.zero)
        {
            ResetNenufarTimer();
            Toungue(toungueDir);
        }

        FollowNenufar(); // ---------------- movimiento CON NENUFAR
        HandleAutoNenufar(); // ----------- ni idea

        HandleNenufarIdle();

    }

    void ResetNenufarTimer()
    {
        _nenufarIdleTimer = 0f;
    }

    void Move(Vector2 direction)
    {
        Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);
        Vector2Int dir = Vector2Int.RoundToInt(direction);

        Vector2Int targetPos = currentPos + dir;
        Vector2Int nextPos = targetPos + dir;

        if (_gridManager.GetTileAtPosition(targetPos) == null)
            return;

        bool hasNenufar = _gridManager.HasNenufarAtPosition(targetPos);

        // 🟢 CASO 1: HAY NENÚFAR (subirse + posible empuje)
        if (hasNenufar)
        {
            GameObject n = _gridManager.GetNenufarAtPosition(targetPos);

            if (n == null || touchedFly)
                return;

            // 👉 guardar referencia para follow
            _currentNenufar = n;

            // 👉 activar auto-movimiento SOLO si quieres esa casilla especial
            // (luego puedes hacerlo por tag o data)
            if (IsAutoZone(targetPos))
                _ridingAutoMove = true;

            // ❌ si no puede empujar, solo subirse
            if (_gridManager.GetTileAtPosition(nextPos) == null ||
                _gridManager.HasRockAtPosition(nextPos) ||
                _gridManager.HasNenufarAtPosition(nextPos))
            {
                transform.position = n.transform.position;
                return;
            }

            // calcular cuánto puede avanzar el nenúfar
            int steps = CalculateTrip(targetPos, dir);

            // si no puede moverse
            if (steps <= 0)
            {
                transform.position = n.transform.position;
                return;
            }

            // nueva posición final del nenúfar
            Vector2Int finalPos = targetPos + dir * steps;

            // mover nenúfar (GRID primero)
            _gridManager.MoveNenufar(targetPos, finalPos);
            n.transform.position = new Vector3(finalPos.x, finalPos.y, n.transform.position.z);

            // player se sube a la casilla inicial del nenúfar
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

            return;
        }

        //  CASO 2: SIN NENÚFAR
        _currentNenufar = null;
        _ridingAutoMove = false;

        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

        //  agua mata
        if (!_gridManager.HasNenufarAtPosition(targetPos) && !_gridManager.HasRockAtPosition(targetPos))
        {
            touchedWater = true;
            _renderer.color = _deathmark;
        }
    }

    int CalculateTrip(Vector2Int startPos, Vector2Int dir)
    {
        int steps = 0;
        Vector2Int pos = startPos;

        while (true)
        {
            Vector2Int next = pos + dir;

            if (_gridManager.GetTileAtPosition(next) == null)
                break;

            if (_gridManager.HasRockAtPosition(next))
                break;

            GameObject n = _gridManager.GetNenufarAtPosition(next);

            // 🔥 CLAVE: ignorar el que estás moviendo
            if (n != null && n != _currentNenufar)
                break;

            steps++;
            pos = next;
        }

        return steps;
    }

    void FollowNenufar()
    {
        if (_currentNenufar == null)
            return;

        transform.position = _currentNenufar.transform.position;
    }

    void Toungue(Vector2 direction)
    {
        Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);

        Vector2Int targetPos = currentPos + Vector2Int.RoundToInt(direction);      // casilla cercana
        Vector2Int nenufarPos = currentPos + Vector2Int.RoundToInt(direction) * 2; // casilla lejana

        GameObject nenufar = _gridManager.GetNenufarAtPosition(nenufarPos);

        if (_gridManager.HasNenufarAtPosition(targetPos))
        return;

        else if (_gridManager.HasNenufarAtPosition(nenufarPos))
        {
            if (nenufar != null)
            {
                //Mover el nenúfar a la casilla cercana
                nenufar.transform.position = new Vector3(targetPos.x, targetPos.y, nenufar.transform.position.z);

                //actualizar el diccionario
                _gridManager.MoveNenufar(nenufarPos, targetPos);
            }
        }
    }

    void CheckVictory()
    {
        Vector2Int position = Vector2Int.RoundToInt(transform.position);
        if (_gridManager.HasFlyAtPosition(position))
        touchedFly = true;

        else
        return;
    }

    void HandleAutoNenufar()
    {
        if (!_ridingAutoMove || _currentNenufar == null)
            return;

        Vector2Int dir = Vector2Int.up;

        Vector2Int currentPos = _gridManager.GetNenufarPosition(_currentNenufar);

        // 🔥 1. SIMULACIÓN (ANTES de mover nada)
        int steps = CalculateTrip(currentPos, dir);

        if (steps <= 0)
        {
            _ridingAutoMove = false;
            return;
        }

        Vector2Int newPos = currentPos + dir * steps;

        // 🔥 2. ACTUALIZAR GRID PRIMERO
        _gridManager.MoveNenufar(currentPos, newPos);

        // 🔥 3. MOVER VISUAL
        _currentNenufar.transform.position = new Vector3(newPos.x, newPos.y, _currentNenufar.transform.position.z);
        // 🔥 4. PLAYER SIGUE
        transform.position = _currentNenufar.transform.position;
    }

    bool IsAutoZone(Vector2Int pos)
    {
        // por ahora hardcoded, luego lo puedes mejorar
        return pos == new Vector2Int(2, 2);
    }

    void HandleNenufarIdle()
{
    if (_currentNenufar == null)
    {
        _nenufarIdleTimer = 0f;
        _lastNenufar = null;
        return;
    }

    // Si cambiamos de nenúfar, reset
    if (_currentNenufar != _lastNenufar)
    {
        _nenufarIdleTimer = 0f;
        _lastNenufar = _currentNenufar;
        return;
    }

    // Contar tiempo
    _nenufarIdleTimer += Time.deltaTime;

    if (_nenufarIdleTimer >= _maxNenufarIdleTime)
    {
        Vector2Int pos = _gridManager.GetNenufarPosition(_currentNenufar);

        // quitar del grid
        _gridManager.RemoveNenufar(pos);

        // destruir objeto
        Destroy(_currentNenufar);

        // reset estado
        _currentNenufar = null;
        _lastNenufar = null;
        _nenufarIdleTimer = 0f;

        // el jugador cae al agua
        touchedWater = true;
        _renderer.color = _deathmark;
    }
}

}
