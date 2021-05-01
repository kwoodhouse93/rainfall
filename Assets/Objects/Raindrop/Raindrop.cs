using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Raindrop : MonoBehaviour
{
    // Object to spawn when collision is processed - perhaps a particle system
    // for a shatter or splash effect?
    [SerializeField] private GameObject spawnOnDestroy;
    // Allows raindrops to survive for a random amount of time between 0 and
    // maxCollisionLife after hitting a trigger - cheaply simulates a more
    // 3d body of water - longer living raindrops appear to be closer to the
    // camera.
    [SerializeField] private float maxCollisionLife;
    // How likely collision is to despawn the object (0 to 1).
    [SerializeField] private float destructionChance;

    private Rainfall rainfall;
    private TrailRenderer trailRenderer;

    void Start()
    {
        rainfall = transform.parent.GetComponent<Rainfall>();
        trailRenderer = transform.GetChild(0).GetComponent<TrailRenderer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        StartCoroutine(HandleCollision());
    }

    IEnumerator HandleCollision()
    {
        yield return new WaitForSeconds(Random.Range(0, maxCollisionLife));

        Object.Instantiate(spawnOnDestroy, transform.position, transform.rotation);

        if (Random.Range(0f, 1f) < destructionChance)
        {
            Debug.Log("Destroying");
            Object.Destroy(gameObject);
        }

        transform.position = rainfall.NewSpawnPosition();
        trailRenderer.Clear();

        AudioManager.Instance.PlayRainSound();
    }
}
