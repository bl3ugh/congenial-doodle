using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab;
    private List<Particle> particles = new List<Particle>();

    public List<Particle> GetParticles()
    {
        return particles;
    }

    void Start()
    {
        // Start the spawning process
        for (int i = 0; i < 10000; i++)
        {
            spawnRandom();
        }
    }

    IEnumerator SpawnParticlesWithLoopDelay(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            spawnRandom();

            // Wait for the specified amount of time before continuing the loop
            yield return new WaitForSeconds(delay);
        }
    }   

    public void spawnRandom(){
        // Generate a random spawn position
        Vector2 spawnPosition = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        //random speed x
        float speedX = Random.Range(-10.0f, 10.0f);
        //random speed y
        float speedY = Random.Range(-10.0f, 10.0f);
        spawnParticle(spawnPosition, new Vector2(speedX, speedY));
    }

    private void spawnParticle(Vector2 spawnPosition)
    {
        spawnParticle(spawnPosition, new Vector2(0, 0));
    }

    private void spawnParticle(Vector2 spawnPosition, Vector2 speed)
    {
        // Instantiate the particle at the spawn position
        GameObject particleObject = Instantiate(particlePrefab, spawnPosition, Quaternion.identity);

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
