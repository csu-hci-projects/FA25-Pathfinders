using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float speed = 6.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speedFactor = speed * Time.deltaTime;
        float x = speedFactor * Input.GetAxis("Horizontal");
        float z = speedFactor * Input.GetAxis("Vertical");
        float y = 0;

        if (Input.GetKey(KeyCode.Q)) {
            y = speedFactor;
        }

        else if (Input.GetKey(KeyCode.E)) {
            y = speedFactor * -1;
        }

        transform.Translate(x, y, z);
    }
}
/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovePlayer : MonoBehaviour
{
    [SerializeField] float speed = 6.0f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = 0f;

        if (Input.GetKey(KeyCode.Q))
            y = 1f;
        else if (Input.GetKey(KeyCode.E))
            y = -1f;

        Vector3 move = new Vector3(x, y, z).normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}*/