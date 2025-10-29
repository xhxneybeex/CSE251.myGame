using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab; // what to spawn
    [SerializeField] private float spawnXMin = -6f; // left side
    [SerializeField] private float spawnXMax = 6f;  // right side
    [SerializeField] private float spawnY = 7f;     // height to spawn at
    [SerializeField] private Vector2 spawnDelayRange = new Vector2(1f, 3f); // time between spawns

    void Start()
    {
        // start the spawn loop
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // keep spawning forever CAN change if i wanna make it so coins only fall when player "beats" lvl
        while (true)
        {
            // pick random X position
            float x = Random.Range(spawnXMin, spawnXMax);

            // spawn the coin
            Instantiate(coinPrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);

            // wait a random amount of time before next spawn
            yield return new WaitForSeconds(Random.Range(spawnDelayRange.x, spawnDelayRange.y));
        }
    }
}
