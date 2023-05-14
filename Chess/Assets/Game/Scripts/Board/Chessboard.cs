using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chessboard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] Sprite whiteColor;
    [SerializeField] Sprite blackColor;

    [SerializeField] Material tileMaterial;


    // LOGIC
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;

    public Vector2Int changeHover{
        get{
            return currentHover;
        }
        set{
            if(value.x >= -1 && value.x <= 7){
                if(value.y >= -1 && value.y <= 7){
                    currentHover = value;
                }
            }
        }
    }

    private void Awake()
    {
        GenerateAllTiles(0.75f, TILE_COUNT_X, TILE_COUNT_Y);
    }
    private void Update()
    {
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        tiles = new GameObject[tileCountX, tileCountY];
        bool _switch1 = true;
        bool _switch2 = false;
        Sprite color = null;
        for(int x = 0; x < tileCountX; x++){
            _switch1 = !_switch1;
            _switch2 = !_switch2;
            for(int y = 0; y < tileCountY; y++){
                _switch1 = !_switch1;
                if(_switch2 && _switch1){color = whiteColor;}
                if(!_switch2 && _switch1){color = whiteColor;}
                if(_switch2 && !_switch1){color = blackColor;}
                if(!_switch2 && !_switch1){color = blackColor;}
                tiles[x,y] = GenerateSingleTile(tileSize, x, y, color);
            }
        }
            
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y, Sprite color)
    {
        GameObject tileObjectHolder = new GameObject($"X:{x}, Y:{y}");
        GameObject tileObject = new GameObject($"X:{x}, Y:{y}");
        tileObjectHolder.transform.parent = transform;
        tileObject.transform.parent = tileObjectHolder.transform;

        tileObject.AddComponent<SpriteRenderer>().material = tileMaterial;
        tileObject.GetComponent<SpriteRenderer>().sprite = color;
        tileObject.transform.localScale = new Vector3(5.1f, 5.1f, 5.1f);

        tileObject.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
        tileObject.AddComponent<Tile>();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for(int x = 0; x < TILE_COUNT_X; x++)
            for(int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        
        return -Vector2Int.one; // Invalid
    }
}
