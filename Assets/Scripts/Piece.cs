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

    public void move(Vector3 end) {
        StartCoroutine(SmoothMovement(end));
    }

    public IEnumerator SmoothMovement(Vector3 end) {
        float inverseMoveTime = 1f / moveTime;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

    }
}
