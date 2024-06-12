using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{


    // Center point of the window in world space
    private Vector3 center;
    private float width;
    private float height;

    public float getWidth()
    {
        return width;
    }
    public float getHeight()
    {
        return height;
    }
    void Start()
    {
        center = transform.position;
    }

    public void Initialize(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    // Check if a point is inside the window bounds
    public bool IsInside(Vector2 point)
    {
        // Define the min and max bounds in 2D space assuming the window is centered at 'center'
        float minX = center.x - width / 2f;
        float maxX = center.x + width / 2f;
        float minY = center.y - height / 2f;
        float maxY = center.y + height / 2f;

        return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
    }

    // Optionally visualize the window in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(0,0), new Vector3(width, height, 0));
    }

}
