using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Sprite blackSprite;
    public Sprite whiteSprite;
    public float moveTime = 0.1f;
    public PlayerColor color;
    public GameManager gameManager;

    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Queue<Position> directions;

    public void setColor(PlayerColor color) {
        this.color = color;
        if (color == PlayerColor.Black) {
            spriteRenderer.sprite = blackSprite;
        } else {
            spriteRenderer.sprite = whiteSprite;
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

    public void move(Vector3 end) {
        StartCoroutine(SmoothMovement(end));
    }

    private IEnumerator SmoothMovement(Vector3 end) {
        gameObject.GetComponent<Renderer>().sortingLayerName = "MovingPiece";
        float inverseMoveTime = 5f / moveTime;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon) {
            //get the inverse distance to middle
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime); //I think I dont understand this code and that why its off.
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        gameObject.GetComponent<Renderer>().sortingLayerName = "Piece";
        gameManager.endMove();
    }

    private IEnumerator SmoothMovement() {
        gameObject.GetComponent<Renderer>().sortingLayerName = "MovingPiece";
        Position endPosition = directions.Dequeue();
        Vector3 end = gameManager.positionToVector(endPosition);
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
            gameObject.GetComponent<Renderer>().sortingLayerName = "Piece";
            gameManager.endMove();
        }
    }

}
