using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainfall : MonoBehaviour
{
    [SerializeField] private float spawnHeight;
    [SerializeField] private float spawnCount;
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnRange;
    [SerializeField] private Raindrop raindrop;
    [SerializeField] private float wobbleFreq;
    [SerializeField] private float wobbleAmount;
    [SerializeField] private float wobbleRandomness;

    private float nextSpawn;

    void Start()
    {
        StartCoroutine(SpawnRaindrops());
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * wobbleFreq + Random.Range(-wobbleRandomness, wobbleRandomness)) * wobbleAmount;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private IEnumerator SpawnRaindrops()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Object.Instantiate(raindrop, NewSpawnPosition(), Quaternion.identity, transform);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public Vector3 NewSpawnPosition()
    {
        float x = Random.Range(-spawnRange, spawnRange);
        return new Vector3(x, spawnHeight, 0);
    }
}
