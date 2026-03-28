using UnityEngine;

public class SideEnvironmentSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SideLane
    {
        public string laneName;
        public GameObject[] prefabs;
        public float leftX;
        public float rightX;
    }

    [Header("Side Lanes")]
    public SideLane[] lanes;

    [Header("Spawn Settings")]
    public float spawnY = 8f;
    public float verticalSpacing = 3f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Density")]
    [Range(0f, 1f)]
    public float spawnChance = 0.3f;
    public float densityIncrease = 0.01f;
    public float maxSpawnChance = 0.85f;

    private float spawnTimer;
    private float spawnInterval;

    private int intersectionRowsToSkip = 0;

    void Start()
    {
        spawnInterval = verticalSpacing / moveSpeed;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnRow();
            spawnTimer = 0f;
        }
    }

    public void NotifyIntersectionSpawn(bool isIntersection)
    {
        if (isIntersection)
        {
            intersectionRowsToSkip = 4;
        }
    }

    void SpawnRow()
    {
        if (intersectionRowsToSkip > 0)
        {
            intersectionRowsToSkip--;
            return;
        }

        spawnChance = Mathf.Min(
            spawnChance + densityIncrease * Time.deltaTime,
            maxSpawnChance
        );

        foreach (SideLane lane in lanes)
        {
            if (Random.value < spawnChance)
                SpawnFromLane(lane, true);

            if (Random.value < spawnChance)
                SpawnFromLane(lane, false);
        }
    }

    void SpawnFromLane(SideLane lane, bool isLeft)
    {
        if (lane.prefabs.Length == 0) return;

        int randomIndex = Random.Range(0, lane.prefabs.Length);
        GameObject prefab = lane.prefabs[randomIndex];

        float xPos = isLeft ? lane.leftX : lane.rightX;

        GameObject obj = Instantiate(
            prefab,
            new Vector3(xPos, spawnY, 0f),
            Quaternion.identity
        );

        SideObjectMover mover = obj.GetComponent<SideObjectMover>();
        if (mover != null)
            mover.moveSpeed = moveSpeed;

        if (isLeft)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = true;
        }
    }
}