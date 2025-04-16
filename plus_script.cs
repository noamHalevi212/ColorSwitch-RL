using UnityEngine;

public class plus_script : MonoBehaviour
{
    public float rotate_speed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotate_speed * Time.deltaTime, Space.Self);
    }
}
