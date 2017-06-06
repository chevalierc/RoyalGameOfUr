using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Position position;
    public Sprite aiSprite;
    public Sprite playerSprite;
    public float moveTime = 0.1f;
    public bool isAi = false;
    public GameManager gameManager;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Queue<Position> directions;

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
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void move(Queue<Position> directions) {
        this.directions = directions;
        StartCoroutine(SmoothMovement());
    }

    public IEnumerator SmoothMovement() {
        Position endPosition = directions.Dequeue();
        this.position = endPosition;
        Vector3 end = new Vector3((float)(endPosition.x * 1.28), (float)(endPosition.y * 1.28), 0);
        float inverseMoveTime = 1f / moveTime;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        Vector3 middle = ((Vector3)(rb2d.position) + end) / 2;
        while (sqrRemainingDistance > float.Epsilon) {
            //get the inverse distance to middle
            float inverseDistanceToMiddle = 1 / ( Mathf.Abs(rb2d.position.x - middle.x) + Mathf.Abs(rb2d.position.y - middle.y) ); //this feels a little off durring gameplay.
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime * inverseDistanceToMiddle); //I think I dont understand this code and that why its off.
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        if(directions.Count != 0) {
            StartCoroutine(SmoothMovement());
        }else {
            gameManager.endTurn();
        }

    }
}
