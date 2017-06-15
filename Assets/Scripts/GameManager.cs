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
    public GameObject dicePrefab;

    private float tileWidth = 3.05f;
    private Vector2 boardOffset;
    private Board board;
    private float scaleRatio;
    private GameObject dice;


    private bool pieceIsMoving = false;
    private int rollValue;
    private Pool[] pools = new Pool[2] { new Pool(), new Pool() };
    private PlayerColor turn = PlayerColor.White;

    private PlayerColor[] PlayerColors = new[] { PlayerColor.Black, PlayerColor.White };

    private bool extraTurn = false;

    //capture piece
    bool capturedPieceThisMove = false;
    GameObject capturedPiece;


    // Use this for initialization
    void Start() {
        board = new Board();
        drawBoard();
        createPools();
        createDice();
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + " 's turn";
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

    public void onDiceRoll(int roll) {
        rollValue = roll;
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + " rolled a " + rollValue;
        dice.GetComponent<Dice>().setDisabled();
        if (roll == 0) {
            StartCoroutine(zeroRoll());
        }else if (board.hasNoMoves(turn, roll)) {
            StartCoroutine(zeroRoll());
        }
    }

    IEnumerator zeroRoll() {
        yield return new WaitForSeconds(2);
        endMove();
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
            } else if (board.isPieceOfPlayer(clickPosition, turn)) {
                move(clickPosition, rollValue, turn);
            }
        }
    }

    public void endMove() {
        if (capturedPieceThisMove == true) {
            moveCapturedPiece();
        } else if (extraTurn) {
            setupExtraTurn();
        }else { 
            dice.GetComponent<Dice>().setActive();
            endTurn();
        }
    }

    private void setupExtraTurn() {
        dice.GetComponent<Dice>().setActive();
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + "'s turn";
        pieceIsMoving = false;
        extraTurn = false;
    }

    private void endTurn() {
        pieceIsMoving = false;
        turn++;
        turn = (PlayerColor)((int)turn % 2);
        turn = PlayerColors[(int) turn];//there HAS to be a better way to write this
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + "'s turn";
    }

    private void move(Position start, int numOfPlaces, PlayerColor color) {
        if (board.isValidMove(start, numOfPlaces, color)) {
            //get piece to move
            GameObject piece = board.get(start);
            //get ending position
            Position end = board.getLandingPositionFrom(start, numOfPlaces, color);
            //get directions for piece
            Queue<Position> directions = Board.getPathFrom(start, end, color);
            //tell piece to move
            piece.GetComponent<Piece>().move(directions);
            pieceIsMoving = true;
            //check if landing on oponent piece
            if(board.isOponentPiece(end, color)) {
                capturePiece(end, color);
            }
            if (board.isRossete(end)) {
                extraTurn = true;
            }
            //move ref in gameBoard class
            board.move(start, end);
        } else {
            Debug.Log("Invalid move");
        }
    }

    private void capturePiece(Position capturePosition, PlayerColor color) {
        GameObject opponentPiece = board.get(capturePosition);
        capturedPieceThisMove = true;
        capturedPiece = opponentPiece;
        board.set(capturePosition, null);
    }

    private void moveCapturedPiece() {
        PlayerColor opponentColor = capturedPiece.GetComponent<Piece>().color;
        int placedAt = pools[(int)opponentColor].add(capturedPiece);
        Vector2 position;
        if (opponentColor == PlayerColor.Black) {
            position = new Vector2(4.5f + (placedAt / 5f), 0.5f);
        } else {
            position = new Vector2(4.5f + (placedAt / 5f), 2.5f);
        }
        Vector2 location = (position * tileWidth) - boardOffset;
        capturedPiece.GetComponent<Piece>().move(location);
        capturedPieceThisMove = false;
    }

    private void movePieceFromPool(int numOfPlaces, PlayerColor color){
        if (board.isValidMove(null, numOfPlaces, color) ){
            //get piece to move
            GameObject piece = pools[(int)color].getPiece();
            //get ending position
            Position end = board.getLandingPositionFrom(null, numOfPlaces, color);
            //get directions to end
            Queue<Position> directions = Board.getPathFrom(null, end, color);
            //set piece to move
            piece.GetComponent<Piece>().move(directions);
            pieceIsMoving = true;
            //set ref in gameBoard class
            board.set(end, piece);
        } else {
            Debug.Log("Invalid move");
        }

    }

    private void createDice() {
        float screenHeight = 2f * Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        float diceWidth = boardPrefab.GetComponent<Renderer>().bounds.size.x;
        float diceHeight = boardPrefab.GetComponent<Renderer>().bounds.size.y;

        Debug.Log(screenWidth);

        Vector2 location = new Vector2(-(screenWidth - diceWidth) / 2f,-(screenHeight- diceHeight) / 2f);
        location = Camera.main.ScreenToWorldPoint(location);

        GameObject dice = dicePrefab;
        GameObject instance = Instantiate(dice, location, Quaternion.identity) as GameObject;
        instance.GetComponent<Dice>().gameManager = this;
        //instance.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);
        this.dice = instance;
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
                instance.transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
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
        scaleRatio = Mathf.Min( (screenWidth/boardWidth), (screenHeight/boardWidth) );
        tileWidth = tileWidth * scaleRatio;
        float boardY = -((screenHeight - (scaleRatio * boardHeight)) / 2) + (tileWidth * 1.5f);

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
