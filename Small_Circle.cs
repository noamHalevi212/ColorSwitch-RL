using UnityEngine;

public class Small_Circle : MonoBehaviour
{
    public float rotate_speed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotate_speed * Time.deltaTime);
    }
}
