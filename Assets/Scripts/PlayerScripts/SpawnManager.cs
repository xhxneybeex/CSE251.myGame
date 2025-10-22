using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnXMin = -6f;
    [SerializeField] private float spawnXMax = 6f;
    [SerializeField] private float spawnY = 7f;
    [SerializeField] private Vector2 spawnDelayRange = new Vector2(1f, 3f);

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float x = Random.Range(spawnXMin, spawnXMax);
            Instantiate(coinPrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(spawnDelayRange.x, spawnDelayRange.y));
        }
    }
}
