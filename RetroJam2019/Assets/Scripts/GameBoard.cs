using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameBoard
{
    private readonly int boardXSize = 20;
    private readonly int boardYSize = 11;

    private Cell[,] board;

    public GameBoard()
    {
        board = new Cell[boardXSize, boardYSize];

        for (int x = 0; x < boardXSize; x++)
        {
            for (int y = 0; y < boardYSize; y++)
            {
                board[x, y] = new Cell(x, y);
            }
        }
    }

    public Cell[,] GetBoard()
    {
        return board;
    }

	public Cell GetCell(int x, int y)
    {
        return board[x, y];
    }

    public BoardItemBehavior[] GetAllItems()
    {
        var allItems = new List<BoardItemBehavior>();

        foreach (var cell in board)
        {
            allItems = allItems.Union(cell.BoardItems).ToList();
        }

        return allItems.ToArray();
    }

    public int GetWidth ()
    {
        return boardXSize;
    }

    public int GetHeight ()
    {
        return boardYSize;
    }

    public bool AddItem(BoardItemBehavior item, int x, int y)
    {
        if (x >= boardXSize || y >= boardYSize || x < 0 || y < 0)
        {
            return false;
        }

        if (board[x, y].BoardItems.Contains(item))
        {
            return false;
        }

        board[x, y].BoardItems.Add(item);
        //items.Add(item);

        return true;
    }

    public bool HasItem(BoardItemBehavior item)
    {
        foreach (var cell in board)
        {
            if (cell.BoardItems.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveItem(BoardItemBehavior item)
    {
        foreach(var cell in board)
        {
            cell.BoardItems.Remove(item);
        }

        //items.Remove(item);
    }

    public void MoveItem(BoardItemBehavior item, Vector2 direction)
    {
        int xDirection, yDirection;

        if (direction.x < 0)
        {
            xDirection = -1;
        }
        else
        {
            xDirection = 1;
        }

        if (direction.x < 0)
        {
            yDirection = -1;
        }
        else
        {
            yDirection = 1;
        }

        int startX = 0;
        int startY = 0;

        if (yDirection > 0) startY = boardYSize - 1;
        if (xDirection > 0) startX = boardXSize - 1;

        //Run through th board starting in the direction of the motion (for objects that occupy multiple tiles)
        for (var x = startX; x < boardXSize && x >= 0; x += xDirection)
        {
            for (var y = startY; y < boardYSize && y >= 0; y += yDirection)
            {
                Cell c = board[x, y];

                if (!c.BoardItems.Contains(item))
                {
                    continue;
                }

                c.BoardItems.Remove(item);
                AddItem(item, x + xDirection, y + yDirection);
            }
        }
    }
}

public class Cell
{
    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }

    public List<BoardItemBehavior> BoardItems = new List<BoardItemBehavior>();
    public int X;
    public int Y;
}

