using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour {
    public Sprite aiSprite;
    public Sprite playerSprite;
    public bool isAi;
    public GameManager gameManager;

    private SpriteRenderer spriteRenderer;

    public void setAsAi(bool asAI) {
        if (asAI) {
            this.isAi = true;
            spriteRenderer.sprite = aiSprite;
        } else {
            this.isAi = false;
            spriteRenderer.sprite = playerSprite;
        }
    }

    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown() {
        gameManager.onBagClick();
        Debug.Log("yo");
    }


}
