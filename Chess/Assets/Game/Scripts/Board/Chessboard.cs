using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chessboard : MonoBehaviourPun
{
    [Header("Art Stuff")]
    [SerializeField] Sprite whiteColor;
    [SerializeField] Sprite blackColor;
    [SerializeField] Sprite shadowSprite;
    [SerializeField] public Sprite markerSprite;
    [SerializeField] Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    public float zOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 1f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float deadOffset = 0.5f;
    [SerializeField] private float dragOffset = 0.2f;
    [SerializeField] private GameObject victoryScreen;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] whitePrefabs;
    [SerializeField] private GameObject[] blackPrefabs;


    // LOGIC
    public ChessPiece[,] chessPieces;
    public GameObject shadow;
    private ChessPiece currentlyDragging;
    public List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    public Vector3 bounds;
    private Vector2 mousePos;
    private bool iwt;
    
    public bool isWhiteTurn{
        get{
            return iwt;
        }
    }
    public GameObject[,] publicTilesArray{
        get{
            return tiles;
        }
        set{
            tiles = value;
        }
    }
    public Vector2 mousePosition{
        get{
            return mousePos;
        }
    }
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
    public ChessPiece draggingPiece{
        get{
            return currentlyDragging;
        }
        set{
            currentlyDragging = value;
        }
    }
    public int CONST_TILE_COUNT_X{
        get{
            return TILE_COUNT_X;
        }
    }
    public int CONST_TILE_COUNT_Y{
        get{
            return TILE_COUNT_Y;
        }
    }

    private void Awake()
    {
        iwt = true;

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        SpawnAllPieces();
        PositionAllPieces();
        GenerateShadow();
    }

    private void Update()
    {
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            if(!PhotonNetwork.LocalPlayer.IsMasterClient)
                currentCamera.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));

            return;
        }
        mousePos = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        shadow.transform.position = new Vector3(
                mousePos.x, 
                mousePos.y + (dragOffset / 2),
                0.1f
            );
        if(Mouse.current.leftButton.isPressed && currentlyDragging != null){

            shadow.GetComponent<SpriteRenderer>().enabled = true;
            currentlyDragging.transform.position = Vector3.Lerp(
                currentlyDragging.transform.position, new Vector3(
                    mousePos.x, 
                    mousePos.y + (PhotonNetwork.LocalPlayer.IsMasterClient ? dragOffset : -dragOffset), 
                    -zOffset - 0.1f
                ), 
                Time.deltaTime * 30
            );
        } else{shadow.GetComponent<SpriteRenderer>().enabled = false;}
    }

    // Generate the shadow
    private void GenerateShadow()
    {
        shadow = new GameObject("Shadow");
        shadow.AddComponent<SpriteRenderer>();
        shadow.transform.parent = transform;
        shadow.transform.position = Vector3.zero;
        shadow.transform.localScale = Vector3.one * 5;
        shadow.GetComponent<SpriteRenderer>().sprite = shadowSprite;
        shadow.GetComponent<SpriteRenderer>().enabled = false;
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

        tileObject.layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
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
            cp = PhotonNetwork.Instantiate("Pieces/" + whitePrefabs[(int)type - 1].name, transform.position, Quaternion.identity).GetComponent<ChessPiece>();
        } else{
            cp = PhotonNetwork.Instantiate("Pieces/" + blackPrefabs[(int)type - 1].name, transform.position, Quaternion.identity).GetComponent<ChessPiece>();
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
    public void PositionSinglePiece(int x, int y, bool force = false){

        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPosition(GetTileMatrix(x,y), force);
    }
    
    // Highlight Tiles
    public void HighlightTiles()
    {
        for(int i = 0; i < availableMoves.Count; i++){
            Debug.Log("Current iteration: " + availableMoves[i]);
            Vector2Int aPos = availableMoves[i];
            Debug.Log($"Creating marker at: ({publicTilesArray[aPos.x,aPos.y].GetComponent<Tile>().pos.x},{publicTilesArray[aPos.x,aPos.y].GetComponent<Tile>().pos.y})");
            GameObject Marker = new GameObject($"{publicTilesArray[aPos.x, aPos.y].name} Highlight");
            Marker.transform.localScale = Vector3.one * 5;
            Marker.transform.parent = transform;
            Marker.AddComponent<SpriteRenderer>().sprite = markerSprite;
            Marker.transform.position = publicTilesArray[aPos.x, aPos.y].transform.position + (Vector3.back * 0.1f);
        }
    }
    public void RemoveHighlightTiles()
    {
        for(int i = 0; i < availableMoves.Count; i++){
            Debug.Log("Current iteration: " + availableMoves[i]);
            Vector2Int aPos = availableMoves[i];
            Debug.Log($"Destroying marker at: ({publicTilesArray[aPos.x,aPos.y].GetComponent<Tile>().pos.x},{publicTilesArray[aPos.x,aPos.y].GetComponent<Tile>().pos.y})");
            GameObject.Destroy(GameObject.Find($"{publicTilesArray[aPos.x, aPos.y].name} Highlight"));
            publicTilesArray[aPos.x, aPos.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }
    
    // Checkmate
    private void CheckMate(int team){

        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam){

        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton(){
        
        // UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        // Fields reset
        currentlyDragging = null;
        availableMoves = new List<Vector2Int>();

        // Clean up
        for(int x = 0; x < TILE_COUNT_X; x++){

            for(int y = 0; y < TILE_COUNT_Y; y++){

                if(chessPieces[x,y] != null)
                    PhotonNetwork.Destroy(chessPieces[x,y].gameObject);
                
                chessPieces[x,y] = null;
            }
        }

        for(int i = 0; i < deadWhites.Count; i++)
            PhotonNetwork.Destroy(deadWhites[i].gameObject);
        for(int i = 0; i < deadBlacks.Count; i++)
            PhotonNetwork.Destroy(deadBlacks[i].gameObject);
        
        // deadWhites.Clear();
        // deadBlacks.Clear();

        // SpawnAllPieces();
        // PositionAllPieces();
        photonView.RPC(nameof(ClearDead), RpcTarget.AllViaServer, photonView.ViewID);
        iwt = true;
    }
    [PunRPC]
    public void ClearDead(int viewID){

        PhotonView view = PhotonView.Find(viewID);
        Chessboard cb = view.GetComponent<Chessboard>();
        cb.deadWhites.Clear();
        cb.deadBlacks.Clear();
        cb.SpawnAllPieces();
        cb.PositionAllPieces();
    }


    public void OnExitButton(){

        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("MainMenu");
    }
    
    // Operations
    public bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos){

        for(int i = 0; i < moves.Count; i++)
            if(moves[i].x == pos.x && moves[i].y == pos.y)
                return true;
        
        return false;
    }
    public bool MoveTo(ChessPiece cp, int x, int y){

        if(!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
            return false;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        // Is there another piece on the target position?
        if(chessPieces[x,y] != null){

            ChessPiece ocp = chessPieces[x,y];

            if(cp.team == ocp.team)
                return false;
            
            // if it's the enemy team
            if(ocp.team == 0){

                if(ocp.type == ChessPieceType.King)
                    CheckMate(1);

                deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(8 * tileSize, -1 * tileSize, -zOffset) 
                - bounds 
                + new Vector3(tileSize / 2, tileSize / 2, 0) 
                + (Vector3.up * deathSpacing) * (deadWhites.Count + deadOffset));
            }
            else if(ocp.team == 1){

                if(ocp.type == ChessPieceType.King)
                    CheckMate(0);

                deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(-1 * tileSize, 8 * tileSize, -zOffset) 
                - bounds 
                + new Vector3(tileSize / -2, tileSize / -2, 0) 
                + (Vector3.down * deathSpacing) * (deadBlacks.Count + deadOffset));
            }
        }

        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x,previousPosition.y] = null;

        PositionSinglePiece(x,y);

        iwt = !iwt;

        return true;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for(int x = 0; x < TILE_COUNT_X; x++)
            for(int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        
        return -Vector2Int.one; // Invalid
    }
    public Vector3 GetTileMatrix(int x, int y){

        return new Vector3(x * tileSize, y * tileSize, -zOffset) - bounds;
    }

}
