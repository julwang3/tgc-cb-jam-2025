using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float length;
    private Vector3 startPos;
    public GameObject cam;
    public float parallaxEffectX;
    public float parallaxEffectY;

    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float distX = (cam.transform.position.x * parallaxEffectX);
        float distY = (cam.transform.position.y * parallaxEffectY);
        //float distY = 0.0f; // Disable vertical parallax for now

        transform.position = startPos + new Vector3(distX, distY, 0.0f);
    }
}