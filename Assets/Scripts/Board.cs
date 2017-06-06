using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board  {
    private GameObject[,] pieces;
    private static Position[] playerPath;
    private static Position[] aiPath;

    public Board() {
        pieces = new GameObject[8, 3];
        playerPath = new Position[] {
            new Position(3,0),
            new Position(2,0),
            new Position(1,0),
            new Position(0,0),
            new Position(0,1),
            new Position(1,1),
            new Position(2,1),
            new Position(3,1),
            new Position(4,1),
            new Position(5,1),
            new Position(6,1),
            new Position(7,1),
            new Position(7,0),
            new Position(6,0),
        };
        aiPath = new Position[] {
            new Position(3,2),
            new Position(2,2),
            new Position(1,2),
            new Position(0,1),
            new Position(0,2),
            new Position(1,1),
            new Position(2,1),
            new Position(3,1),
            new Position(4,1),
            new Position(5,1),
            new Position(6,1),
            new Position(7,1),
            new Position(7,2),
            new Position(6,2),
        };
    }

    public GameObject get(Position pieceLocation) {
        if ( pieceLocation.x < this.pieces.GetLength(0) && pieceLocation.y < this.pieces.GetLength(1)) {
            return this.pieces[pieceLocation.x, pieceLocation.y];
        }else {
            return null;
        }
    }

    public void set(Position position, GameObject piece) {
        this.pieces[position.x, position.y] = piece;
    }

    public void move(Position start, Position end) {
        this.set(end, this.get(start));
        this.set(start, null);
    }

    public bool isPlayerPiece(Position location) {
        GameObject piece = this.get(location);
        if(piece != null && !piece.GetComponent<Piece>().isAi) {
            return true;
        }else {
            return false;
        }
    }

    public Position getLandingPositionFrom(Position start, int moves, bool isAi) {
        Position[] path;
        if (isAi) {
            path = aiPath;
        } else {
            path = playerPath;
        }

        int index;
        if (start == null) {
            index = -1;
        } else {
            index = System.Array.FindIndex(path, x => x == start); //find index in path of start position
        }

        index += moves;
        if (index < path.Length) {
            return path[index];
        }else {
            return null;
        }
        
    }

    public bool isValidMove(Position start, int moves, bool isAi) {
        //null piece location means picking from bag
        Position end = this.getLandingPositionFrom(start, moves,  isAi);

        if(end != null) {
            if(this.get(end) == null) {
                return true; //player/ai landing on free space
            } else if( isAi &&  this.get(end).GetComponent<Piece>().isAi) {
                return false; //ai trying to land on ai
            }else if (!isAi && !this.get(end).GetComponent<Piece>().isAi) {
                return false; //player trying to land on player
            }else {
                return true; //player/ai capturing piece
            }
        }else {
            return false; //trying to go off the board
        }
    }

    public static Queue<Position> getPathFrom(Position start, Position end, bool isAi) {
        //return points the piece has to move to
        Queue<Position> directions = new Queue<Position>();
        Position[] path;
        if (isAi) {
            path = aiPath;
        }else {
            path = playerPath;
        }
        int index = System.Array.FindIndex(path, x => x == start); //find index in path of start position
        Position currentPosition;
        do {
            index++;
            currentPosition = path[index];
            directions.Enqueue(currentPosition);
        } while (currentPosition != end);
        return directions;
    }

}
