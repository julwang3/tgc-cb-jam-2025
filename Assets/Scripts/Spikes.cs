using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] GameObject respawnPoint;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.SetPosition(respawnPoint.transform.position);
        }
    }
}
