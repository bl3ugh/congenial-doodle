using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ParticleSpawner spawner;
    public BoundingBox boundingBox;
    public ParticleManager particleManager;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Vector2 speed = new Vector2(0, 0);
    [SerializeField] private Vector2 position = new Vector2(0, 0);
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float height = 10f;
    [SerializeField] private float width = 20f;
    [SerializeField] private float damping = 0.9f;
    private Particle particle;
    void Start()
    {
        particle = particlePrefab.GetComponent<Particle>();
        // Initialize the bounding box
        boundingBox.Initialize(width, height);

        // Initialize the particle manager
        particleManager.Initialize(spawner,boundingBox,damping);

        // Initialize the default particle prefab
        particle.Initialize(speed, position, gravity, radius);
    }

    void OnValidate()
    {   
        boundingBox.Initialize(width, height);

        // Update the particle radius
        UpdateParticleRadius(radius);

        particleManager.reInitialize(damping, boundingBox);

        particle = particlePrefab.GetComponent<Particle>();
        particle.reInitialize(gravity, radius);
    }

    public void UpdateParticleRadius(float newRadius)
    {
        radius = newRadius;
        List<Particle> particles = spawner.GetParticles();
        foreach (Particle particle in particles)
        {
            particle.reInitialize(gravity, radius);
        }
    }
}

