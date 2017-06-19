using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private BoardManager boardManager;

	// Use this for initialization
	void Start () {
        boardManager = gameObject.GetComponent<BoardManager>();
	}

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 clickLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickLocation = clickLocation + boardManager.boardOffset;
            int x = Mathf.FloorToInt(clickLocation.x / boardManager.tileWidth);
            int y = Mathf.FloorToInt(clickLocation.y / boardManager.tileWidth);
            boardManager.onClick(new Position(x, y));
        }
    }
}
