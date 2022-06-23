using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStars : MonoBehaviour
{
    private Utils utils;

    public GameObject Star;

    private GameObject photon;
    private Photon photonClass;
    public GameObject Asteroid;

    private Vector2 currentChunk, lastChunk; 

    [Range(0,1)]
    public float starDensity=0.1f, redShift, blueShift;

    public float distRange;
    public float distOffset=0f;
    private int chunkSize;
    public float updateDelay = 0.1f;
    [Range(0,1)]
    public float asteroidDensity;
    public float minAsteroidSize, maxAsteroidSize;
    private List<Vector2> renderedChunks;


    void Start()
    {        
        // Get player GameObject and Class
        photon = GameObject.FindGameObjectWithTag("Player");
        photonClass = photon.GetComponent<Photon>();
        currentChunk = photonClass.currentChunk;
        renderedChunks = new List<Vector2>();
        chunkSize = photonClass.chunkSize;

        utils = new Utils();

        // Initialise map in current pos of player
        lastChunk = currentChunk;
        GenerateMap(currentChunk);
    }

    void Update()
    {
        photon = GameObject.FindGameObjectWithTag("Player");
        photonClass = photon.GetComponent<Photon>();
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
        GameObject chunkObj = new GameObject(string.Format("Chunk_{0}_{1}", chunk.x, chunk.y));

        // Total num of stars to create in chunk
        float numStars = chunkSize * chunkSize * starDensity/100;

        for (int i=0; i<numStars; i++)
        {
            GameObject star = createStar(chunk);
            star.transform.SetParent(chunkObj.transform);
        }
        // Create asteroids
        
        float numAsteroids = chunkSize * chunkSize * asteroidDensity/100;
        for (int i=0; i<numAsteroids; i++)
        {
            // Random (x,y) pos within chunk
            float xpos = Random.Range((chunk.x) * chunkSize, (chunk.x + 1) * chunkSize);
            float ypos = Random.Range((chunk.y) * chunkSize, (chunk.y + 1)* chunkSize);

            // Set asteroid into foreground or background
            // int coinflip = (int) Mathf.Round(Random.Range(0,1));
            float zpos = 0;
            // if (coinflip == 1)
            // {
            //     zpos = 1;
            // }

            float size = Random.Range(minAsteroidSize, maxAsteroidSize);

            // Create new asteroid with a random size
            GameObject newAsteroid = Instantiate(Asteroid, new Vector3(xpos, ypos, zpos), new Quaternion(0,0,Random.Range(0,360),1));
            newAsteroid.GetComponent<WanderingBody>().Init(size, Vector3.zero, true, false);
            newAsteroid.transform.SetParent(chunkObj.transform);
        }

        // Group all chunks
        chunkObj.transform.SetParent(transform);
    }

    GameObject createStar(Vector2 chunk)
    {
        // Random (x,y) pos within chunk
        float xpos = Random.Range((chunk.x) * chunkSize, (chunk.x + 1) * chunkSize);
        float ypos = Random.Range((chunk.y) * chunkSize, (chunk.y + 1)* chunkSize);

        // Random z (distance) of star
        float z_min = distOffset - distRange/2; 
        float z_max = distOffset + distRange/2; 
        float zpos = utils.getNormalDistVal(z_min, z_max);

        // Apply red shift for far away stars
        float R, G, B;
        if (zpos > distOffset)
        {
            R = 1.0f;
            G = utils.Remap(zpos, z_min, z_max, 1.0f, 1 - redShift);
            B = utils.Remap(zpos, z_min, z_max, 1.0f, 1 - redShift);
        }
        // Apply blue shift for close stars
        else
        {
            R = utils.Remap(zpos, z_min, z_max, 1 - blueShift, 1.0f);
            G = utils.Remap(zpos, z_min, z_max, 1 - blueShift, 1.0f);
            B = 1.0f;
        }

        // Create star and apply color
        GameObject newStar = Instantiate(Star, new Vector3(xpos, ypos, zpos), Quaternion.identity);
        newStar.GetComponent<Renderer>().material.SetColor("_Color", new Color(R, G, B));

        return newStar;
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
