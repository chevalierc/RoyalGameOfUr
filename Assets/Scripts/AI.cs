using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {

    public static Position getBestClick(BoardManager boardManager, PlayerColor myColor) {
        Board board = boardManager.board;
        int roll = boardManager.rollValue;
        Position bestClick = null;

        Position[] positions = board.getPositionsForPlayer(myColor);

        //if all pieces are in pool
        if(positions.Length == 0) {
            return new Position(2, -1);
        }

        for(int i = 0; i < positions.Length; i++) {
            if(!board.isValidMove(positions[i], roll, myColor)) {
                continue;
            }

            Position end = board.getLandingPositionFrom(positions[i], roll, myColor);

            if (bestClick == null) {
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

    private class Node {
        public Move move;
        public int value;

        public Node(Move move, int value) {
            this.move = move;
            this.value = value;
        }
    }

    public static Move getBestMove(Board currentBoard) {
        int maxDepth = 2;
        int alpha = int.MaxValue;
        int beta = int.MinValue;

        Board board = new Board(currentBoard);

        Node bestNode = value(board, true, alpha, beta, 0, maxDepth);

        return bestNode.move;
    }

    private static Node value(Board currentBoard, bool aiTurn, int alpha, int beta, int curDepth, int maxDepth) {
        if (aiTurn) {
            curDepth++;
        }

        if (curDepth == maxDepth) {
            return new Node(null, currentBoard.getBoardScore());
        }

        return minMax(currentBoard, aiTurn, alpha, beta, curDepth, maxDepth);
    }

    private static Node minMax(Board currentBoard, bool isAiTurn, int alpha, int beta, int curDepth, int maxDepth) {
        Node minOrMaxNode = new Node(null, 0);

        Move[] possibleMoves = currentBoard.getPossibleMovesFor(isAiTurn);
        for (var i = 0; i < possibleMoves.Length; i++) {
            Move move = possibleMoves[i];
            Board newBoard = currentBoard.getBoardAfterMove(move);

            int boardValue = value(newBoard, !isAiTurn, alpha, beta, curDepth, maxDepth).value;

            //set min max node
            if (minOrMaxNode.move == null) {
                minOrMaxNode = new Node(move, boardValue);
            } else {
                //player turn. Maximize score
                if (!isAiTurn) {
                    if (boardValue > minOrMaxNode.value) {
                        minOrMaxNode = new Node(move, boardValue);
                    }
                    //ai turn. Minimize score
                } else {
                    if (boardValue < minOrMaxNode.value) {
                        minOrMaxNode = new Node(move, boardValue);
                    }
                }
            }

            //set alpha/beta

            /*
            if(isAiTurn) {
                if(boardValue > beta) {
                    return new Node(move, boardValue);
                }
                if(alpha < boardValue) {
                    alpha = boardValue;
                }
            }else {
                if (boardValue < alpha) {
                    return new Node(move, boardValue);
                }
                if (beta > boardValue) {
                    beta = boardValue;
                }
            }
            */


        }

        return minOrMaxNode;
    }

}
