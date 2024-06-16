using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ParticleSpawner spawner;
    public BoundingBox boundingBox;
    public ParticleManager particleManager;
    public GridManager gridManager;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private Vector2 speed = new Vector2(0, 0);
    [SerializeField] private Vector2 position = new Vector2(0, 0);
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float diameter = 1f;
    [SerializeField] private float cellSizeToParticle = 1f;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int amount = 1;
    private float tempDiameter = 1f;
    [SerializeField] private float height = 10f;
    [SerializeField] private float width = 20f;
    [SerializeField] private float wallRestitution = 0.9f;
    [SerializeField] private float particleRestitution = 0.9f;
    [SerializeField] private float frictionCoefficient = 0.01f;
    [SerializeField] private int stepAmount = 1;
    private Particle particle;
    void Start()
    {
        if (cellSizeToParticle < 0.01f)
        {
            cellSizeToParticle = 0.01f;
        }
        if (diameter < 0.01f)
        {
            diameter = 0.01f;
        }
        if( frictionCoefficient < 0.01f){
            frictionCoefficient = 0.01f;
        }
        cellSize = diameter * cellSizeToParticle;
        particle = particlePrefab.GetComponent<Particle>();
        // Initialize the bounding box
        boundingBox.Initialize(width, height);

        // Initialize the particle manager
        particleManager.Initialize(spawner,boundingBox,gridManager,wallRestitution,amount,particleRestitution,frictionCoefficient,gravity,stepAmount);

        // Initialize the default particle prefab
        particle.Initialize(speed, position, radius);

        // Initialize the grid manager
        gridManager.Initialize(cellSize, width, height);

        // Initialize the particle spawner
        spawner.Initialize(amount, width, height);
    }

    void OnValidate()
    {
        if (frictionCoefficient < 0.01f)
        {
            frictionCoefficient = 0.01f;
        }
        if (cellSizeToParticle < 0.01f)
        {
            cellSizeToParticle = 0.01f;
        }
        updateDiamRad();
        updateBoundingBox(width, height);

        updateParticleManager(wallRestitution, boundingBox, particleRestitution, frictionCoefficient);

        updateParticle(radius, gravity);
        
        cellSize = diameter * cellSizeToParticle;
        

        



        updateGridManager(cellSize, width, height);
    }


    public void updateDiamRad(){
        if (diameter != tempDiameter){
            radius = diameter/2;
        }
        else{
            diameter = 2*radius;
        }
        tempDiameter = diameter;
    }

    public void updateParticleManager(float newDamping, BoundingBox newBoundingBox, float particleRestitution, float frictionCoefficient){
        particleManager.reInitialize(newDamping, newBoundingBox,gridManager, particleRestitution, frictionCoefficient,gravity,stepAmount);
    }

    public void updateBoundingBox(float newWidth, float newHeight){
        boundingBox.Initialize(newWidth, newHeight);
    }

    public void updateGridManager(float newDiameter, float newWidth, float newHeight ){
        gridManager.Initialize(newDiameter, newWidth, newHeight);
    }

    public void updateParticle(float newRadius, float gravity){
        particle = particlePrefab.GetComponent<Particle>();
        UpdateParticleRadius(newRadius);
        radius = newRadius;
        this.gravity = gravity;
        particle.reInitialize(radius);
    }

    public void UpdateParticleRadius(float newRadius)
    {
        radius = newRadius;
        List<Particle> particles = spawner.GetParticles();
        foreach (Particle particle in particles)
        {
            particle.reInitialize( radius);
        }
    }
}

