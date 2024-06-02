using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private float spawnRate = 1.0f;
	[SerializeField] private GameObject[] enemyPrefabs;
	[SerializeField] private bool canSpawn = true;
	[SerializeField] private Transform dragon;
	[SerializeField] private float spawnRadius = 10.0f; // The radius around the dragon to spawn enemies

	private void Start()
	{
		StartCoroutine(enemySpawner());
	}

	private IEnumerator enemySpawner()
	{
		WaitForSeconds wait = new WaitForSeconds(spawnRate);

		while (canSpawn)
		{
			yield return wait;

			int rand = Random.Range(0, enemyPrefabs.Length);
			GameObject enemyToSpawn = enemyPrefabs[rand];

			Vector3 spawnPosition = GetRandomPositionNearDragon();
			Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
		}
	}

	private Vector3 GetRandomPositionNearDragon()
	{
		Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction
		float randomDistance = Random.Range(5, spawnRadius); // Random distance within the radius
		Vector3 spawnPosition = dragon.position + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;
		return spawnPosition;
	}
}
