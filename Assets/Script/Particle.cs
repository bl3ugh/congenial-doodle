using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 position;
    [SerializeField] private float radius;

    public void Initialize(Vector2 speed, Vector2 position, float radius)
    {
        this.speed = speed;
        this.position = position;
        this.radius = radius;
        UpdateScale();
    }
    
    public void reInitialize(float radius)
    {
        this.radius = radius;
        UpdateScale();
    }

    public Vector2 GetSpeed()
    {
        return speed;
    }
    
    public void SetSpeed(Vector2 speed)
    {
        this.speed = speed;
    }

    public void AddSpeed(Vector2 speed)
    {
        this.speed += speed;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public void AddPosition(Vector2 position)
    {
        this.position += position;
    }

    public float GetRadius()
    {
        return radius;
    }

    public void UpdateDeets()
    {
        
        transform.position = position + speed * Time.deltaTime;
        position = transform.position;
    }

    private void UpdateScale()
    {
        transform.localScale = Vector3.one * (2 * radius);
    }
}
