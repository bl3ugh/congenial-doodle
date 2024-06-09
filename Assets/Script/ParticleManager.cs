using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private ParticleSpawner particleSpawner;
    private BoundingBox boundingBox;
    
    private Vector2 halfBoundings;

    private float damping;
    public void Initialize(ParticleSpawner spawner, BoundingBox boundingBox, float damping)
    {
        this.damping = damping;
        this.particleSpawner = spawner;
        this.boundingBox = boundingBox;
        this.halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
    }
    
    public void reInitialize(float damping, BoundingBox boundingBox)
    {
        this.damping = damping;
        this.halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
    }
    void Update()
    {
        if (particleSpawner != null)
        {
            List<Particle> particles = particleSpawner.GetParticles();
            foreach (Particle particle in particles)
            {
                ResolveCollisions(particle);
            }
        }
    }

    private void ResolveCollisions(Particle particle)
    {
        float radius = particle.GetRadius();
        Vector2 position = particle.GetPosition();
        Vector2 speed = particle.GetSpeed();
        if(Mathf.Abs(particle.GetPosition().x) + radius > halfBoundings.x)
        {
            speed = (speed * new Vector2(-1 * damping,1));
            position = (new Vector2 ((halfBoundings.x - radius) * Mathf.Sign(position.x), position.y));
            particle.SetSpeed(speed);
            particle.SetPosition(position);
        }
        if(Mathf.Abs(particle.GetPosition().y) + radius > halfBoundings.y)
        {
            speed = speed * new Vector2(1,-1 * damping);
            position = new Vector2 (position.x, (halfBoundings.y - radius) * Mathf.Sign(position.y));
            particle.SetSpeed(speed);
            particle.SetPosition(position);
        }
    }


}