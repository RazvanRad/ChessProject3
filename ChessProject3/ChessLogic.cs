﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ChessProject3
{
    class ChessLogic
    {
        Board board;
        bool running = true;
        public int round = 0;
        bool whiteRound() => round % 2 == 0;
        public bool isRunning() => running;

        public ChessLogic(Board b)
        {
            board = b;
            initPiecesDefault();
        }
        bool isTileEmpty(int x, int y)
        {
            return board.getTile(x, y) == null;
        }
        bool isWhiteOnTile(int x, int y) => !isTileEmpty(x, y) && ((int)board.ePiecegetTile(x, y) < 7);
        bool isBlackOnTile(int x, int y) => !isTileEmpty(x, y) && ((int)board.ePiecegetTile(x, y) > 6);
        public bool squareSelectionSuccess(int x, int y)
        {
            if (whiteRound())
            {
                if (isWhiteOnTile(x, y))
                {
                    return true;
                }
            }
            else
            {
                if (isBlackOnTile(x, y))
                {
                    return true;
                }

            }
            return false;
        }
        public void nextRound()
        {
            if (round % 2 == 0)
            {

            }
            else
            {

            }
            round++;

        }
        void initPiecesDefault()
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(i);
                board.setTile(i, 1, ePiece.pawnB);
                board.setTile(i, 6, ePiece.pawnW);
            }

            board.setTile(0, 0, ePiece.rookB);
            board.setTile(7, 0, ePiece.rookB);

            board.setTile(0, 7, ePiece.rookW);
            board.setTile(7, 7, ePiece.rookW);

            board.setTile(1, 0, ePiece.horseB);
            board.setTile(6, 0, ePiece.horseB);

            board.setTile(1, 7, ePiece.horseW);
            board.setTile(6, 7, ePiece.horseW);

            board.setTile(2, 0, ePiece.bishopB);
            board.setTile(5, 0, ePiece.bishopB);

            board.setTile(2, 7, ePiece.bishopW);
            board.setTile(5, 7, ePiece.bishopW);

            board.setTile(3, 0, ePiece.kingB);
            board.setTile(4, 0, ePiece.queenB);

            board.setTile(3, 7, ePiece.kingW);
            board.setTile(4, 3, ePiece.queenW);
        }
        public List<Tuple<int, int>> getAllSpecialMoves(int x, int y)
        {
            var specialMoveset = board.getTile(x, y).getSpecialMoveList(x, y);
            return filterSpecial(x, y, specialMoveset);
        }
        public List<Tuple<int, int>> getAllMoves(int x, int y) //EXCEPT SPECIAL
        {
            //3
            var current = board.getTile(x, y);

            var moveset = getAllNormalMoves(x, y);
            var specialMoveset = getAllSpecialMoves(x, y);

            if (specialMoveset != null)
            {
                moveset.AddRange(specialMoveset);
            }
            return moveset;
        }
        public bool movePiece(int x, int y, int newX, int newY)
        {
            bool ret = false;
            List<Tuple<int, int>> special = getAllNormalMoves(x,y);
            List<Tuple<int, int>> normal = getAllSpecialMoves(x,y);
            var target = Tuple.Create(newX, newY);
            
            if (normal.Contains(target))
            {
                board.moveTile(x, y, newX, newY);
                ret = true;
            }
            else if (special.Contains(target))
            {
                board.getTile(x, y).setSpecialMove(false);
                board.moveTile(x, y, newX, newY);
                ret = true;
            }
            return ret;
        }
        bool anythingInTheWayCustom(int x, int y, int newX, int newY)
        {
            iPiece current = board.getTile(x, y);
            iPiece target = board.getTile(newX, newY);
            if (isWhiteOnTile(x, y))
            {
                if (newY == y - 1)
                {
                    if (target != null)
                    {
                        return true;
                    }
                }

            }
            else if (isBlackOnTile(x, y))
            {
                if (newY == y + 1)
                {
                    return true;
                }
            }
            else
            { while (true) { } }

            return false;
        }
        bool anythingInTheWayBishop(int oldX, int oldY, int newX, int newY)
        {
            int iterX = (oldX < newX) ? oldX : newX;
            int finalX = (oldX > newX) ? oldX : newX;
            int iterY = (oldY < newY) ? oldY : newY;
            int finalY = (oldY > newY) ? oldY : newY;
            for (int i = iterX + 1, j = iterY + 1; i < finalX && j < finalY; i++, j++)
            {
                if (board.getTile(i, j) != null)
                {
                    return true;
                }
            }
            return false;
        }
        bool anythingInTheWayRook(int oldX, int oldY, int newX, int newY)
        {
            if (oldX == newX)
            {
                int iterStart = oldY;
                int iterEnd = newY;
                if (newY < oldY)
                {
                    iterStart = newY;
                    iterEnd = oldY;
                }

                for (int i = iterStart + 1; i < iterEnd; i++)
                {
                    if (board.getTile(oldX, i) != null)
                    {
                        return true;
                    }
                }
            }
            else if (oldY == newY)
            {
                int iterStart = oldX;
                int iterEnd = newX;
                if (newX < oldX)
                {
                    iterStart = newX;
                    iterEnd = oldX;
                }
                for (int i = iterStart + 1; i < iterEnd; i++)
                {
                    if (board.getTile(i, oldY) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        List<Tuple<int, int>> filterAnythingInTheWay(int x, int y, List<Tuple<int, int>> list)
        {
            List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();
            iPiece current = board.getTile(x, y);
            foreach (Tuple<int, int> element in list)
            {
                int newX = element.Item1;
                int newY = element.Item2;
                if (current.isSameAs(ePiece.rookW) &&  anythingInTheWayRook(x, y, newX, newY))
                {
                    toRemove.Add(element);
                }

                else if (current.isSameAs(ePiece.bishopW) && anythingInTheWayBishop(x, y, newX, newY))
                {
                    toRemove.Add(element);
                }

                else if (current.isSameAs(ePiece.queenW) &&
                    anythingInTheWayRook(x, y, newX, newY) ||
                    anythingInTheWayBishop(x, y, newX, newY))
                {
                    toRemove.Add(element);
                }

                else if (current.isSameAs(ePiece.pawnW) )
                {
                    if (anythingInTheWayRook(x, y, newX, newY))
                    {
                        toRemove.Add(element);
                    }
                }
            }

            return list.Except(toRemove).ToList();
        }
        List<Tuple<int, int>> filterSpecial(int x, int y, List<Tuple<int, int>> list)
        {
            List<Tuple<int, int>> moveset = new List<Tuple<int, int>>();
            if (list != null)
            {
                var current = board.getTile(x, y);
                //COPY PASTED, CARE
                List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();
                if (current.isSameAs(ePiece.pawnW))
                {
                    foreach (Tuple<int, int> piece in list)
                    {
                        iPiece target = board.getTile(piece.Item1, piece.Item2);

                        //2 move
                        if (target == null)
                        {
                        }
                        else
                        {
                            toRemove.Add(piece);
                        }
                    }
                }
                moveset = list.Except(toRemove).ToList();
                moveset = filterPieceTakeAllyPiece(x, y, moveset);
                moveset = filterAnythingInTheWay(x, y, moveset);
            }

            return moveset;

        }
        List<Tuple<int, int>> filterDynamic(iPiece current, List<Tuple<int, int>> list)
        {
            List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();
            if (current.isSameAs(ePiece.pawnW))
            {
                foreach (Tuple<int, int> piece in list)
                {
                    iPiece target = board.getTile(piece.Item1, piece.Item2);

                    if (target == null || pieceTakeAllyPiece(current, target))
                    {
                        toRemove.Add(piece);
                    }
                }
            }
            return list.Except(toRemove).ToList();
        }

        public List<Tuple<int, int>> getAllNormalMoves(int x, int y)
        {
            List<Tuple<int, int>> moveset = new List<Tuple<int, int>>();
            List<Tuple<int, int>> movesetDynamic;
            List<Tuple<int, int>> movesetStatic;

            iPiece current = board.getTile(x, y);

            // filter static
            movesetStatic = current.getUnfilteredStatic(x, y);
            if (movesetStatic != null)
            {
                moveset.AddRange(movesetStatic);
            }

            // filter dynamic
            movesetDynamic = current.getUnfilteredDynamic(x, y);
            if (movesetDynamic != null)
            {
                movesetDynamic = filterDynamic(current, movesetDynamic);
                if (movesetDynamic != null)
                {
                    moveset.AddRange(movesetDynamic);
                }
            }
            moveset = filterPieceTakeAllyPiece(x, y, moveset);
            moveset = filterAnythingInTheWay(x, y, moveset);

            //// after move, is there check?
            //if (isCheck()) ret = false;
            return moveset;
        }
        bool isSquareReachable(int oldX, int oldY, int newX, int newY, List<Tuple<int, int>> moveset)
        {
            foreach (Tuple<int, int> element in moveset)
            {
                if (element.Item1 == newX && element.Item2 == newY) return true;
            }
            return false;
        }
        bool isCheck()
        {
            return false;
        }
        public static List<Tuple<int, int>> getDynamicBishopMoves(int pieceX, int pieceY)
        {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>() { };
            // Top Left to bottom right diagonal
            for (int i = pieceX, j = pieceY; i < 8 && j < 8; i++, j++)
            {
                list.Add(Tuple.Create(i, j));
            }
            for (int i = pieceX, j = pieceY; i >= 0 && j >= 0; i--, j--)
            {
                list.Add(Tuple.Create(i, j));
            }
            // Bottom left to top right diagonal
            for (int i = pieceX, j = pieceY; i >= 0 && j < 8; i--, j++)
            {
                list.Add(Tuple.Create(i, j));
            }
            for (int i = pieceX, j = pieceY; i < 8 && j >= 0; i++, j--)
            {
                list.Add(Tuple.Create(i, j));
            }
            return list;
        }
        public static List<Tuple<int, int>> getDynamicRookMoves(int pieceX, int pieceY)
        {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>() { };
            //rook left to right
            for (int i = pieceX; i < 8; i++)
            {
                list.Add(Tuple.Create(i, pieceY));
            }
            for (int i = pieceX; i >= 0; i--)
            {
                list.Add(Tuple.Create(i, pieceY));
            }
            //rook top to bottom
            for (int i = pieceY; i < 8; i++)
            {
                list.Add(Tuple.Create(pieceX, i));
            }
            for (int i = pieceY; i >= 0; i--)
            {
                list.Add(Tuple.Create(pieceX, i));
            }
            return list;
        }
        List<Tuple<int, int>> filterPieceTakeAllyPiece(int x, int y, List<Tuple<int, int>> list)
        {
            var current = board.getTile(x, y);
            List<Tuple<int, int>> toRemove = new List<Tuple<int, int>>();
            foreach (Tuple<int, int> element in list)
            {
                iPiece target = board.getTile(element.Item1, element.Item2);
                if (target != null)
                {
                    if (pieceTakeAllyPiece(current, target))
                    {
                        toRemove.Add(element);
                    }
                }
            }
            return list.Except(toRemove).ToList();
        }
        bool pieceTakeAllyPiece(iPiece current, iPiece target)
        {
            return !((int)current.getId() > 6 ^ (int)target.getId() > 6);
        }

    }
}
