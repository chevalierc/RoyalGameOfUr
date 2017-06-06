using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject tilePrefab;
    public GameObject piecePrefab;
    public GameObject bagPrefab;
    public GameObject rollDisplay;

    private Board board;
    private float tileWidth;

    private bool pieceIsMoving = false;
    private int rollValue;

    // Use this for initialization
    void Start() {
        board = new Board();
        tileWidth = (float)tilePrefab.GetComponent<Renderer>().bounds.size.x;
        initBoardTiles();
        createBags();
        roll();
    }

    public void onClick(Position clickPosition) {
        if (!pieceIsMoving) {
            if (board.isPlayerPiece(clickPosition)) {
                move(clickPosition, rollValue);
            }
        }
    }

    public void onBagClick() {
        GameObject newPiece = createPiece(false);
        moveNewPiece(newPiece, rollValue);
    }

    public void endTurn() {
        pieceIsMoving = false;
        roll();
    }

    private void roll() {
        rollValue = Random.Range(1, 5);
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = "Roll: " + rollValue;
    }

    private void moveNewPiece(GameObject newpiece, int numOfPlaces){
        if( board.isValidMove(null, numOfPlaces, false)) {
            Position end = board.getLandingPositionFrom(null, numOfPlaces, false);
            Queue<Position> directions = Board.getPathFrom(null, end, false);
            board.set(end, newpiece);
            newpiece.GetComponent<Piece>().move(directions);
            pieceIsMoving = true;
        }
    }
    private void move(Position start, int numOfPlaces) {
        if( board.isValidMove(start, numOfPlaces, false)) {
            Position end = board.getLandingPositionFrom(start, numOfPlaces, false);
            Queue<Position> directions = Board.getPathFrom(start, end, false);
            board.move(start, end);
            GameObject piece = board.get(end);
            piece.GetComponent<Piece>().move(directions);
            pieceIsMoving = true;
        }
    }

    public void createBags() {
        bool[] players = new bool[2]{ false, true};
        for(int i = 0; i < players.Length; i++) {
            bool isAi = players[i];
            Position position;
            if (isAi) {
                position = new Position(4, 3);
            } else {
                position = new Position(4, -1);
            }
            Vector3 location = position.toVector3() * tileWidth;
            GameObject newBag = bagPrefab;
            GameObject instance = Instantiate(newBag, location, Quaternion.identity) as GameObject;
            instance.GetComponent<Bag>().setAsAi(isAi);
            instance.GetComponent<Bag>().gameManager = this;
        }
    }

    private GameObject createPiece(bool forAi) {
        GameObject newPiece = piecePrefab;
        Position position;
        if (forAi) {
            position = new Position(4, 3);
        } else {
            position = new Position(4, -1);
        }
        Vector3 location = position.toVector3() * tileWidth;
        GameObject instance = Instantiate(newPiece, location, Quaternion.identity) as GameObject;
        instance.GetComponent<Piece>().position = new Position(3, 0);
        instance.GetComponent<Piece>().gameManager = this;
        instance.GetComponent<Piece>().isAi = forAi;
        return instance;
    }

    private void initBoardTiles() {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 3; y++) {
                if( (y == 0 || y == 2) && (x == 4 || x == 5) ){
                    continue; //dont place tiles at these places
                }

                GameObject newTile = tilePrefab;
                Vector3 location = new Vector3(x * tileWidth, y * tileWidth, 0f);
                GameObject instance = Instantiate(newTile, location, Quaternion.identity) as GameObject;
                instance.GetComponent<Tile>().position = new Position(x, y);
                instance.GetComponent<Tile>().gameManager = this;
            }
        }
    }


}
