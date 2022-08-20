using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class S0_Manager : MonoBehaviour
{
    public int nbParticles;
    public ComputeShader computeShader;
    public Shader shader;
    public Material material;

    [Range(1.0f, 100.0f)] public float intencityR = 0.5f;
    [Range(1.0f, 100.0f)] public float intencityG = 0.5f;
    [Range(1.0f, 100.0f)] public float intencityB = 0.5f;

    [Range(0.0f, 1.0f)] public float particlesAlpha = 0.5f;
    public int seed;

    struct Particle
    {
        public Vector3 position;
        public Vector4 color;
        public float speed;
    }

    ComputeBuffer particles;

    private void Start()
    {
        material = new Material(shader);
        particles = new ComputeBuffer(nbParticles, Marshal.SizeOf<Particle>());
        var particleData = new Particle[nbParticles];

        for (int i = 0; i < nbParticles; i++)
        {
            particleData[i] = new Particle
            {
                position = Random.onUnitSphere * 100.0f,
                color = GetRandomColor(),
                //speed = Random.Range(0.05f, 0.07f),
                speed = Random.Range(1.0f, 3.0f),
            };
        }
        particles.SetData(particleData);
    }

    Vector4 GetRandomColor()
    {
        float r = Random.Range(0.0f, 1.0f * intencityR);
        float g = Random.Range(0.0f, 1.0f * intencityG);
        float b = Random.Range(0.0f, 1.0f * intencityB);

        return new Vector4(r, g, b);
    }

    private void Update()
    {
        int kernel = computeShader.FindKernel("UpdateParticles");
        computeShader.SetBuffer(kernel, "_particles", particles);
        computeShader.SetFloat("_time", Time.time);
        computeShader.SetInt("_seed", seed);

        var count = (nbParticles + 1024 - 1) / 1024;
        computeShader.Dispatch(kernel, count, 1, 1);

        //Dessiner les particules 
        material.SetBuffer("_particles", particles);
        material.SetFloat("_Alpha", particlesAlpha);
        Graphics.DrawProcedural(material, new Bounds(transform.position, Vector3.one * 100.0f),
                                MeshTopology.Points, nbParticles);
    }

    private void OnDestroy()
    {
        particles?.Release();
    }
}
