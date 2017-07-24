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

        for (int p = 0; p < 2; p++) {
            //for which player
            PlayerColor color;
            if (p == 0) {
                color = player;
            } else {
                color = Board.otherColor(player);
            }
            Position[] posiions = board.getPositionsForPlayer(color);

            for (int i = 0; i < posiions.Length; i++) {
                Position currentPosition = posiions[i];
                //add 10 for being on rossete
                if (Board.isRossete(currentPosition)) {
                    score += 25;
                }

                //add 10 for being in safe row, 3 for contested row
                
                if (currentPosition.y != 1) {
                    score += 5;
                } else {
                    score += 3;
                }
            }

            //add 100 for all the pieces in the ending pool
            score += 100 * board.endingPoolCount[(int)color];
        }
        Debug.Log(player);
        Debug.Log(score);
        return score;
    }

    public static Position getBestClick(BoardManager boardManager, PlayerColor aiColor) {
        Board board = boardManager.board;
        int roll = boardManager.rollValue;
        int maxDepth = 2;
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
        if (aiTurn) {
            curDepth++;
        }

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
