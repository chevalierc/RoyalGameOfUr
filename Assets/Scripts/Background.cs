using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    // Use this for initialization
    void Start() {
        ResizeSpriteToScreen();
    }

    // Update is called once per frame
    void Update() {

    }

    void ResizeSpriteToScreen() {
        var sr = gameObject.GetComponent<Renderer>();
        if (sr == null) return;

        gameObject.transform.localScale = new Vector3(1, 1, 1);

        float width = sr.bounds.size.x;
        float height = sr.bounds.size.y;

        float worldScreenHeight = (float) (Camera.main.orthographicSize * 2.0);
        float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

        float scaleRatio = Mathf.Max((worldScreenWidth / width), (worldScreenHeight / height));

        Debug.Log(worldScreenHeight);
        Debug.Log(height);

        gameObject.transform.localScale = new Vector3(scaleRatio, scaleRatio, 0);
    }

    public void setColorDarker() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public void setColorNormal() {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
