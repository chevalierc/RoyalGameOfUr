using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board  {
    private GameObject[,] pieces;
    private static Position[][] paths = new Position[2][];
    private static Position[] rosseteLocations;

    public Board() {
        pieces = new GameObject[8, 3];
        paths[(int)PlayerColor.Black] = new Position[] {
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
        paths[(int)PlayerColor.White] = new Position[] {
            new Position(3,2),
            new Position(2,2),
            new Position(1,2),
            new Position(0,2),
            new Position(0,1),
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
        rosseteLocations = new Position[] {
            new Position(0,0),
            new Position(0,2),
            new Position(3,1),
            new Position(6,0),
            new Position(6,2)
        };
    }

    public GameObject get(Position pieceLocation) {
        if ( pieceLocation.x < this.pieces.GetLength(0) && pieceLocation.y < this.pieces.GetLength(1) && pieceLocation.x >= 0 && pieceLocation.y >= 0) {
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

    public bool isRossete(Position location) {
        for(int i = 0; i < rosseteLocations.Length; i++) {
            if(location == rosseteLocations[i]) {
                return true;
            }
        }
        return false;
    }

    public bool isPieceOfPlayer(Position location, PlayerColor color) {
        GameObject piece = this.get(location);
        if (piece != null && piece.GetComponent<Piece>().color == color) {
            return true;
        }else {
            return false;
        }
    }

    public bool isOponentPiece(Position location, PlayerColor color) {
        GameObject piece = this.get(location);
        if (piece != null && piece.GetComponent<Piece>().color != color) {
            return true;
        } else {
            return false;
        }
    }

    public Position getLandingPositionFrom(Position start, int moves, PlayerColor color) {
        Position[] path = paths[(int)color];

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

    public bool hasNoMoves(PlayerColor color, int moves) {
        for(int x = 0; x < pieces.GetLength(0); x++) {
            for (int y = 0; y < pieces.GetLength(1); y++) {
                if(isValidMove(new Position(x,y), moves, color)) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool isValidMove(Position start, int moves, PlayerColor color) {
        //null piece location means picking from bag
        Position end = this.getLandingPositionFrom(start, moves, color);
        if(end != null) {
            if (this.get(end) == null) {
                return true; // landing on free space
            } else if (this.get(end).GetComponent<Piece>().color == color) {
                return false; //landing on same color
            }else if (this.isRossete(end) && this.get(end) != null) {
                return false; //landing on opponent (or self) on rosset
            }else {
                return true; //capturing piece
            }
        }else {
            return false; //trying to go off the board
        }
    }

    public static Queue<Position> getPathFrom(Position start, Position end, PlayerColor color) {
        //return points the piece has to move to
        Queue<Position> directions = new Queue<Position>();
        Position[] path = paths[(int)color];
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
