using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GridManager _gridManager;
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
        Vector2 moveDir = Vector2.zero;

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


        void Move(Vector2 direction)
        {
            Vector2 currentPos = Vector2Int.RoundToInt(transform.position);
            Vector2 targetPos = currentPos + direction;


            if (_gridManager.GetTileAtPosition(targetPos) == null) // pa q no salga del mapa ig
                return;

            if (_gridManager.HasNenufarAtPosition(targetPos) || _gridManager.HasRockAtPosition(targetPos)) //provisional pa ver si hay algo debajo
            {
                transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
                return;
            }
            else
            {
                _renderer.color = _deathmark;
                transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
                touchedWater = true;
                return;
            }

        }

    }

}
