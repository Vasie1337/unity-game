using UnityEngine;

public class DeathOnTouch : MonoBehaviour
{
    public Spawner spawner;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawner.SpawnPlayer();
        }
    }

}
