using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor { Black, White, Free };

public class Board  {
    //pieces
    private PlayerColor[,] pieces;
    public int[] startingPoolCount;
    public int[] endingPoolCount;

    //static facts
    private static Position[][] paths;
    private static Position[] rosseteLocations;

    static Board() {
        paths = new Position[2][];
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
            new Position(5,0)
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
            new Position(5,2)
        };
        rosseteLocations = new Position[] {
            new Position(0,0),
            new Position(0,2),
            new Position(3,1),
            new Position(6,0),
            new Position(6,2)
        };
    }

    //static
    public static bool isEnd(Position position, PlayerColor color) {
        int pathLength = paths[(int)PlayerColor.Black].Length;
        if (paths[(int)color][pathLength - 1] == position) {
            return true;
        }
        return false;
    }

    public static bool isGoal(Position location) {
        if (location == new Position(5, 2) || location == new Position(5, 0)) {
            return true;
        }
        return false;
    }

    public static bool isRossete(Position location) {
        for (int i = 0; i < rosseteLocations.Length; i++) {
            if (location == rosseteLocations[i]) {
                return true;
            }
        }
        return false;
    }

    public static PlayerColor otherColor(PlayerColor color) {
        if(color == PlayerColor.Black) {
            return PlayerColor.White;
        }else {
            return PlayerColor.Black;
        }
    }

    //constructors
    public Board() {
        pieces = new PlayerColor[8, 3];
        for (int x = 0; x < pieces.GetLength(0); x++) {
            for (int y = 0; y < pieces.GetLength(1); y++) {
                pieces[x, y] = PlayerColor.Free;
            }
        }
        startingPoolCount = new int[]{7, 7};
        endingPoolCount = new int[]{0, 0};
    }

    public Board(Board oldBoard) {
        pieces = new PlayerColor[8, 3];
        for (int x = 0; x < pieces.GetLength(0); x++) {
            for (int y = 0; y < pieces.GetLength(1); y++) {
                pieces[x, y] = oldBoard.get(x,y);
            }
        }
        startingPoolCount = new int[] {7, 7 };
        endingPoolCount = new int[] {0, 0 };
        startingPoolCount[(int)PlayerColor.Black] = oldBoard.startingPoolCount[(int)PlayerColor.Black];
        startingPoolCount[(int)PlayerColor.White] = oldBoard.startingPoolCount[(int)PlayerColor.White];
        endingPoolCount[(int)PlayerColor.Black] = oldBoard.endingPoolCount[(int)PlayerColor.Black];
        endingPoolCount[(int)PlayerColor.White] = oldBoard.endingPoolCount[(int)PlayerColor.White];
    }

    //getter/setters

    public PlayerColor get(int x, int y) {
        if (x < this.pieces.GetLength(0) && y < this.pieces.GetLength(1) && x >= 0 && y >= 0) {
            return this.pieces[x, y];
        } else {
            return PlayerColor.Free;
        }
    }

    public PlayerColor get(Position pieceLocation) {
        return this.get(pieceLocation.x, pieceLocation.y);
    }

    public void set(Position position, PlayerColor piece) {
        this.pieces[position.x, position.y] = piece;
    }

    public void set(int x, int y, PlayerColor piece) {
        this.pieces[x, y] = piece;
    }

    public void aiMove(Position start, Position end, PlayerColor color) {
        if( start == new Position(2, -1) || start == new Position(2, 3) ) {
            moveFromPool(end, color);
        }else {
            move(start, end);
        }
    }

    public void move(Position start, Position end) {
        if(this.get(end) != PlayerColor.Free){
            startingPoolCount[(int)this.get(end)]++;
        }
        if(Board.isGoal(end)){
            endingPoolCount[(int)this.get(start)]++;
        }else{
            this.set(end, this.get(start) );
        }
        this.set(start, PlayerColor.Free);
    }

    public void moveFromPool(Position end, PlayerColor color){
        this.set(end, color);
        startingPoolCount[(int)color]--;
    }

    public bool isWin(PlayerColor color) {
        if(this.endingPoolCount[(int) color] == 7) {
            return true;
        }else {
            return false;
        }
    }

    public int distanceFromStart(Position position, PlayerColor color) {
        Position[] path = paths[(int)color];
        return System.Array.FindIndex(path, x => x == position) + 1;
    }

    //other

    public bool isPieceOfPlayer(Position location, PlayerColor color) {
        PlayerColor piece = this.get(location);
        if (piece == color) {
            return true;
        }else {
            return false;
        }
    }

    public bool isOponentPiece(Position location, PlayerColor color) {
        PlayerColor piece = this.get(location);
        if (piece == Board.otherColor(color) ) {
            return true;
        } else {
            return false;
        }
    }

    public Position getLandingPositionFrom(Position start, int moves, PlayerColor color) {
        Position[] path = paths[(int)color];

        int index;
        if (start == null || start == new Position(2,-1) || start == new Position(2,3) ) {
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
        Position[] positions = getPositionsForPlayer(color);
        for (int i = 0; i < positions.Length; i++) {
            if (isValidMove(positions[i], moves, color)) {
                return false;
            }
        }
        //check if moving from starting pool is possible
        if (isValidMove(null, moves, color)) {
            return false;
        }
        return true;
    }

    public Position[] getValidMovesForPlayer(PlayerColor color, int moves){
        Position[] currentPositions = getPositionsForPlayer(color);
        List<Position> validMoves = new List<Position>();
        for(var i = 0; i < currentPositions.Length; i++){
            Position start = currentPositions[i];
            if(isValidMove(start, moves, color)){
                validMoves.Add(start);
            }
        }
        if (isValidMove(null, moves, color) && this.startingPoolCount[(int)color] != 0) {
            if (color == PlayerColor.Black) {
                validMoves.Add(new Position(2, -1));//clicking on the pool
            } else {
                validMoves.Add(new Position(2, 3));
            }
        }
        return validMoves.ToArray();
    }

    public Position[] getPositionsForPlayer(PlayerColor color) {
        List<Position> positions = new List<Position>();
        for (int x = 0; x < pieces.GetLength(0); x++) {
            for (int y = 0; y < pieces.GetLength(1); y++) {
                if (pieces[x, y] == color) {
                    positions.Add(new Position(x, y));
                }
            }
        }
        return positions.ToArray();
    }

    public bool isValidMove(Position start, int moves, PlayerColor color) {
        //null piece location means picking from bag
        Position end = this.getLandingPositionFrom(start, moves, color);
        if(end != null) {
            if (this.get(end) == PlayerColor.Free) {
                return true; // landing on free space
            } else if (this.get(end) == color) {
                return false; //landing on same color
            } else if (Board.isRossete(end) && this.get(end) != PlayerColor.Free) {
                return false; //landing on opponent (or self) on rosset
            }else {
                return true; //capturing piece
            }
        }else {
            return false; //trying to go off the board
        }
    }

    //piece movement

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
