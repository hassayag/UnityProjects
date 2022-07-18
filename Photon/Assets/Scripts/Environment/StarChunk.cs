using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarChunk : MonoBehaviour
{
    public Vector2 chunk;

    private ParticleSystem.Particle[] particles;
    private ParticleSystem chunkParticleSystem;
    private Utils utils;
    private int numStars;
    private float chunkSize, starDensity, distOffset, distRange, redShift, blueShift;

    public void BeginParticles(Vector2 inputChunk)
    {        
        utils = new Utils();

        // Get starmap params
        BuildStars starMap = transform.parent.GetComponent<BuildStars>();
        starDensity = starMap.starDensity;
        distOffset = starMap.distOffset;
        distRange = starMap.distRange;
        redShift = starMap.redShift;
        blueShift = starMap.blueShift;

        chunkSize = GameObject.FindGameObjectWithTag("Player").GetComponent<Doggy>().chunkSize;

        chunk = inputChunk;
        // ParticleSystem.MainModule particleSystem = GetComponent<ParticleSystem>().main;

        // Total num of stars to create in chunk
        numStars = (int) Mathf.Round(chunkSize * chunkSize * starDensity/100);

        initChunk();
    }

    void initChunk()
    {        
        chunkParticleSystem = GetComponent<ParticleSystem>();

        ParticleSystem.MainModule m = chunkParticleSystem.main;
        m.maxParticles = numStars;
            
        particles = new ParticleSystem.Particle[numStars];
        chunkParticleSystem.Emit(numStars);

        int numOfParticles = chunkParticleSystem.GetParticles(particles);

        for (int i=0; i<numOfParticles; i++)
        {
            particles[i] = createStar(particles[i]);
        }

        chunkParticleSystem.SetParticles(particles, numOfParticles);
    }

    ParticleSystem.Particle createStar(ParticleSystem.Particle newStar)
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

        newStar.position = new Vector3(xpos, ypos, zpos);
        newStar.startColor = new Color(R, G, B);
        newStar.remainingLifetime = Mathf.Infinity;
        newStar.startSize = 5.0f;

        return newStar;
    }
}
