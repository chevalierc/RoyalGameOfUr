using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMovieOnSpace : MonoBehaviour {
	void Update () {
		Renderer r = GetComponent<Renderer>();
		MovieTexture movie = (MovieTexture)r.material.mainTexture;
		movie.Play ();
	}
}

