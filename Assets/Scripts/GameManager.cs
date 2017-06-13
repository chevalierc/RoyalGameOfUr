using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerColor { Black, White};

public class GameManager : MonoBehaviour {
    public GameObject boardPrefab;
    public GameObject piecePrefab;
    public GameObject bagPrefab;
    public GameObject rollDisplay;

    private float tileWidth = 3f;
    private Vector2 boardOffset;
    private Board board;
    private float scaleRatio;

    private bool pieceIsMoving = false;
    private int rollValue;
    private Pool[] pools = new Pool[2] { new Pool(), new Pool() };
    private PlayerColor turn = PlayerColor.White;

    private PlayerColor[] PlayerColors = new[] { PlayerColor.Black, PlayerColor.White };


    // Use this for initialization
    void Start() {
        board = new Board();
        drawBoard();
        createPools();
        roll();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 clickLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickLocation = clickLocation + boardOffset;
            int x = Mathf.FloorToInt(clickLocation.x / tileWidth);
            int y = Mathf.FloorToInt(clickLocation.y / tileWidth);
            onClick(new Position(x, y));
        }
    }

    public Vector2 positionToVector(Position p) {
        Vector2 vec = p.toVector2();
        vec *= tileWidth;
        vec -= boardOffset;
        vec.x += (tileWidth * .5f);
        vec.y += (tileWidth * .5f);
        return vec;
    }

    public void onClick(Position clickPosition) {
        if (!pieceIsMoving) {
            if( clickPosition == new Position(4,2) || clickPosition == new Position(5,2)) {
                if(turn == PlayerColor.White) {
                    movePieceFromPool(rollValue, PlayerColor.White);
                }
            } else if (clickPosition == new Position(4, 0) || clickPosition == new Position(5, 0)) {
                if(turn == PlayerColor.Black) {
                    movePieceFromPool(rollValue, PlayerColor.Black);
                }
            } else if (board.isPlayerPiece(clickPosition, turn)) {
                move(clickPosition, rollValue, turn);
            }
        }
    }

    public void endTurn() {
        pieceIsMoving = false;
        turn++;
        turn = (PlayerColor)((int)turn % 2);
        turn = PlayerColors[(int) turn];//there HAS to be a better way to write this
        roll();
    }

    private void roll() {
        rollValue = Random.Range(1, 5);
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + "Roll: " + rollValue;
    }

    private void move(Position start, int numOfPlaces, PlayerColor color) {
        if (board.isValidMove(start, numOfPlaces, color)) {
            Position end = board.getLandingPositionFrom(start, numOfPlaces, color);
            Queue<Position> directions = Board.getPathFrom(start, end, color);
            board.move(start, end);
            GameObject piece = board.get(end);
            piece.GetComponent<Piece>().move(directions);
            pieceIsMoving = true;
        }else {
            Debug.Log("no");
        }
    }

    private void movePieceFromPool(int numOfPlaces, PlayerColor color){
        GameObject piece = pools[(int) color].getPiece();
        Position end = board.getLandingPositionFrom(null, numOfPlaces, color);
        Queue<Position> directions = Board.getPathFrom(null, end, color);
        board.set(end, piece);
        piece.GetComponent<Piece>().move(directions);
        pieceIsMoving = true;
    }

    public void createPools() {
        for (int p = 0; p < PlayerColors.Length; p++) {
            for (int i = 0; i < 6; i++) {
                PlayerColor color = PlayerColors[p];
                Vector2 position;
                if (color == PlayerColor.Black) {
                    position = new Vector2(4.5f + (i / 5f), 0.5f);
                } else {
                    position = new Vector2(4.5f + (i / 5f), 2.5f);
                }
                Vector2 location = (position * tileWidth) - boardOffset;
                GameObject newPiece = piecePrefab;
                GameObject instance = Instantiate(newPiece, location, Quaternion.identity) as GameObject;
                instance.GetComponent<Piece>().gameManager = this;
                instance.GetComponent<Piece>().setColor(color);
                instance.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);
                pools[(int)color].add(instance);
            }
        }
    }

    private void drawBoard() {
        float screenHeight = 2f * Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        float boardWidth = boardPrefab.GetComponent<Renderer>().bounds.size.x;
        float boardHeight = boardPrefab.GetComponent<Renderer>().bounds.size.y;

        //determine a scale for board
        scaleRatio = screenWidth / (boardWidth + 4);
        float boardY = 1 -( screenHeight - (scaleRatio * boardHeight) ) / 2;
        tileWidth = tileWidth * scaleRatio;

        //create board to right scale and position
        Vector3 location = new Vector3(0, boardY, 0);
        GameObject board = boardPrefab;
        GameObject boardInstance = Instantiate(board, location, Quaternion.identity) as GameObject;
        boardInstance.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);

        //determine offset of board so 0,0 is bottom left of board
        float offsetX = (boardInstance.GetComponent<Renderer>().bounds.size.x / 2);
        float offsetY = ( (boardInstance.GetComponent<Renderer>().bounds.size.y / 2) - boardY);
        boardOffset = new Vector2(offsetX, offsetY);
    }


}
