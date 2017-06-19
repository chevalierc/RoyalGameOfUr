using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private BoardManager boardManager;
    private bool is2p = false;
    private PlayerColor playerColor = PlayerColor.White;

	// Use this for initialization
	void Start () {
        boardManager = gameObject.GetComponent<BoardManager>();
	}

    private void Update() {
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
        if( boardManager.turn != playerColor) {
            boardManager.dice.GetComponent<Dice>().rollDice();
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
}
