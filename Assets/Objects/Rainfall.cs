using System.Collections;
using UnityEngine;

public class Rainfall : MonoBehaviour
{
    [SerializeField] private Raindrop raindrop;

    [Header("Initial spawn parameters")]
    [SerializeField] private float spawnHeight;
    [SerializeField] private float spawnCount;
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnRange;

    [Header("Subsequent spawn parameters")]
    [SerializeField] private float spawnChance;

    [Header("Wobble parameters")]
    [SerializeField] private float wobbleFreq;
    [SerializeField] private float wobbleAmount;
    [SerializeField] private float wobbleRandomness;

    private float nextSpawn;

    void Start()
    {
        StartCoroutine(SpawnInitialRaindrops());
    }

    void Update()
    {
        // Rotate the entire parent object to get a wavy motion. Just a touch
        // of randomness gives it a shaky effect.
        float angle = Mathf.Sin(Time.time * wobbleFreq + Random.Range(-wobbleRandomness, wobbleRandomness)) * wobbleAmount;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Random chance to spawn a new raindrop at each spawn interval after the initial spawns.
        if (Time.time > nextSpawn)
        {
            Debug.Log("Testing spawn");
            if (Random.Range(0f, 1f) < spawnChance)
            {
                Debug.Log("Spawning");
                Object.Instantiate(raindrop, NewSpawnPosition(), Quaternion.identity, transform);
            }

            nextSpawn = Time.time + spawnRate;
        }
    }

    private IEnumerator SpawnInitialRaindrops()
    {
        // Make sure we don't spawn multiple raindrops during the setup coroutine
        nextSpawn = Time.time + (spawnRate * (spawnCount + 1));
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
