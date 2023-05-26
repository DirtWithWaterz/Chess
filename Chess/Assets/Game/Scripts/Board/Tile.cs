using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
#pragma warning disable 0618

public class Tile : MonoBehaviourPun
{
    Chessboard chessboard;
    public Vector2Int pos;
    Color hoverColor;

    public Vector2Int BoardPosition{
        get{
            return pos;
        }
    }

    void Start(){
        hoverColor = new Color(0.75f,0.75f,0.75f,1);
        chessboard = GameObject.Find("Chessboard").GetComponent<Chessboard>();
        pos = new Vector2Int(int.Parse(this.gameObject.name[2].ToString()), int.Parse(this.gameObject.name[7].ToString()));
    }

    void OnMouseOver(){

        this.gameObject.layer = LayerMask.NameToLayer("Hover");
        this.gameObject.GetComponent<SpriteRenderer>().color = hoverColor;
        chessboard.changeHover = pos;
        // Debug.Log(chessboard.changeHover);

        // If the mouse button is released over me while holding a piece
        if(chessboard.draggingPiece != null && Mouse.current.leftButton.wasReleasedThisFrame){
            Debug.Log("Piece Discovered!");
            Vector2Int previousPosition = new Vector2Int(chessboard.draggingPiece.currentX, chessboard.draggingPiece.currentY);
            Debug.Log($"Telling {chessboard.name} to move {chessboard.draggingPiece.GetComponent<ChessPiece>().type} to position: ({pos.x},{pos.y})");
            bool validMove = chessboard.MoveTo(chessboard.draggingPiece, pos.x, pos.y);
            Debug.Log($"Valid? : {validMove}");
            if(!validMove)
                chessboard.draggingPiece.SetPosition(chessboard.GetTileMatrix(previousPosition.x,previousPosition.y), previousPosition.x, previousPosition.y);

            if(validMove){
                chessboard.draggingPiece.currentX = pos.x;
                chessboard.draggingPiece.currentY = pos.y;
            }
            chessboard.draggingPiece = null;
            chessboard.RemoveHighlightTiles();
        }
    }
    void OnMouseExit(){

        if(chessboard.changeHover != -Vector2Int.one){
            
            this.gameObject.layer = (chessboard.ContainsValidMove(ref chessboard.availableMoves, chessboard.changeHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            chessboard.changeHover = -Vector2Int.one;
        }
        // Debug.Log(chessboard.changeHover);

        if(chessboard.draggingPiece && Mouse.current.leftButton.wasReleasedThisFrame){

            chessboard.draggingPiece.SetPosition(chessboard.GetTileMatrix(chessboard.draggingPiece.currentX,chessboard.draggingPiece.currentY), chessboard.draggingPiece.currentX, chessboard.draggingPiece.currentY);
            chessboard.draggingPiece = null;
            chessboard.RemoveHighlightTiles();
        }
    }
    void OnMouseDown(){
        Debug.Log($"Tile ({pos.x},{pos.y}) was clicked!");
        if(chessboard.chessPieces[pos.x, pos.y] != null){
            Debug.Log("There is a chess piece here!");
            //is it our turn?
            if((chessboard.chessPieces[pos.x, pos.y].team == 0 && chessboard.isWhiteTurn && PhotonNetwork.LocalPlayer.IsMasterClient) || (chessboard.chessPieces[pos.x, pos.y].team == 1 && !chessboard.isWhiteTurn && !PhotonNetwork.LocalPlayer.IsMasterClient)){

                chessboard.draggingPiece = chessboard.chessPieces[pos.x, pos.y];

                // Get a list of where I can go and highlight those tiles
                chessboard.availableMoves = chessboard.draggingPiece.GetAvailableMoves(ref chessboard.chessPieces, chessboard.CONST_TILE_COUNT_X, chessboard.CONST_TILE_COUNT_Y);
                
                chessboard.HighlightTiles();
            }
        }
        else{Debug.Log("There is nothing here!");}
    }
}
#pragma warning restore 0618