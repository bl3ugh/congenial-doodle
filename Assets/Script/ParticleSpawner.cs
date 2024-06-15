using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab;
    private List<Particle> particles = new List<Particle>();

    //boundings
    private float width;
    private float height;
    public List<Particle> GetParticles()
    {
        return particles;
    }

    public void Initialize(int amount, float width, float height)
    {
        this.width = width;
        this.height = height;
        SpawnParticles(amount);
    }

    private void SpawnParticles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            spawnRandom(i);
        }
    }
    IEnumerator SpawnParticlesWithLoopDelay(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            spawnRandom(count);

            // Wait for the specified amount of time before continuing the loop
            yield return new WaitForSeconds(delay);
        }
    }   

    public void spawnRandom(int count){
        // Generate a random spawn position
        Vector2 spawnPosition = new Vector2(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
        //random speed x
        float speedX = Random.Range(-10.0f, 10.0f);
        //random speed y
        float speedY = Random.Range(-10.0f, 10.0f);



        spawnParticle(spawnPosition, new Vector2(speedX, speedY),count);
    }

    private void spawnParticle(Vector2 spawnPosition, Vector2 speed, int particleNumber)
    {
        // Instantiate the particle at the spawn position
        GameObject particleObject = Instantiate(particlePrefab, spawnPosition, Quaternion.identity);

        // Set the name of the particle to its number
        particleObject.name = "Particle " + particleNumber;

        // Get the Particle component and add it to the list
        Particle particleComponent = particleObject.GetComponent<Particle>();

        particleComponent.SetSpeed(speed);
        particleComponent.SetPosition(spawnPosition);
        if (particleComponent != null)
        {
            particles.Add(particleComponent);
        }
    }
}
