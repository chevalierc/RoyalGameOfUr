using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Position position;
    public Sprite aiSprite;
    public Sprite playerSprite;
    public float moveTime = 0.1f;
    public bool isAI = false;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Queue<Position> directions;

    public void setAsAi(bool asAI) {
        if (asAI) {
            this.isAI = true;
            spriteRenderer.sprite = aiSprite;
        } else {
            this.isAI = false;
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
        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        if(directions.Count != 0) {
            StartCoroutine(SmoothMovement());
        }

    }
}
