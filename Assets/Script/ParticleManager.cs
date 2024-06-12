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

    private Dictionary<int, List<int>> gridPositions = new Dictionary<int, List<int>>();
    private float damping;

    private GridManager gridManager;

    [SerializeField] private int CellNumber = 0;
    public void Initialize(ParticleSpawner spawner, BoundingBox boundingBox, float damping, GridManager gridManager)
    {
        this.damping = damping;
        this.boundingBox = boundingBox;
        this.gridManager = gridManager;
        particleSpawner = spawner;
        halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
        particles = particleSpawner.GetParticles();
    }
    

    public void reInitialize(float damping, BoundingBox boundingBox)
    {
        this.damping = damping;
        this.halfBoundings = new Vector2(boundingBox.getWidth() / 2, boundingBox.getHeight() / 2);
    }
    void Update()
    {
        gridPositions.Clear();
        if (particleSpawner != null)
        {
            List<Particle> particles = particleSpawner.GetParticles();
            for(int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];
                particle.Update();
                if (particle != null)
                {
                    UpdateGridPosition(particle, i);
                    ResolveCollisions(particle);
                }
            }
        }
    }
    public void UpdateGridPosition(Particle particle,int i){

        //find grid pos for particle
        //particle index , grid position
        int gridId = gridManager.GetGridId(particle.GetPosition());
        if (!gridPositions.ContainsKey(gridId))
        {
            gridPositions.Add(gridId, new List<int>());
        }
        gridPositions[gridId].Add(i);
    
    }
    private void ResolveCollisions(Particle particle)
    {
        float radius = particle.GetRadius();
        Vector2 position = particle.GetPosition();
        Vector2 speed = particle.GetSpeed();
        if(Mathf.Abs(particle.GetPosition().x) + radius > halfBoundings.x)
        {
            speed = speed * new Vector2(-1 * damping,1);
            position = new Vector2 ((halfBoundings.x - radius) * Mathf.Sign(position.x), position.y);
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


    //loop through the  particles and draw the cell that they are in

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            float cellSize = gridManager.cellSize;
            //as a test draw whatever cellnumber is so i can change cellnumber and see what is higlighted
            foreach(KeyValuePair<int, List<int>> entry in gridPositions)
            {
                //draw the grid cell that each sphere is in
                Vector2 gridPosition = gridManager.GetGridPosition(entry.Key);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(gridPosition.x, gridPosition.y, 0) + new Vector3(cellSize/2,cellSize/2,0), cellSize/2);
            }
        }
    }
}