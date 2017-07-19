using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool {
    private GameObject[] pool = new GameObject[7];
    public int count = 0;
    public bool full = false;

    public GameObject getPiece() {
        for (int i = 0; i < 7; i++) {
            if(pool[i] != null) {
                GameObject piece = pool[i];
                pool[i] = null;
                count--;
                checkIfFull();
                return piece;
            }
        }
        return null;
    }

    public int add(GameObject piece) {
        for (int i = 0; i < 7; i++) {
            if (pool[i] == null) {
                pool[i] = piece;
                count++;
                checkIfFull();
                return i;
            }
        }
        return 0;
    }

    public void checkIfFull() {
        if(this.count == 7) {
            this.full = true;
        }else {
            this.full = false;
        }
    }


}
