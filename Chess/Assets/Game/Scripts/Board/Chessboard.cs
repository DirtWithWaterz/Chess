using System;
using System.Collections;
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
    public List<ChessPiece> deadWhites = new List<ChessPiece>();
    public List<ChessPiece> deadBlacks = new List<ChessPiece>();
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
        GenerateShadow();
        
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        if(PhotonNetwork.LocalPlayer.IsMasterClient){SpawnAllPieces();}
        if(!PhotonNetwork.LocalPlayer.IsMasterClient){StartCoroutine(SetupClient());}
    }
    public bool masterSetupComplete = false;
    private IEnumerator SetupClient(){

        yield return new WaitUntil(() => masterSetupComplete);

        PositionAllPieces();
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

        int whiteTeam = 0, blackTeam = 1;
        // the names are created as follows
        // first int = white(0) or black(1)
        // second int = Bishop(1) or King(2) or Knight(3) or Pawn(4) or Queen(5) or Rook(6)
        // third int = instance number, ie. the first white rook placed is 0, the second is 1, then 2, 3, etc.
        // example: white bishop of the second instance = 011

        // White team
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam, "060", 0, 0);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam, "030", 1, 0);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam, "010", 2, 0);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam, "050", 3, 0);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam, "020", 4, 0);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam, "011", 5, 0);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam, "031", 6, 0);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam, "061", 7, 0);
        for(int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam, $"04{i}", i, 1);

        // Black team
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam, "160", 0, 7);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam, "130", 1, 7);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam, "110", 2, 7);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam, "150", 3, 7);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King, blackTeam, "120", 4, 7);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam, "111", 5, 7);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam, "131", 6, 7);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam, "161", 7, 7);
        for(int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam, $"14{i}", i, 6);

        PositionAllPieces();
        photonView.RPC(nameof(CompleteMasterSetup), RpcTarget.AllBufferedViaServer);

    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team, string name, int x, int y){
        ChessPiece cp = null;
        if(team == 0){
            cp = PhotonNetwork.Instantiate("Pieces/" + whitePrefabs[(int)type - 1].name, transform.position, Quaternion.identity).GetComponent<ChessPiece>();
        } else{
            cp = PhotonNetwork.Instantiate("Pieces/" + blackPrefabs[(int)type - 1].name, transform.position, Quaternion.identity).GetComponent<ChessPiece>();
        }

        int viewID = cp.gameObject.GetComponent<PhotonView>().ViewID;
        photonView.RPC(nameof(NamePiece), RpcTarget.AllBufferedViaServer, viewID, name, (int)type, team, x, y);

        return cp;
    }

    [PunRPC]
    public void CompleteMasterSetup(){

        Chessboard cb = GameObject.Find("Chessboard").GetComponent<Chessboard>();

        cb.masterSetupComplete = true;
    }

    [PunRPC]
    public void NamePiece(int viewID, string name, int type, int team, int x, int y){

        PhotonView view = PhotonView.Find(viewID);
        view.gameObject.name = name;

        chessPieces[x,y] = view.GetComponent<ChessPiece>();
        view.GetComponent<ChessPiece>().team = team;
        view.GetComponent<ChessPiece>().type = type == 1 ? ChessPieceType.Pawn : type == 2 ? ChessPieceType.Rook : type == 3 ? ChessPieceType.Knight : type == 4 ? ChessPieceType.Bishop : type == 5 ? ChessPieceType.Queen : type == 6 ? ChessPieceType.King : ChessPieceType.None;
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
        chessPieces[x,y].SetPosition(GetTileMatrix(x,y), x, y, force);
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
        
        photonView.RPC(nameof(SyncReset), RpcTarget.AllBufferedViaServer, photonView.ViewID);
    }

    [PunRPC]
    public void SyncReset(int boardViewID){

        Chessboard cb = PhotonView.Find(boardViewID).GetComponent<Chessboard>();

        // UI
        cb.victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        cb.victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        cb.victoryScreen.SetActive(false);

        // Fields reset
        cb.currentlyDragging = null;
        cb.availableMoves = new List<Vector2Int>();

        // Clean up
        for(int x = 0; x < cb.CONST_TILE_COUNT_X; x++){

            for(int y = 0; y < cb.CONST_TILE_COUNT_Y; y++){

                if(cb.chessPieces[x,y] != null)
                    Destroy(cb.chessPieces[x,y].gameObject);
                
                cb.chessPieces[x,y] = null;
            }
        }

        for(int i = 0; i < cb.deadWhites.Count; i++)
            Destroy(cb.deadWhites[i].gameObject);
        for(int i = 0; i < cb.deadBlacks.Count; i++)
            Destroy(cb.deadBlacks[i].gameObject);

        cb.deadWhites.Clear();
        cb.deadBlacks.Clear();

        if(PhotonNetwork.LocalPlayer.IsMasterClient){SpawnAllPieces();}
        if(!PhotonNetwork.LocalPlayer.IsMasterClient){StartCoroutine(SetupClient());}
        cb.iwt = true;
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
            photonView.RPC(nameof(SyncDeath), RpcTarget.OthersBuffered, photonView.ViewID, cp.GetComponent<PhotonView>().ViewID, ocp.GetComponent<PhotonView>().ViewID);
            if(cp.team == ocp.team)
                return false;
            
            // if it's the enemy team
            if(ocp.team == 0){

                if(ocp.type == ChessPieceType.King)
                    CheckMate(1);

                deadWhites.Add(ocp);
                ocp.isDead = true;
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
                ocp.isDead = true;
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(-1 * tileSize, 8 * tileSize, -zOffset) 
                - bounds 
                + new Vector3(tileSize / -2, tileSize / -2, 0) 
                + (Vector3.down * deathSpacing) * (deadBlacks.Count + deadOffset));
            }
        }

        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x,previousPosition.y] = null;

        cp.currentX = x;
        cp.currentY = y;
        PositionSinglePiece(x,y);

        photonView.RPC(nameof(SyncMove), RpcTarget.OthersBuffered, photonView.ViewID, cp.GetComponent<PhotonView>().ViewID, x, y, previousPosition.x, previousPosition.y);
        photonView.RPC(nameof(ToggleTurn), RpcTarget.AllBufferedViaServer, photonView.ViewID);

        return true;
    }

    [PunRPC]
    public void SyncMove(int boardViewID, int pieceViewID, int x, int y, int pX, int pY){

        Chessboard cb = PhotonView.Find(boardViewID).GetComponent<Chessboard>();
        ChessPiece cp = PhotonView.Find(pieceViewID).GetComponent<ChessPiece>();

        cb.chessPieces[x,y] = cp;
        cb.chessPieces[pX,pY] = null;

        cp.currentX = x;
        cp.currentY = y;
        cb.PositionSinglePiece(x,y);
    }
    [PunRPC]
    public void SyncDeath(int cbViewID, int cpViewID, int ocpViewID){
        
        Chessboard cb = PhotonView.Find(cbViewID).GetComponent<Chessboard>();
        ChessPiece cp = PhotonView.Find(cpViewID).GetComponent<ChessPiece>();
        ChessPiece ocp = PhotonView.Find(ocpViewID).GetComponent<ChessPiece>();

        if(cp.team == ocp.team)
            return;
            
        // if it's the enemy team
        if(ocp.team == 0){

            if(ocp.type == ChessPieceType.King)
                cb.CheckMate(1);

            cb.deadWhites.Add(ocp);
            ocp.SetScale(Vector3.one * cb.deathSize);
            ocp.SetPosition(new Vector3(8 * cb.tileSize, -1 * cb.tileSize, -cb.zOffset) 
            - cb.bounds 
            + new Vector3(cb.tileSize / 2, cb.tileSize / 2, 0) 
            + (Vector3.up * cb.deathSpacing) * (cb.deadWhites.Count + cb.deadOffset));
        }
        else if(ocp.team == 1){

            if(ocp.type == ChessPieceType.King)
                cb.CheckMate(0);

            cb.deadBlacks.Add(ocp);
            ocp.SetScale(Vector3.one * cb.deathSize);
            ocp.SetPosition(new Vector3(-1 * cb.tileSize, 8 * cb.tileSize, -cb.zOffset) 
            - cb.bounds 
            + new Vector3(cb.tileSize / -2, cb.tileSize / -2, 0) 
            + (Vector3.down * cb.deathSpacing) * (cb.deadBlacks.Count + cb.deadOffset));
        }
    }

    [PunRPC]
    public void ToggleTurn(int viewID){

        Chessboard cb = PhotonView.Find(viewID).GetComponent<Chessboard>();

        cb.iwt = !cb.iwt;
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
