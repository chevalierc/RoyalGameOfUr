using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {

    public static Position getBestClick(BoardManager boardManager, PlayerColor myColor) {
        Board board = boardManager.board;
        int roll = boardManager.rollValue;
        Position bestClick = null;

        Debug.Log(myColor);
        Position[] positions = board.getPositionsForPlayer(myColor);
        Debug.Log(positions.Length);

        //if all pieces are in pool
        if(positions.Length == 0) {
            return new Position(2, -1);
        }

        for(int i = 0; i < positions.Length; i++) {
            if(!board.isValidMove(positions[i], roll, myColor)) {
                continue;
            }
            Position end = board.getLandingPositionFrom(positions[i], roll, myColor);

            if(bestClick == null) {
                bestClick = positions[i];
            }

            if (board.isRossete(end)) {
                bestClick = positions[i];
            }

            if (board.isOponentPiece(end, myColor)) {
                bestClick = positions[i];
            }
        }

        Debug.Log(bestClick);
        return bestClick;
    }
	
}
