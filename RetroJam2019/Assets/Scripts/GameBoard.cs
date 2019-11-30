using UnityEngine;
using System;
public class GameBoard {

    private int boardXSize = 20;
    private int boardYSize = 11;

	public Cell[][] board;

	public Cell GetCell(int x, int y) {
        throw new NotImplementedException("The requested feature is not implemented.");
    }

    public GameObject[] GetAllItems() {
        throw new NotImplementedException("The requested feature is not implemented.");
    }

    public int GetWidth () {
        return boardXSize;
    }

    public int GetHeight () {
        return boardYSize;
    }

}

public struct Cell{
    public BoardItemBehavior[] boardItems;
}

