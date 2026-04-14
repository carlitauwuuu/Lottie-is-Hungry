/* using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController02 : MonoBehaviour
{
    private GridManager _gridManager;
    private GameObject _currentNenufar;
    private bool _ridingAutoMove;
    [SerializeField] private Color _deathmark;
    [SerializeField] private SpriteRenderer _renderer;

    public bool touchedWater;

    void Start()
    {
        touchedWater = false;
        _gridManager = FindAnyObjectByType<GridManager>();
    }

    void Update()
    {

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
            Toungue(toungueDir);
        }

        FollowNenufar(); // ---------------- movimiento CON NENUFAR
        HandleAutoNenufar(); // ----------- ni idea

    }


    void Move(Vector2 direction)
    {
        Vector2Int dir = Vector2Int.RoundToInt(direction);
        Vector2Int currentPos = Vector2Int.RoundToInt(transform.position);

        Vector2Int target = currentPos + dir;

        if (!_gridManager.InBounds(target))
            return;

        GameObject nenufar = _gridManager.GetNenufarAt(target);

        // ❌ vacío o agua
        if (nenufar == null)
        {
            transform.position = target;
            return;
        }

        // 🟢 HAY NENUFAR → calcular cuánto puede moverse
        Vector2Int finalPos = CalculatePush(target, dir);

        // si no hay espacio
        if (finalPos == target)
        {
            transform.position = target;
            return;
        }

        // mover nenúfar
        nenufar.transform.position = new Vector3(finalPos.x, finalPos.y, 0);

        // mover player encima del primer paso
        transform.position = target;
    }


    Vector2Int CalculatePush(Vector2Int start, Vector2Int dir)
    {
        Vector2Int pos = start;

        while (true)
        {
            Vector2Int next = pos + dir;

            if (!_gridManager.InBounds(next))
                break;

            if (_gridManager.HasRock(next))
                break;

            if (_gridManager.HasNenufar(next))
                break;

            pos = next;
        }

        return pos;
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
        _currentNenufar.transform.position = new Vector3(newPos.x, newPos.y, 0);

        // 🔥 4. PLAYER SIGUE
        transform.position = _currentNenufar.transform.position;
    }

    bool IsAutoZone(Vector2Int pos)
    {
        // por ahora hardcoded, luego lo puedes mejorar
        return pos == new Vector2Int(2, 2);
    }

}  */
