using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject tile;
    public GameObject piecePrefab;

    private GameObject[] board;
    private float tileWidth;
    private GameObject piece; 

    // Use this for initialization
    void Start () {
        tileWidth = (float)tile.GetComponent<Renderer>().bounds.size.x;
        initBoardTiles();
        initPieces();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClick(Position clickPosition) {
        Position start = piece.GetComponent<Piece>().position;
        Queue<Position> directions = Board.getPathFrom(start, clickPosition);
        piece.GetComponent<Piece>().move(directions);
    }

    private void initPieces() {
        GameObject newPiece = piecePrefab;
        Vector3 location = new Vector3(3 * tileWidth, 0 * tileWidth, 0f);
        piece = Instantiate(newPiece, location, Quaternion.identity) as GameObject;
        piece.GetComponent<Piece>().position = new Position(3, 0);
    }

    private void initBoardTiles() {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 3; y++) {
                if( (y == 0 || y == 2) && (x == 4 || x == 5) ){
                    continue; //dont place tiles at these places
                }

                GameObject newTile = tile;
                Vector3 location = new Vector3(x * tileWidth, y * tileWidth, 0f);
                GameObject instance = Instantiate(newTile, location, Quaternion.identity) as GameObject;
                instance.GetComponent<Tile>().position = new Position(x, y);
                instance.GetComponent<Tile>().gameManager = this;
            }
        }
    }


}
