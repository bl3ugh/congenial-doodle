using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{

    [SerializeField] private Vector2 speed;
    [SerializeField] private Vector2 position;
    [SerializeField] private float gravity;
    [SerializeField] private float radius;


    public void Initialize(Vector2 speed, Vector2 position, float gravity, float radius)
    {
        this.speed = speed;
        this.position = position;
        this.gravity = gravity;
        this.radius = radius;
        UpdateScale();
    }
    
    public void reInitialize(float gravity, float radius)
    {
        this.gravity = gravity;
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
    public Vector2 GetPosition()
    {
        return position;
    }
    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public float GetRadius()
    {
        return radius;
    }

    public void Update()
    {
        speed.y += gravity * Time.deltaTime;
        transform.position = position + speed * Time.deltaTime;
        position = transform.position;
    }
    private void UpdateScale()
    {
        transform.localScale = Vector3.one * (2 * radius);
    }

}
