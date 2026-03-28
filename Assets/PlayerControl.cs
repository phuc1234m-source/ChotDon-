using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool canControl = false;
    public float introSpeed = 3f; // How fast bike moves during Intro phase
    private float targetIntroY; // For Intro Y location

    public float moveSpeed = 5f;   // How fast the player moves
    private Rigidbody2D rb;        // Reference to the Rigidbody
    private Vector2 movement;      // Stores input direction

    public float maxTiltAngle = 25f;     // How much the bike tilts
    public float tiltSpeed = 8f;         // How fast it rotates

    public float leftLimit;
    public float rightLimit;

    private SpriteRenderer sr;
    private Camera cam;

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();

        float bottom = Camera.main.transform.position.y - Camera.main.orthographicSize; // Bottom of screen
        float height = Camera.main.orthographicSize * 2f; // Height of screen

        targetIntroY = bottom + height * 0.25f; // 1/4 from bottom

        float startY = bottom - 1f; // Set starting position of bike slightly below screen 
        transform.position = new Vector3(transform.position.x, startY, transform.position.z); // Apply that position

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;


    }


    void Update()
    {

        if (!canControl)
            return; // If the condition isn't met (player can Control) then the codes below will not be executed

        // Get WASD / Arrow key input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize so diagonal movement isn't faster
        movement = movement.normalized;

        // Calculate target rotation based on horizontal movement
        float targetAngle = -movement.x * maxTiltAngle;

        // Get current rotation
        float currentAngle = transform.eulerAngles.z;

        // Convert to -180 to 180 range (important for smooth rotation)
        if (currentAngle > 180) currentAngle -= 360;

        // Smoothly interpolate to target angle
        float newAngle = Mathf.Lerp(currentAngle, targetAngle, tiltSpeed * Time.deltaTime);

        // Apply rotation
        transform.rotation = Quaternion.Euler(0, 0, newAngle);

    }

    void FixedUpdate()
    {
        if (!canControl)
        {
            // Move upward automatically
            rb.linearVelocity = Vector2.up * introSpeed;

            // Check if we've reached target
            if (transform.position.y >= targetIntroY)
            {
                rb.linearVelocity = Vector2.zero;
                canControl = true;
            }

            return; // Intro sequence  
        }
        // Move using physics
        rb.linearVelocity = movement * moveSpeed;

        // Clamp position
        Vector3 pos = transform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = cam.transform.position.x - camWidth;
        float maxX = cam.transform.position.x + camWidth;
        float minY = cam.transform.position.y - camHeight;
        float maxY = cam.transform.position.y + camHeight;

        float halfWidth = sr.bounds.extents.x;
        float halfHeight = sr.bounds.extents.y;

        pos.x = Mathf.Clamp(pos.x, leftLimit + halfWidth, rightLimit - halfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + halfHeight, maxY - halfHeight);

        transform.position = pos;





    }
    public bool CanControl
    {
        get { return canControl; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.name);

        if (!canControl) return;

        if (other.GetComponent<CarObstacle>() != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
    
