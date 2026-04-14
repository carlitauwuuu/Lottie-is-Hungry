using System.Collections.Generic;
using UnityEngine;

public class GridManager02 : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _nenufarPrefab;
    [SerializeField] private GameObject _rockPrefab;

    private List<GameObject> _nenufars = new();
    private List<GameObject> _rocks = new();
    private Dictionary<Vector2Int, Tile> _tiles = new();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        for (int y = 0; y < _height; y++)
        {
            var tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            _tiles[new Vector2Int(x, y)] = tile;
        }
    }

    public bool InBounds(Vector2Int pos)
    {
        return _tiles.ContainsKey(pos);
    }

    public bool HasRock(Vector2Int pos)
    {
        foreach (var r in _rocks)
            if (Vector2Int.RoundToInt(r.transform.position) == pos)
                return true;

        return false;
    }

    public GameObject GetNenufarAt(Vector2Int pos)
    {
        foreach (var n in _nenufars)
            if (Vector2Int.RoundToInt(n.transform.position) == pos)
                return n;

        return null;
    }

    public bool HasNenufar(Vector2Int pos)
    {
        return GetNenufarAt(pos) != null;
    }

    public void RegisterNenufar(GameObject n) => _nenufars.Add(n);
    public void RegisterRock(GameObject r) => _rocks.Add(r);
}
