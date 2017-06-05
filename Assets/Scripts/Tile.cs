using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public Position position;
    public GameManager gameManager;
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite optionSprite;

    private SpriteRenderer spriteRenderer;


    public void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setUnSelected();
    }

    void OnMouseDown() {
        gameManager.onClick(position);
    }

    public void setAsOption() {
        spriteRenderer.sprite = optionSprite;
    }

    public void setSelected() {
        spriteRenderer.sprite = selectedSprite;
    }

    public void setUnSelected() {
        spriteRenderer.sprite = defaultSprite;
    }

}
