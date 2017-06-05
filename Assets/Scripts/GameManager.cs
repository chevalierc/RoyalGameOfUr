using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject tile;
    public GameObject piece;

    private GameObject[] board;
    private float tileWidth;

    // Use this for initialization
    void Start () {
        tileWidth = (float)tile.GetComponent<Renderer>().bounds.size.x;
        initBoardTiles();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClick(Position position) {
        Debug.Log("click");
    }

    private void initPieces() {
        GameObject newPiece = piece;
        newPiece.GetComponent<Tile>().position = new Position(0, 3);
        newPiece.GetComponent<Tile>().gameManager = this;

        Vector3 location = new Vector3(0 * tileWidth, 3 * tileWidth, 0f);
        GameObject instance = Instantiate(newPiece, location, Quaternion.identity) as GameObject;
    }

    private void initBoardTiles() {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 3; y++) {
                if( (y == 0 || y == 2) && (x == 4 || x == 5) ){
                    continue; //dont place tiles at these places
                }

                GameObject newTile = tile;
                newTile.GetComponent<Tile>().position = new Position(x, y);
                newTile.GetComponent<Tile>().gameManager = this;

                Vector3 location = new Vector3(x * tileWidth, y * tileWidth, 0f);
                GameObject instance = Instantiate(newTile, location, Quaternion.identity) as GameObject;
            }
        }
    }


}
