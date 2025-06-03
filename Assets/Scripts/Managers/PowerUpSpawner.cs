using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Tooltip("List of power-up prefabs to spawn randomly")]
    public GameObject[] powerUpPrefabs;

    [Tooltip("List of spawn areas (BoxColliders) where power-ups can spawn")]
    public BoxCollider[] spawnAreas;

    [Tooltip("Estimated width of each power-up in world units")]
    public float powerUpWidth = 1f;

    [Tooltip("Spacing between power-ups in world units")]
    public float spacing = 0.5f;

    private void Start()
    {
        if (spawnAreas == null || spawnAreas.Length == 0)
        {
            Debug.LogError("No spawn areas assigned.");
            return;
        }

        foreach (var area in spawnAreas)
        {
            if (area != null)
            {
                SpawnPowerUps(area);
            }
        }
    }

    void SpawnPowerUps(BoxCollider spawnArea)
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("No power-up prefabs assigned.");
            return;
        }

        Vector3 worldSize = Vector3.Scale(spawnArea.size, spawnArea.transform.lossyScale);
        Vector3 worldCenter = spawnArea.transform.TransformPoint(spawnArea.center);

        Vector3 right = spawnArea.transform.right;
        Vector3 forward = spawnArea.transform.forward;

        float usableWidth = Mathf.Max(worldSize.x, worldSize.z);
        Vector3 spawnDirection = worldSize.x > worldSize.z ? right : forward;

        float totalUnitWidth = powerUpWidth + spacing;
        int numToSpawn = Mathf.FloorToInt((usableWidth + spacing) / totalUnitWidth);

        if (numToSpawn <= 0)
        {
            Debug.LogWarning("Spawn area too small for power-ups.");
            return;
        }

        float actualTotalWidth = (numToSpawn - 1) * totalUnitWidth;
        Vector3 startOffset = -spawnDirection * (actualTotalWidth / 2f);

        Vector3 spawnBase = new Vector3(worldCenter.x, spawnArea.transform.position.y, worldCenter.z);

        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 offset = spawnDirection * (i * totalUnitWidth);
            Vector3 spawnPos = spawnBase + startOffset + offset;
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);

            Instantiate(powerUpPrefabs[randomIndex], spawnPos, Quaternion.identity, transform);
        }
    }
}
