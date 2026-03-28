using UnityEngine;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;

    public float spawnInterval = 2f;

    // 3 lane positions (X values)
    public float[] lanes = new float[3];

    // Track which lanes are currently occupied
    private bool[] laneOccupied = new bool[3];
    private PlayerController player;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        InvokeRepeating(nameof(SpawnCar), 1f, spawnInterval);
        player = FindFirstObjectByType<PlayerController>();
    }

    void SpawnCar()
    {
        float camHeight = cam.orthographicSize;
        float camY = cam.transform.position.y;

        bool spawnFromTop = Random.value > 0.5f;

        List<int> availableLanes = new List<int>();

        for (int i = 0; i < lanes.Length; i++)
        {
            if (!laneOccupied[i])
            {
                // Block middle lane during intro
                if (!player.CanControl && i == 1)
                    continue;

                // NEW RULE:
                // If spawning from TOP (moving Down),
                // block middle lane permanently
                if (spawnFromTop && i == 1)
                    continue;

                availableLanes.Add(i);
            }
        }

        // Prevent filling all lanes
        if (availableLanes.Count <= 1)
            return;

        int randomLaneIndex = availableLanes[Random.Range(0, availableLanes.Count)];

        float xPos = lanes[randomLaneIndex];
        float yPos;

        GameObject car = Instantiate(carPrefab);
        CarObstacle carScript = car.GetComponent<CarObstacle>();

        if (spawnFromTop)
        {
            yPos = camY + camHeight + 1f;
            carScript.moveDirection = CarObstacle.Direction.Down;
        }
        else
        {
            yPos = camY - camHeight - 1f;
            carScript.moveDirection = CarObstacle.Direction.Up;
        }

        car.transform.position = new Vector3(xPos, yPos, 0f);

        laneOccupied[randomLaneIndex] = true;

        StartCoroutine(FreeLaneWhenDestroyed(car, randomLaneIndex));
    }

    System.Collections.IEnumerator FreeLaneWhenDestroyed(GameObject car, int laneIndex)
    {
        yield return new WaitUntil(() => car == null);
        laneOccupied[laneIndex] = false;
    }
}