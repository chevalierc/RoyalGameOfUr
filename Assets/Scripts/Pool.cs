using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool {
    private GameObject[] pool = new GameObject[6];
    public int count = 6;

    public GameObject getPiece() {
        for (int i = 0; i < 6; i++) {
            if(pool[i] != null) {
                GameObject piece = pool[i];
                pool[i] = null;
                count--;
                return piece;
            }
        }
        return null;
    }

    public int add(GameObject piece) {
        for (int i = 0; i < 6; i++) {
            if (pool[i] == null) {
                pool[i] = piece;
                count++;
                return i;
            }
        }
        return 0;
    }


}
