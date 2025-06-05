using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] bool hasCustomSpawnPosition = false;
    [SerializeField] Vector2 playerSpawnPosition;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isActivated = true;
            if (hasCustomSpawnPosition)
            {
                LevelManager.Instance.LoadLevel(sceneToLoad, playerSpawnPosition);
            }
            else
            {
                LevelManager.Instance.LoadLevel(sceneToLoad);
            }
        }
    }
}
