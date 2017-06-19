using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {
    public BoardManager boardManager;
    public Sprite enabledSprite;
    public Sprite disabledSprite;
    private bool isEnabled = true;

    void OnMouseDown() {
        if (isEnabled) {
            rollDice();
        }
    }

    public void setActive() {
        gameObject.GetComponent<SpriteRenderer>().sprite = enabledSprite;
        isEnabled = true;
    }

    public void setDisabled() {
        gameObject.GetComponent<SpriteRenderer>().sprite = disabledSprite;
        isEnabled = false;
    }

    public void rollDice() {
        int sum = 0;
        for(int i = 0; i < 4; i++) {
            sum += Mathf.RoundToInt( Random.Range(0, 2) );//believe it or not this will return 0 or 1 equally
        }
        boardManager.onDiceRoll(sum);
    }
}
