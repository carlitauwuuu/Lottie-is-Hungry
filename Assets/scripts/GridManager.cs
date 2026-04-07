using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class NenufData
    {
        public Vector2 position;
    }

    [System.Serializable]
    public class RockData
    {
        public Vector2 position;
    }

    [SerializeField] private int _width, _height;
    [SerializeField] private Vector2 _playerStartPos = new Vector2(0, 0);
    [SerializeField] private Vector2 _flyPos = new Vector2(0, 0);


    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _nenufarPrefab;
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private GameObject _flyPrefab;
    [SerializeField] private GameObject _playerPrefab;


    [SerializeField] private List <NenufData> _nenufData;
    [SerializeField] private List<RockData> _rockData;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, GameObject> _nenufars;
    private Dictionary<Vector2, GameObject> _rocks;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        _nenufars = new Dictionary<Vector2, GameObject>();
        _rocks = new Dictionary<Vector2, GameObject>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        Instantiate(_playerPrefab, _playerStartPos, Quaternion.identity);
        Instantiate(_flyPrefab, _flyPos, Quaternion.identity);

        foreach (var rockData in _rockData)
        {
            SpawnRock(rockData.position);
        }

        foreach (var nenufData in _nenufData)
        {
            SpawnNenufar(nenufData.position);
        }


    }

    void SpawnNenufar(Vector2 pos)
    {
        var nenufObj = Instantiate(_nenufarPrefab, pos, Quaternion.identity);

        //var nenufar = nenufObj.GetComponent<Nenufar>();

        _nenufars[pos] = nenufObj;
    }

    void SpawnRock(Vector2 pos)
    {
        var rockObj = Instantiate(_rockPrefab, pos, Quaternion.identity);

        _rocks[pos] = rockObj;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    public bool HasRockAtPosition(Vector2 pos)
    {
        return _rocks.ContainsKey(pos);
    }

    public bool HasNenufarAtPosition(Vector2 pos)
    {
        return _nenufars.ContainsKey(pos);
    }
}