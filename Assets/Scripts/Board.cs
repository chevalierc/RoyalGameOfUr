using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board  {
    private GameObject[,] tiles;

    public static Queue<Position> getPathFrom(Position start, Position end) {
        //return points the piece has to move to
        Queue<Position> directions = new Queue<Position>();
        Debug.Log(start);
        Debug.Log(end);
        if( start.y == 0) {
            if(end.y == 0) {
                directions.Enqueue(end);
            }else {
                directions.Enqueue(new Position(0, 0));
                directions.Enqueue(new Position(0, 1));
                if (end != new Position(0, 1)) {
                    directions.Enqueue(end);
                }
            }
        }else {
            directions.Enqueue(end);
        }
        return directions;
    }

}
