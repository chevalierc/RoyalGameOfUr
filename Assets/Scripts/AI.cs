using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    private static float[] rollProbability = new float[]{
        0.0625f,
        0.25f,
        0.375f,
        0.25f,
        0.0625f
    };

    public static int evalutateBoard(Board board, PlayerColor player) {
        int score = 0;
        int multiplier = 1;

        for (int p = 0; p < 2; p++) {
            //for which player
            PlayerColor color;
            if (p == 0) {
                color = player;
                multiplier = 1;
            } else {
                color = Board.otherColor(player);
                multiplier = -1;
            }
            Position[] positons = board.getPositionsForPlayer(color);

            for (int i = 0; i < positons.Length; i++) {
                Position currentPosition = positons[i];
                
                if (currentPosition == new Position(3, 1)) {
                    //if location is contested rossette
                    score += 40 * multiplier;
                } else if (Board.isRossete(currentPosition)) {
                    score += 20*multiplier;
                }

                if (currentPosition.y != 1 && !(currentPosition.x == 7 || currentPosition.x == 6) ) {
                    //in first 4
                    score += 10 * multiplier;
                } else {
                    // if in contested row add distance from start to encourage moving pieces twords end of board
                    score += board.distanceFromStart(currentPosition, color) * multiplier;
                }
            }

            //add 100 for all the pieces in the ending pool
            score += 100 * board.endingPoolCount[(int)color] * multiplier;
        }
        return score;
    }

    public static Position getBestClick(BoardManager boardManager, PlayerColor aiColor) {
        Board board = boardManager.board;
        int roll = boardManager.rollValue;
        int maxDepth = 6;
        Board newBoard = new Board(board);
        Node bestNode = value(board, true, 0, maxDepth, aiColor, roll);
        Position bestClick = bestNode.move;

        Debug.Log(bestClick);

        return bestClick;
    }

    private class Node {
        public Position move;
        public float value;
        public Board board;

        public Node(Position move, float value) {
            this.move = move;

            this.value = value;
        }
    }

    private static Node value(Board currentBoard, bool aiTurn, int curDepth, int maxDepth, PlayerColor color, int existingRoll) {
        curDepth++;

        if (curDepth == maxDepth) {
            return new Node(null, evalutateBoard(currentBoard, color) );
        }

        if(existingRoll != -1) {
            return minMaxOfProbability(currentBoard, aiTurn, curDepth, maxDepth, color, existingRoll);
        }

        return minMax(currentBoard, aiTurn, curDepth, maxDepth, color);
    }

    private static Node minMax(Board currentBoard, bool isAiTurn, int curDepth, int maxDepth, PlayerColor color) {
        Node minOrMaxNode = new Node(null, 0);
        for (int roll = 1; roll < rollProbability.Length; roll++){
            Node currentBestNode = minMaxOfProbability(currentBoard, isAiTurn, curDepth, maxDepth, color, roll);

            if (minOrMaxNode.move == null) {
                minOrMaxNode = currentBestNode;
            } else {
                if (!isAiTurn) {
                    if (currentBestNode.value < minOrMaxNode.value) {
                        minOrMaxNode = currentBestNode;
                    }
                } else {
                    if (currentBestNode.value > minOrMaxNode.value) {
                        minOrMaxNode = currentBestNode;
                    }
                }
            }

        }

        return minOrMaxNode;
    }

    private static Node minMaxOfProbability(Board currentBoard, bool isAiTurn, int curDepth, int maxDepth, PlayerColor color, int roll) {
        Node minOrMaxNode = new Node(null, 0);
        float probability = rollProbability[roll];
        Position[] possiblePositionOfPiecesToMove = currentBoard.getValidMovesForPlayer(color, roll);
        for (var i = 0; i < possiblePositionOfPiecesToMove.Length; i++) {
            Position start = possiblePositionOfPiecesToMove[i];
            Position end = currentBoard.getLandingPositionFrom(start, roll, color);
            Board newBoard = new Board(currentBoard);
            newBoard.aiMove(start, end, color);

            float boardValue = value(newBoard, !isAiTurn, curDepth, maxDepth, Board.otherColor(color), -1).value * probability;

            //set min max node
            if (minOrMaxNode.move == null) {
                minOrMaxNode = new Node(start, boardValue);
            } else {
                if (!isAiTurn) {
                    if (boardValue < minOrMaxNode.value) {
                        minOrMaxNode = new Node(start, boardValue);
                    }
                }else {
                    if (boardValue > minOrMaxNode.value) {
                        minOrMaxNode = new Node(start, boardValue);
                    }
                }

            }
        }
        return minOrMaxNode;
    }

}
