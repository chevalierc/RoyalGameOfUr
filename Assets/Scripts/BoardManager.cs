using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {
    private GameManager gameManager;
    public GameObject boardPrefab;
    public GameObject piecePrefab;
    public GameObject bagPrefab;
    public GameObject rollDisplay;
    public GameObject dicePrefab;

    public float tileWidth = 3.05f;
    public Vector2 boardOffset;
    public Board board;
    public GameObject[,] pieces;
    private float scaleRatio;
    public GameObject dice;

    private bool pieceIsMoving = false;
    public int rollValue = 0;
    public Pool[] startingPools = new Pool[2] { new Pool(), new Pool() };
    public Pool[] endingPools = new Pool[2] { new Pool(), new Pool() };
    public PlayerColor turn;

    public PlayerColor[] PlayerColors = new[] { PlayerColor.Black, PlayerColor.White };

    private bool extraTurn = false;
    private bool hasRolled;

    //capture piece
    bool capturedPieceThisMove = false;
    GameObject capturedPiece;

    //finish piece
    bool finishPieceThisMove = false;
    GameObject pieceToFinish;


    // Use this for initialization
    void Start() {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    public void setUpNewGame() {
        board = new Board();
        pieces = new GameObject[8, 3];
        turn = PlayerColor.Black; //opposite of who goes first
        scaleRatio = 0f;
        tileWidth = 3.05f;
        drawBoard();
        createPools();
        createDice();
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + " 's turn";
        endMove();
    }

    public void onDiceRoll(int roll) {
        rollValue = roll;
        hasRolled = true;
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + " rolled a " + rollValue;
        dice.GetComponent<Dice>().setDisabled();
        if (roll == 0) {
            StartCoroutine(zeroRoll());
        }else if (board.hasNoMoves(turn, roll)) {
            rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + " rolled a " + rollValue + " but has no legal moves.";
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

    //controll

    public void onClick(Position clickPosition) {
        if (!pieceIsMoving && rollValue != 0 && hasRolled) {
            if( clickPosition == new Position(2,3) || clickPosition == new Position(3,3)) {
                if(turn == PlayerColor.White) {
                    movePieceFromPool(rollValue, PlayerColor.White);
                }
            } else if (clickPosition == new Position(2, -1) || clickPosition == new Position(3, -1)) {
                if(turn == PlayerColor.Black) {
                    movePieceFromPool(rollValue, PlayerColor.Black);
                }
            } else if (board.isPieceOfPlayer(clickPosition, turn)) {
                move(clickPosition, rollValue, turn);
            }
        }
    }

    //turn managment

    public void endMove() {
        hasRolled = false;
        if (capturedPieceThisMove) {
            moveCapturedPiece();
        } else if (finishPieceThisMove) {
            moveFinishPiece();
        } else if (extraTurn) {
            extraTurn = false;
            endTurn();
        } else {
            turn = Board.otherColor(turn);
            endTurn();
        }
    }

    private void endTurn() {
        pieceIsMoving = false;
        rollDisplay.GetComponent<UnityEngine.UI.Text>().text = turn + "'s turn";
        gameManager.onEndTurn();
    }

    //piece movement

    private void movePieceFromPool(int numOfPlaces, PlayerColor color) {
        if (board.isValidMove(null, numOfPlaces, color)) {
            //get piece to move
            GameObject piece = startingPools[(int)color].getPiece();
            if(piece != null) {
                //get ending position
                Position end = board.getLandingPositionFrom(null, numOfPlaces, color);
                //get directions to end
                Queue<Position> directions = Board.getPathFrom(null, end, color);
                //set piece to move
                piece.GetComponent<Piece>().move(directions);
                pieceIsMoving = true;
                //check to see if landed on rosette
                if (Board.isRossete(end)) {
                    extraTurn = true;
                }
                //set ref in gameBoard class
                board.moveFromPool(end, color);
                pieces[end.x, end.y] = piece;
            }else {
                Debug.Log("No pieces left in pool");
            }
        } else {
            Debug.Log("Invalid move");
        }

    }

    private void move(Position start, int numOfPlaces, PlayerColor color) {
        if (board.isValidMove(start, numOfPlaces, color)) {
            //get piece to move
            GameObject piece = pieces[start.x, start.y];
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
            //check if landing on end
            if(Board.isEnd(end, color)) {
                finishPiece(piece);
            }
            if (Board.isRossete(end)) {
                extraTurn = true;
            }
			//piece landing sound
			//*******************
            //move ref in gameBoard class and pieces array
            board.move(start, end);
            pieces[end.x, end.y] = pieces[start.x, start.y];
            pieces[start.x, start.y] = null;
        } else {
            Debug.Log("Invalid move");
        }
    }

    //events

   private void finishPiece(GameObject piece) {
        finishPieceThisMove = true;
        pieceToFinish = piece;
    }

    private void capturePiece(Position capturePosition, PlayerColor color) {
        capturedPiece = pieces[capturePosition.x, capturePosition.y];
        capturedPieceThisMove = true;
    }

    private void moveFinishPiece() {
        PlayerColor color = pieceToFinish.GetComponent<Piece>().color;
        int placedAt = endingPools[(int)color].add(capturedPiece);
        Vector2 position;
        if (color == PlayerColor.Black) {
            position = new Vector2(6.5f + (placedAt / 6f), -0.5f);
            board.set(5, 0, PlayerColor.Free);
        } else {
            position = new Vector2(6.5f + (placedAt / 6f), 3.5f);
            board.set(5, 2, PlayerColor.Free);
        }
        Vector2 location = (position * tileWidth) - boardOffset;
        pieceToFinish.GetComponent<Piece>().move(location);
        finishPieceThisMove = false;
        //TODO: check for gameEnd
    }

    private void moveCapturedPiece() {
        PlayerColor opponentColor = capturedPiece.GetComponent<Piece>().color;
        int placedAt = startingPools[(int)opponentColor].add(capturedPiece);
        Vector2 position;
        if (opponentColor == PlayerColor.Black) {
            position = new Vector2(2.5f + (placedAt / 6f), -0.5f);
        } else {
            position = new Vector2(2.5f + (placedAt / 6f), 3.5f);
        }
        Vector2 location = (position * tileWidth) - boardOffset;
        capturedPiece.GetComponent<Piece>().move(location);
        capturedPieceThisMove = false;
    }

    //initialization

    private void createDice() {
        GameObject dice = dicePrefab;
        GameObject instance = Instantiate(dice, new Vector2(0,0), Quaternion.identity) as GameObject;

        Vector2 location = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        instance.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);
        location.x += instance.GetComponent<Renderer>().bounds.size.x / 2f;
        location.y += instance.GetComponent<Renderer>().bounds.size.y / 2f;
        instance.transform.position = location;

        instance.GetComponent<Dice>().boardManager = this;
        this.dice = instance;
    }

    public void createPools() {
        for (int p = 0; p < PlayerColors.Length; p++) {
            for (int i = 0; i < 7; i++) {
                PlayerColor color = PlayerColors[p];
                Vector2 position;
                if (color == PlayerColor.Black) {
                    position = new Vector2(2.5f + (i / 6f), -0.5f);
                } else {
                    position = new Vector2(2.5f + (i / 6f), 3.5f);
                }
                Vector2 location = (position * tileWidth) - boardOffset;
                GameObject newPiece = piecePrefab;
                GameObject instance = Instantiate(newPiece, location, Quaternion.identity) as GameObject;
                instance.GetComponent<Piece>().boardManager = this;
                instance.transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
                instance.GetComponent<Piece>().setColor(color);
                instance.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);
                startingPools[(int)color].add(instance);
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

    //end game
    public void removeAllGameObjects() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("GameObjects");
        for (var i = 0; i < gameObjects.Length; i++) {
            Destroy(gameObjects[i]);
        }
    }


}
