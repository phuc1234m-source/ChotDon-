using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject roadStraightPrefab;
    public GameObject intersectionPrefab;

    [Range(0f, 1f)]
    public float intersectionChance = 0.08f;

    public float scrollSpeed = 5f;
    public int chunksOnScreen = 6;

    private List<GameObject> activeChunks = new List<GameObject>();

    private bool lastWasIntersection = false;

    float bottomOfScreen = -7.5f;

    SideEnvironmentSpawner sideSpawner;

    void Start()
    {
        sideSpawner = FindFirstObjectByType<SideEnvironmentSpawner>();

        float spawnY = bottomOfScreen;

        for (int i = 0; i < chunksOnScreen; i++)
        {
            GameObject chunk = SpawnChunk(spawnY);

            RoadTile tile = chunk.GetComponent<RoadTile>();
            spawnY += tile.tileHeight;
        }
    }

    void Update()
    {
        MoveChunks();

        if (activeChunks.Count == 0)
            return;

        GameObject firstChunk = activeChunks[0];
        RoadTile firstTile = firstChunk.GetComponent<RoadTile>();

        if (firstChunk.transform.position.y <= bottomOfScreen - firstTile.tileHeight)
        {
            Destroy(firstChunk);
            activeChunks.RemoveAt(0);

            GameObject lastChunk = activeChunks[activeChunks.Count - 1];
            RoadTile lastTile = lastChunk.GetComponent<RoadTile>();

            float newY = lastChunk.transform.position.y + lastTile.tileHeight;

            SpawnChunk(newY);
        }
    }

    GameObject SpawnChunk(float yPosition)
    {
        GameObject prefab;
        bool isIntersection = false;

        if (!lastWasIntersection && Random.value < intersectionChance)
        {
            prefab = intersectionPrefab;
            lastWasIntersection = true;
            isIntersection = true;
        }
        else
        {
            prefab = roadStraightPrefab;
            lastWasIntersection = false;
        }

        GameObject chunk = Instantiate(prefab, new Vector3(0, yPosition, 0), Quaternion.identity);

        chunk.layer = LayerMask.NameToLayer("Background");

        activeChunks.Add(chunk);

        // báo cho SideEnvironmentSpawner
        if (sideSpawner != null)
        {
            sideSpawner.NotifyIntersectionSpawn(isIntersection);
        }

        return chunk;
    }

    void MoveChunks()
    {
        foreach (GameObject chunk in activeChunks)
        {
            chunk.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }
    }
}