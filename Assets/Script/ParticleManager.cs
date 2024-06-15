using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class ParticleManager : MonoBehaviour
{


    private ParticleSpawner particleSpawner;
    private BoundingBox boundingBox;

    private Vector2 halfBoundings;

    private List<Particle> particles;

    private int numberOfParticles;
    //particleCellNumber sotres the particle arr number and the cell number 
    //always length of particles
    private (int, int)[] particleCellNumber;

    //stores where the first occurence of ecah cell appears in particleCellNumber
    //always length of the number of cells
    private int[] cellFirstOccurence;
    private float wallRestitution;

    private GridManager gridManager;

    private float particleRestitution;
    private float frictionCoefficient;
    private float gravity;
    private float fixedDeltaTime;
    public void Initialize(ParticleSpawner spawner, BoundingBox boundingBox, GridManager gridManager,
     float wallRestitution, int amount, float particleRestitution,float frictionCoefficient,float gravity,float fixedDeltaTime)
    {
        this.wallRestitution = wallRestitution;
        this.boundingBox = boundingBox;
        this.gridManager = gridManager;
        this.numberOfParticles = amount;
        this.particleRestitution = particleRestitution;
        this.frictionCoefficient = frictionCoefficient;
        this.gravity = gravity;
        this.fixedDeltaTime = fixedDeltaTime;

        particleSpawner = spawner;
        halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
        particles = particleSpawner.GetParticles();

        particleCellNumber = new (int, int)[numberOfParticles];
        cellFirstOccurence = new int[gridManager.GetNumberOfCells()];
        emptyParticleCellNumber();
        emptyCellFirstOccurence();
    }


    public void reInitialize(float wallRestitution, BoundingBox boundingBox, GridManager gridManager,
     float particleRestitution,float frictionCoefficient,float gravity,float fixedDeltaTime)
    {
        this.wallRestitution = wallRestitution;
        this.boundingBox = boundingBox;
        this.particleRestitution = particleRestitution;
        this.halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
        this.frictionCoefficient = frictionCoefficient;
        this.gravity = gravity;
        this.fixedDeltaTime = fixedDeltaTime;

        particleCellNumber = new (int, int)[numberOfParticles];
        cellFirstOccurence = new int[gridManager.GetNumberOfCells()];
    }
    public void emptyParticleCellNumber()
    {
        for (int i = 0; i < particleCellNumber.Length; i++)
        {
            particleCellNumber[i] = (int.MaxValue, int.MaxValue);
        }
    }
    public void emptyCellFirstOccurence()
    {
        for (int i = 0; i < cellFirstOccurence.Length; i++)
        {
            cellFirstOccurence[i] = -1;
        }
    }
    
    private float accumulator = 0.0f;

    void Update()
    {

            emptyParticleCellNumber();
            emptyCellFirstOccurence();

            ApplyGravity(); 
            UpdateGridPosition();
            ResolveCollisions();

            foreach (Particle particle in particles)
            {
                // Update particle positions and ensure they are within boundaries
                particle.UpdateDeets();
            }

        
    }

    // Apply gravity to all particles
    private void ApplyGravity()
    {
        foreach (Particle particle in particles)
        {
            Vector2 newSpeed = particle.GetSpeed() + new Vector2(0,gravity * Time.deltaTime);
            particle.SetSpeed(newSpeed);
        }
    }

    public void ResolveCollisions()
    {
        if (particleSpawner != null)
        {
            List<Particle> particles = particleSpawner.GetParticles();

            ResolveWallCollisions();
            ResolveParticleCollisionsSolid();
        }
    }

    public void UpdateGridPosition()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            Particle particle = particles[i];
            if (particle != null)
            {
                int cellNumber = gridManager.GetGridId(particle.GetPosition());
                particleCellNumber[i] = (i, cellNumber);
            }
        }
        Array.Sort(particleCellNumber, (x, y) => x.Item2.CompareTo(y.Item2));

        for (int i = 0; i < particleCellNumber.Length; i++)
        {
            (int, int) entry = particleCellNumber[i];
            int cellNumber = entry.Item2;

            if (cellNumber < gridManager.GetNumberOfCells() && cellFirstOccurence[cellNumber] == -1)
            {
                cellFirstOccurence[cellNumber] = i;
            }
        }
    }

    private void ResolveWallCollisions()
    {
        foreach (Particle particle in particles)
        {
            float radius = particle.GetRadius();
            Vector2 position = particle.GetPosition();
            Vector2 speed = particle.GetSpeed();

            if (Mathf.Abs(particle.GetPosition().x) + radius > halfBoundings.x)
            {
                speed = speed * new Vector2(-1 * wallRestitution, 1);
                position = new Vector2((halfBoundings.x - radius) * Mathf.Sign(position.x), position.y);
                particle.SetSpeed(speed);
                particle.SetPosition(position);
            }

            if (Mathf.Abs(particle.GetPosition().y) + radius > halfBoundings.y)
            {
                speed = speed * new Vector2(1, -1 * wallRestitution);
                position = new Vector2(position.x, (halfBoundings.y - radius) * Mathf.Sign(position.y));
                particle.SetSpeed(speed);
                particle.SetPosition(position);
            }
        }
    }

    private void ResolveParticleCollisionsSolid()
    {
        float maxSpeed = 60.0f; 

        foreach ((int, int) entry in particleCellNumber)
        {
            int particleNumber = entry.Item1;
            int cellNumber = entry.Item2;
            Particle particleMain = particles[particleNumber];
            List<int> cellsToCheck = gridManager.GetPossibleAdjacentCells(cellNumber);
            Vector2 averageDirection = new Vector2(0, 0);
            foreach (int cell in cellsToCheck)
            {
                if (cellFirstOccurence[cell] != -1)
                {
                    int firstParticleInCell = cellFirstOccurence[cell];
                    int increment = 0;
                    
                    while (true)
                    {
                        if (firstParticleInCell + increment < particleCellNumber.Length)
                        {
                            int particleNumber2 = particleCellNumber[firstParticleInCell + increment].Item1;
                            int cellNumber2 = particleCellNumber[firstParticleInCell + increment].Item2;
                            
                            if (cellNumber2 == cell)
                            {
                                if (particleNumber != particleNumber2)
                                {
                                    Particle particleBitch = particles[particleNumber2];
                                    
                                    //averageDirection += particleBitch.GetPosition() - particleMain.GetPosition();
                                    if (CheckCollision(particleMain, particleBitch))
                                    {
                                        float distance = Vector2.Distance(particleMain.GetPosition(), particleBitch.GetPosition());
                                        float minDistance = particleMain.GetRadius() + particleBitch.GetRadius();

                                        if (distance < minDistance) // too close
                                        {
                                            // Calculate the overlap
                                            float overlap = minDistance - distance;

                                            // Calculate collision normal and correction
                                            Vector2 normal = (particleBitch.GetPosition() - particleMain.GetPosition()).normalized;
                                            Vector2 correction = normal * (overlap / 2); // Correct each particle by half the overlap

                                            // Apply positional correction to prevent overlap
                                            particleMain.SetPosition(particleMain.GetPosition() - correction);
                                            particleBitch.SetPosition(particleBitch.GetPosition() + correction);
                                              // Check if the particles are within the boundaries after the correction
                                            particleMain.SetPosition(CheckBoundary(particleMain.GetPosition(), particleMain.GetRadius()));
                                            particleBitch.SetPosition(CheckBoundary(particleBitch.GetPosition(), particleBitch.GetRadius()));
                                            // Calculate relative velocity in the direction of the normal
                                            Vector2 relativeVelocity = particleBitch.GetSpeed() - particleMain.GetSpeed();
                                            float relativeNormalVelocity = Vector2.Dot(relativeVelocity, normal);

                                            // Compute impulse scalar with restitution
                                            float impulseScalar = -(1 + particleRestitution) * relativeNormalVelocity / 2;
                                            Vector2 impulse = impulseScalar * normal;

                                            // Apply the impulse to adjust velocities
                                            particleMain.SetSpeed(CapVelocity(particleMain.GetSpeed() - impulse, maxSpeed));
                                            particleBitch.SetSpeed(CapVelocity(particleBitch.GetSpeed() + impulse, maxSpeed));

                                            // Apply friction proportional to the speed and a friction coefficient
                                            Vector2 frictionForceMain = frictionCoefficient * particleMain.GetSpeed() * particleMain.GetRadius();
                                            Vector2 frictionForceBitch = frictionCoefficient * particleBitch.GetSpeed() * particleBitch.GetRadius();

                                            particleMain.SetSpeed(CapVelocity(particleMain.GetSpeed() - frictionForceMain, maxSpeed));
                                            particleBitch.SetSpeed(CapVelocity(particleBitch.GetSpeed() - frictionForceBitch, maxSpeed));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        increment++;
                    }
                    //add some push force away from the average direction
                    particleMain.AddSpeed(pushForce(averageDirection));
                }
            }
        }
    }

    private void ResolveParticleCollisionsFluid(){
        //using push force
    }
    //complete
    private Vector2 pushForce(Vector2 averageDirection)
    {
       return averageDirection.normalized * 0.1f; 
    }
    private Vector2 CheckBoundary(Vector2 position, float radius)
    {
        if (Mathf.Abs(position.x) + radius > halfBoundings.x)
        {
            position = new Vector2((halfBoundings.x - radius) * Mathf.Sign(position.x), position.y);
        }

        if (Mathf.Abs(position.y) + radius > halfBoundings.y)
        {
            position = new Vector2(position.x, (halfBoundings.y - radius) * Mathf.Sign(position.y));
        }

        return position;
    }

    private Vector2 CapVelocity(Vector2 speed, float maxSpeed)
    {
        if (speed.magnitude > maxSpeed)
        {
            return speed.normalized * maxSpeed;
        }
        return speed;
    }

    private bool CheckCollision(Particle particle1, Particle particle2)
    {
        float distance = Vector2.Distance(particle1.GetPosition(), particle2.GetPosition());
        float combinedRadii = particle1.GetRadius() + particle2.GetRadius();
        return distance < combinedRadii;
    }



    //loop through the  particles and draw the cell that they are in
    private void drawActiveCells() {
        if (Application.isPlaying)
        {
            float cellSize = gridManager.cellSize;
            //as a test draw whatever cellnumber is so i can change cellnumber and see what is higlighted
            foreach (int entry in particleCellNumber.Select(x => x.Item2))
            {

                if (entry != -1)
                {
                    //draw the grid cell that each sphere is in
                    Vector2 gridPosition = gridManager.GetGridPosition(entry);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(new Vector3(gridPosition.x, gridPosition.y, 0) + new Vector3(cellSize / 2, cellSize / 2, 0), cellSize / 2);
                }
            }
        }
    }


    //gets the first particle and checks what cells it is looking at and what partciels are in those cells
    //draws a circle at the cells being checked and a line to the particles in those cells
    private void colourSearchedCells()
    {
        if (Application.isPlaying)
        {
            Particle particle = particles[0];
            (int, int) thatParticleDeets = (0, 0);
            foreach ((int, int) deets in particleCellNumber)
            {
                if (deets.Item1 == 0)
                {
                    thatParticleDeets = deets;
                    break;
                }
            }
            List<int> cellsToCheck = gridManager.GetPossibleAdjacentCells(thatParticleDeets.Item2);
            foreach (int cell in cellsToCheck)
            {
                Vector2 cellPosition = gridManager.GetGridPosition(cell);
                float cellSize = gridManager.cellSize;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(cellPosition.x, cellPosition.y, 0) + new Vector3(cellSize / 2, cellSize / 2, 0), cellSize / 2);
            }


            int particleNumber = thatParticleDeets.Item1;
            int cellNumber = thatParticleDeets.Item2;
            Particle particleMain = particles[particleNumber];
            foreach (int cell in cellsToCheck)
            {
                if (cellFirstOccurence[cell] != -1)
                {
                    int firstParticleInCell = cellFirstOccurence[cell];
                    int incremement = 0;
                    while (true)
                    {
                        if (firstParticleInCell + incremement < particleCellNumber.Count())
                        {
                            int particleNumber2 = particleCellNumber[firstParticleInCell + incremement].Item1;
                            int cellNumber2 = particleCellNumber[firstParticleInCell + incremement].Item2;
                            if (cellNumber2 == cell)
                            {
                                if (particleNumber != particleNumber2)
                                {
                                    Gizmos.color = Color.green;
                                    Gizmos.DrawLine(particleMain.GetPosition(), particles[particleNumber2].GetPosition());

                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        incremement++;
                    }
                }
            }
        }

    }

    void OnDrawGizmos()
    {
        colourSearchedCells();
    }
}