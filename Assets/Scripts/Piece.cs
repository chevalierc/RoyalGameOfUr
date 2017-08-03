using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour {
    public Sprite blackSprite;
    public Sprite whiteSprite;
    public float moveTime = 0.5f;
    public PlayerColor color;
    public BoardManager boardManager;

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
        StartCoroutine(SmoothMovement(true));
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
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        gameObject.GetComponent<Renderer>().sortingLayerName = "Piece";
        boardManager.endMove();
    }

    private IEnumerator SmoothMovement(bool playSound) {
        gameObject.GetComponent<Renderer>().sortingLayerName = "MovingPiece";
        Position endPosition = directions.Dequeue();
        Vector3 end = boardManager.positionToVector(endPosition);
        float inverseMoveTime = 1f / moveTime;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float totalDistance = (Mathf.Abs(end.x - rb2d.position.x) + Mathf.Abs(end.y - rb2d.position.y));
        Vector3 middle = ((Vector3)(rb2d.position) + end) * .5f;
        float startScale = gameObject.transform.localScale.x;
        while (sqrRemainingDistance > float.Epsilon) {
            float distanceToMiddle = ( Mathf.Abs(middle.x - rb2d.position.x) + Mathf.Abs(middle.y - rb2d.position.y) );
            float ratioToMiddle = 1 - distanceToMiddle / (totalDistance * .5f); //0 is ends, 1 is middle
            gameObject.transform.localScale = new Vector3(startScale + ratioToMiddle, startScale + ratioToMiddle, 0);
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime); 
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
		//piece movement sound
		if (playSound) {
			Debug.Log("sound played");
			//play sound
			AudioSource audio = GetComponent<AudioSource>();
			//audio.loop = false;
			audio.Play();
		}
        if(directions.Count != 0) {
            StartCoroutine(SmoothMovement(true));
        }else {
            gameObject.GetComponent<Renderer>().sortingLayerName = "Piece";
            boardManager.endMove();
        }
    }

}
