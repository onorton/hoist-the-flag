using UnityEngine;

public class RockShip : MonoBehaviour
{
    [SerializeField]
    private float bobbingSpeed = 3f;
    [SerializeField]
    private float bobbingAmount = 0.15f;
    [SerializeField]
    private float rollAmount = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Bob up and down
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;

        // Tilt side to side
        float newRotation = Mathf.Cos(Time.time * bobbingSpeed) * rollAmount;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }
}
