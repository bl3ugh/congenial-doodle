using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize { get; set; }
    //bounding area
    public float width { get; set; }
    public float height { get; set; }

    private int rowSize;

    private Dictionary<String, int> directionValues = new Dictionary<String, int>(
        new KeyValuePair<String, int>[]{
            new KeyValuePair<String, int>("UP", 0),
            new KeyValuePair<String, int>("DOWN", 1),
            new KeyValuePair<String, int>("LEFT", -1),
            new KeyValuePair<String, int>("RIGHT", 1)
        }
    );
    public void Initialize(float cellSize, float width, float height)
    {
        //add min size to grid size and width and height of 5 for dimensions and 0.01 for grid size
        if (cellSize < 0.01f)
        {
            cellSize = 0.01f;
        }
        if (width < 5)
        {
            width = 5;
        }
        if (height < 5)
        {
            height = 5;
        }
        this.cellSize = cellSize;
        this.width = width + 2 * cellSize;
        this.height = height + 2 * cellSize;
        rowSize = (int)Math.Ceiling(this.width / this.cellSize);
        directionValues["UP"] = rowSize;
        directionValues["DOWN"] = -rowSize;
    }


    public int GetRowSize()
    {
        return rowSize;
    }
    public int GetNumberOfCells()
    {
        return (int)Math.Ceiling(width / cellSize) * (int)Math.Ceiling(height / cellSize);
    }
    //can probably be made faster with like some has whatever
    public int GetGridId(Vector2 position)
    {
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;
        int rowSize = (int)Math.Ceiling(width / cellSize);

        int x = (int)((position.x + halfWidth) / cellSize);
        int y = (int)((position.y + halfHeight) / cellSize);

        return x + y * rowSize;

    }

    public Vector2 GetGridPosition(int gridId)
    {
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;
        int rowSize = (int)Math.Ceiling(width / cellSize);
        int x = gridId % rowSize;
        int y = gridId / rowSize;

        return new Vector2(x * cellSize - halfWidth, y * cellSize - halfHeight);
    }




    public List<int> GetPossibleAdjacentCells(int cellNumber)
    {
        int numberOfCells = GetNumberOfCells();
        //check for cells that exist around using the direction values
        List<int> surroundingCells = new List<int>
            {
                cellNumber
            };
        if (cellNumber > rowSize && cellNumber < numberOfCells - rowSize && cellNumber % rowSize != 0 && cellNumber % rowSize != rowSize - 1)
        {
            //middle cells
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["RIGHT"]);
        }
        else if (cellNumber == 0)
        {//bottom left
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["RIGHT"]);
        }
        else if (cellNumber == rowSize - 1)
        {//bottom right
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["LEFT"]);
        }
        else if (cellNumber == numberOfCells - rowSize)
        {//top left
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["RIGHT"]);
        }
        else if (cellNumber == numberOfCells - 1)
        {//top right
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["LEFT"]);
        }
        else if (cellNumber % rowSize == 0)
        {//left wall
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["RIGHT"]);
        }
        else if (cellNumber % rowSize == rowSize - 1)
        {//right wall
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["LEFT"]);
        }
        else if (cellNumber < rowSize)
        {//bottom wall
            surroundingCells.Add(cellNumber + directionValues["UP"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["UP"] + directionValues["RIGHT"]);
        }
        else if (cellNumber > numberOfCells - rowSize)
        {//top wall
            surroundingCells.Add(cellNumber + directionValues["DOWN"]);
            surroundingCells.Add(cellNumber + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["RIGHT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["LEFT"]);
            surroundingCells.Add(cellNumber + directionValues["DOWN"] + directionValues["RIGHT"]);
        }

        return surroundingCells;
    }

    private void drawGrid(){
  //ensure no division by zero and no negatives to be drawn 

        if (cellSize < 0.01f)
        {
            cellSize = 0.01f;
        }
        if (width < 5)
        {
            width = 5;
        }
        if (height < 5)
        {
            height = 5;
        }


        // Draw the grid centered at (0, 0)
        Gizmos.color = Color.green;

        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        for (float x = -halfWidth; x < halfWidth; x += cellSize)
        {
            for (float y = -halfHeight; y < halfHeight; y += cellSize)
            {
                // Correct the position by using x for horizontal and y for vertical coordinates
                Gizmos.DrawWireCube(new Vector3(x, y, 0) + new Vector3(cellSize / 2, cellSize / 2, 0), new Vector3(cellSize, cellSize, 0));
            }
        }
    }
    void OnDrawGizmos()
    {
      
    }
}
