using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStars : MonoBehaviour
{
    private Utils utils;

    public GameObject newParticleSystem;

    private GameObject photon;
    private Doggy photonClass;
    public GameObject Asteroid;

    private Vector2 currentChunk, lastChunk; 
    private GameObject chunkObj;

    [Range(0,1)]
    public float starDensity=0.1f, redShift, blueShift;

    public float distRange;
    public float distOffset=0f;
    private int chunkSize, numStars;
    public float updateDelay = 0.1f;
    [Range(0,1)]
    public float asteroidDensity;
    public float minAsteroidSize, maxAsteroidSize;
    private List<Vector2> renderedChunks;


    void Start()
    {        
        // Get player GameObject and Class
        photon = GameObject.FindGameObjectWithTag("Player");
        photonClass = photon.GetComponent<Doggy>();
        currentChunk = photonClass.currentChunk;
        renderedChunks = new List<Vector2>();
        chunkSize = photonClass.chunkSize;

        // Total num of stars to create in chunk
        numStars = (int) Mathf.Round(chunkSize * chunkSize * starDensity/100);

        utils = new Utils();

        // Initialise map in current pos of player
        lastChunk = currentChunk;
        GenerateMap(currentChunk);
        
    }

    void Update()
    {
        photon = GameObject.FindGameObjectWithTag("Player");
        photonClass = photon.GetComponent<Doggy>();
        currentChunk = photonClass.currentChunk;
        GenerateMap(currentChunk);
    }

    public void GenerateMap(Vector2 chunk)
    {
        // Render 9 chunks in and around player
        for (int i=-1; i<2; i++){
            for (int j=-1; j<2; j++)
            {
                Vector2 chunk_ij = new Vector2(chunk.x+i, chunk.y+j);
                DrawChunkBoundaries(chunk_ij);

                if (!renderedChunks.Contains(chunk_ij))
                {
                    renderChunk(chunk_ij);
                    renderedChunks.Add(chunk_ij);
                }
            }
        }
    }

    void renderChunk(Vector2 chunk)
    {
        // Create a StarChunk 
        chunkObj = Instantiate(newParticleSystem, new Vector3 (chunk.x*chunkSize, chunk.y*chunkSize, 0), Quaternion.identity);
        chunkObj.name = string.Format("Chunk_{0}_{1}", chunk.x, chunk.y);
        chunkObj.transform.SetParent(transform);
        chunkObj.GetComponent<StarChunk>().BeginParticles(chunk);

        // Create asteroids
        float numAsteroids = chunkSize * chunkSize * asteroidDensity/100;
        for (int i=0; i<numAsteroids; i++)
        {
            // Random (x,y) pos within chunk
            float xpos = Random.Range((chunk.x) * chunkSize, (chunk.x + 1) * chunkSize);
            float ypos = Random.Range((chunk.y) * chunkSize, (chunk.y + 1)* chunkSize);

            float zpos = 5f;

            float size = Random.Range(minAsteroidSize, maxAsteroidSize);

            // Create new asteroid with a random size
            GameObject newAsteroid = Instantiate(Asteroid, new Vector3(xpos, ypos, zpos), new Quaternion(0,0,Random.Range(0,360),1));
            newAsteroid.GetComponent<WanderingBody>().Init(size, Vector3.zero, true, false);
            newAsteroid.transform.SetParent(chunkObj.transform);
        }
    }


    void DrawChunkBoundaries(Vector2 chunk)
    {
        // Draw horizontals
        Debug.DrawLine(new Vector3 (chunk.x * chunkSize, chunk.y * chunkSize, 0), new Vector3 ((chunk.x + 1) * chunkSize, chunk.y * chunkSize, 0), Color.red);
        Debug.DrawLine(new Vector3 (chunk.x * chunkSize, (chunk.y + 1) * chunkSize, 0), new Vector3 ((chunk.x + 1) * chunkSize, (chunk.y + 1) * chunkSize, 0), Color.red);

        // Draw verticals
        Debug.DrawLine(new Vector3 (chunk.x * chunkSize, chunk.y * chunkSize, 0), new Vector3 (chunk.x * chunkSize, (chunk.y + 1) * chunkSize, 0), Color.red);
        Debug.DrawLine(new Vector3 ((chunk.x + 1) * chunkSize, chunk.y * chunkSize, 0), new Vector3 ((chunk.x + 1) * chunkSize, (chunk.y + 1) * chunkSize, 0), Color.red);
    }
}
