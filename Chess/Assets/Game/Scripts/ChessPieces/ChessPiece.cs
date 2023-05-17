using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviourPun
{
    public Chessboard chessboard;
    public int team;
    public int currentX;
    public int currentY;
    public Vector2Int currentPos;
    public ChessPieceType type;

    [SerializeField] private Vector3 desiredPosition;
    [SerializeField] private float positionOffset = 0.1f;
    private Vector3 desiredScale = Vector3.one * 5;

    Camera mainCamera;

    public Vector3 desiredPositionRef{
        get{
            return desiredPosition;
        }
    }
    public Vector3 desiredScaleRef{
        get{
            return desiredScale;
        }
    }

    private void Start(){
        mainCamera = Camera.main;
        chessboard = GameObject.Find("Chessboard").GetComponent<Chessboard>();

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
    }

    private void Update(){

        if(transform.localScale != desiredScale){
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 30);
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 30);
        }
        if((chessboard.draggingPiece != null && Mouse.current.leftButton.isPressed)){

            return;
        }
        // if(chessboard.draggingPiece.team == team){return;}
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 30);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 30);
        chessboard.RemoveHighlightTiles();
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY){

        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3,3));
        r.Add(new Vector2Int(3,4));
        r.Add(new Vector2Int(4,3));
        r.Add(new Vector2Int(4,4));

        return r;
    }

    public virtual void SetPosition(Vector3 position, bool force = false){

        desiredPosition = position + (PhotonNetwork.LocalPlayer.IsMasterClient ? (Vector3.up * positionOffset) : (Vector3.down * positionOffset));
        if(force)
            transform.position = desiredPosition;
    }
    public virtual void SetScale(Vector3 scale, bool force = false){

        desiredScale = scale;
        if(force)
            transform.localScale = desiredScale;
    }
}
