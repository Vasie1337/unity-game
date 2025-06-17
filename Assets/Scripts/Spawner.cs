using UnityEngine;

// Spawns the player at the start of the game
// Function to spawn the player
public class Spawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }

}
