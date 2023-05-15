using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chessboard : MonoBehaviourPun
{
    [Header("Art Stuff")]
    [SerializeField] Sprite whiteColor;
    [SerializeField] Sprite blackColor;

    [SerializeField] Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float zOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] whitePrefabs;
    [SerializeField] private GameObject[] blackPrefabs;


    // LOGIC
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

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
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        SpawnAllPieces();
        PositionAllPieces();
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
        zOffset += transform.position.z;
        bounds = new Vector3((tileCountX / 2) * tileSize, (tileCountY / 2) * tileSize, 0) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        bool _switch1 = false;
        bool _switch2 = true;
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

        tileObject.transform.position = new Vector3(x * tileSize, y * tileSize, zOffset) - bounds;
        tileObject.AddComponent<Tile>();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider2D>();

        return tileObject;
    }

    // Spawning the pieces
    private void SpawnAllPieces(){

        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        // White team
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for(int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

        // Black team
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for(int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team){
        ChessPiece cp = null;
        if(team == 0){
            cp = Instantiate(whitePrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        } else{
            cp = Instantiate(blackPrefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        }

        cp.type = type;
        cp.team = team;

        return cp;
    }
    
    // Positioning
    private void PositionAllPieces(){

        for(int x = 0; x < TILE_COUNT_X; x++)
            for(int y = 0; y < TILE_COUNT_Y; y++)
                if(chessPieces[x,y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false){

        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].transform.position = new Vector3(x * tileSize, y * tileSize, -1) - bounds;
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
