using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private BoardManager boardManager;
    private bool is2p = false;
    private PlayerColor playerColor = PlayerColor.White;

    bool gameIsBeingPlayed = false;

    GameObject menu;
    GameObject message;
    GameObject messageText;
    GameObject singlePlayerSetting;

    void Start () {
        boardManager = gameObject.GetComponent<BoardManager>();
        menu = GameObject.FindGameObjectWithTag("Menu");
        message = GameObject.FindGameObjectWithTag("Message");
        messageText = GameObject.FindGameObjectWithTag("MessageText");
        message.SetActive(false);
        singlePlayerSetting = GameObject.FindGameObjectWithTag("SinglePlayerSettings");
        singlePlayerSetting.SetActive(false);
    }

    public void startSinglePlayer() {
        is2p = false;
        menu.SetActive(false);
        singlePlayerSetting.SetActive(true);
    }

    public void startSinglePlayerAsWhite() {
        singlePlayerSetting.SetActive(false);
        playerColor = PlayerColor.White;
        gameIsBeingPlayed = true;
        boardManager.setUpNewGame();
    }

    public void startSinglePlayerAsBlack() {
        singlePlayerSetting.SetActive(false);
        playerColor = PlayerColor.Black;
        gameIsBeingPlayed = true;
        boardManager.setUpNewGame();
    }

    public void startMultiplayer() {
        is2p = true;
        menu.SetActive(false);
        gameIsBeingPlayed = true;
        boardManager.setUpNewGame();
    }

    private void Update() {
        if(!gameIsBeingPlayed) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            if (is2p) {
                passClickEvent();
            } else {
                if(boardManager.turn == playerColor) {
                    passClickEvent();
                }
            }
        }
    }

    public void onEndTurn() {
        checkForGameEnd();
        if (gameIsBeingPlayed) {
            if (boardManager.turn != playerColor && !is2p) {
                boardManager.dice.GetComponent<Dice>().setDisabled();
                doAiTurn();
            } else {
                boardManager.dice.GetComponent<Dice>().setActive();
            }
        }
        Debug.Log(boardManager.board.startingPoolCount[0]);
        Debug.Log(boardManager.board.startingPoolCount[1]);
    }

    private void doAiTurn() {
        boardManager.dice.GetComponent<Dice>().rollDice();
        if(boardManager.rollValue != 0) {
            Position bestClick = AI.getBestClick(boardManager, boardManager.turn);
            boardManager.onClick(bestClick);
        }
    }

    private void passClickEvent() {
        Vector2 clickLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickLocation = clickLocation + boardManager.boardOffset;
        int x = Mathf.FloorToInt(clickLocation.x / boardManager.tileWidth);
        int y = Mathf.FloorToInt(clickLocation.y / boardManager.tileWidth);
        boardManager.onClick(new Position(x, y));
    }

    private void checkForGameEnd() {
        for(var p = 0; p < boardManager.PlayerColors.Length; p++) {
            if(boardManager.endingPools[p].full) {
                gameIsBeingPlayed = false;
                if ((PlayerColor)p == PlayerColor.Black) {
                    showMenu("Black Has Won!");
                } else {
                    showMenu("White Has Won!");
                }
            }
        }
    }


    private void showMenu(string text) {
        message.SetActive(true);
        messageText.GetComponent<UnityEngine.UI.Text>().text = text;
    }

    public void exitGame() {
        message.SetActive(false);
        menu.SetActive(true);
        boardManager.removeAllGameObjects();
        boardManager.rollDisplay.GetComponent<UnityEngine.UI.Text>().text = "";
    }
}
