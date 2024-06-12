using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize{ get; set; }
    //bounding area
    public float width { get; set; }
    public float height { get; set;}
    
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
        this.width = width;
        this.height = height;

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


    void OnDrawGizmos()
    {
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
                Gizmos.DrawWireCube(new Vector3(x, y, 0) + new Vector3(cellSize/2,cellSize/2,0), new Vector3(cellSize, cellSize, 0));
            }
        }
    }


}
