using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Tile : MonoBehaviourPun
{
    Chessboard chessboard;
    Vector2Int pos;

    public Vector2Int BoardPosition{
        get{
            return pos;
        }
    }

    void Start(){
        chessboard = GameObject.Find("Chessboard").GetComponent<Chessboard>();
        pos = new Vector2Int(int.Parse(this.gameObject.name[2].ToString()), int.Parse(this.gameObject.name[7].ToString()));
    }

    void OnMouseOver(){

        this.gameObject.layer = LayerMask.NameToLayer("Hover");
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        chessboard.changeHover = new Vector2Int(int.Parse(this.gameObject.name[2].ToString()), int.Parse(this.gameObject.name[7].ToString()));
        Debug.Log(chessboard.changeHover);
    }
    void OnMouseExit(){

        this.gameObject.layer = LayerMask.NameToLayer("Tile");
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        chessboard.changeHover = -Vector2Int.one;
        Debug.Log(chessboard.changeHover);
    }
}
