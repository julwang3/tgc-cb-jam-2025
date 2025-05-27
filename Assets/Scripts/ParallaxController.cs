using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float length;
    private Vector3 startPos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distX = (cam.transform.position.x * parallaxEffect);
        float distY = (cam.transform.position.y * parallaxEffect);

        transform.position = startPos + new Vector3(distX, distY, 0.0f);
    }
}