using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] Spikes spikes;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spikes.lastRespawnPoint = gameObject;
        }
    }
}
