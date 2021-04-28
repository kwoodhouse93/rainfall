using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raindrop : MonoBehaviour
{
    [SerializeField] private GameObject spawnOnDestroy;
    [SerializeField] private float maxCollisionLife;

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

        transform.position = rainfall.NewSpawnPosition();
        trailRenderer.Clear();

        AudioManager.Instance.PlayRainSound();
    }
}
