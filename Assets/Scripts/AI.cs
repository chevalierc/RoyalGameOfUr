using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {

    public static Position getBestClick(BoardManager boardManager, PlayerColor aiColor) {
        Board board = boardManager.board;
        int roll = boardManager.rollValue;

        Position bestClick = null;
        int bestMoveScore = int.MinValue;

        Position[] positions = board.getPositionsForPlayer(aiColor);

        //check if you can move piece from pool
        if (board.isValidMove(null, roll, aiColor) && boardManager.startingPools[(int)aiColor].count != 0) {
            Position end = board.getLandingPositionFrom(null, roll, aiColor);
            Board newBoard = new Board(board);
            newBoard.set(end, aiColor);
            int boardValue = evalutateBoard(newBoard, aiColor);
            bestClick = new Position(2, -1);
            bestMoveScore = boardValue;
        }

        for (int i = 0; i < positions.Length; i++) {
            Position start = positions[i];
            if (!board.isValidMove(start, roll, aiColor)) {
                continue;
            }

            Position end = board.getLandingPositionFrom(positions[i], roll, aiColor);
            Board newBoard = new Board(board);
            newBoard.move(start, end);
            int boardValue = evalutateBoard(newBoard, aiColor);
            if(boardValue > bestMoveScore || bestClick == null) {
                bestClick = start;
                bestMoveScore = boardValue;
            }
        }

        return bestClick;
    }

    public static int evalutateBoard(Board board, PlayerColor color) {
        int score = 0;
        PlayerColor opponentColor = Board.otherColor(color);
        Position[] myPositions = board.getPositionsForPlayer(color);
        Position[] opponentPositions = board.getPositionsForPlayer(opponentColor);

        for(int i = 0; i < myPositions.Length; i++) {
            Position currentPosition = myPositions[i];
            //if goal add 100
            if (Board.isGoal(currentPosition)) {
                score += 100;
            }
            //add 10 for being on rossete
            if (Board.isRossete(currentPosition)) {
                score += 25;
            }

            //add 10 for being in safe row, 3 for contested row
            if (currentPosition.y != 1) {
                score += 5;
            }else {
                score += 3;
            }
        }

        //add 100 for all the pieces in the ending pool
        score += 100 * board.endingPoolCount[(int)color];

        for (int i = 0; i < opponentPositions.Length; i++) {
            Position currentPosition = opponentPositions[i];
            score -= 20;
        }

        Debug.Log("Score" + score);
        return score;
    }

    /*

    private class Node {
        public Position move;
        public int value;
        public Board board;

        public Node(Position move, int value) {
            this.move = move;
            this.value = value;
        }
    }

    public static Position expectedminimax(Node node, int depth) {
        if(node.board.endingPoolCount[]
    }

    */


    /*
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
            return new Node(null, currentBoard.getBoardScore() );
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


        }

        return minOrMaxNode;
    }

    */

}
