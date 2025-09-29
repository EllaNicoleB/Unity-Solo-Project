using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public Transform playerCam;
    Rigidbody rb;
    Ray jumpRay;
    Vector2 cameraRotation = Vector2.zero;
    Vector3 cameraOffset = new Vector3(0, .5f, .5f);
    InputAction lookAxis;

    public int health = 3;
    public int maxHealth = 5;

    public float Xsensitivity = .1f;
    public float Ysensitivity = .1f;
    float inputX;
    float inputY;
    public float speed = 5f;
    public float jumpHeight = 2.5f;
    public float groundDetectionDistance = 1.1f;
    public float camRotationLimit = 180;

    // Start is called before the first frame update
    void Start()
    {
        jumpRay = new Ray();
        rb = GetComponent<Rigidbody>();
        lookAxis = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");
        playerCam = transform.GetChild(0);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Camera Handler
        playerCam.position = transform.position + cameraOffset;

        cameraRotation.x += lookAxis.ReadValue<Vector2>().x * Xsensitivity;
        cameraRotation.y += lookAxis.ReadValue<Vector2>().y * Ysensitivity;

        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -camRotationLimit, camRotationLimit);

        playerCam.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0);
        transform.rotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);

        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        // Movement System
        Vector3 tempMove = rb.velocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;


        rb.velocity = (tempMove.x * transform.forward) + (tempMove.y * transform.up) + (tempMove.z * transform.right);

    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 InputAxis = context.ReadValue<Vector2>();
        inputX = InputAxis.x;
        inputY = InputAxis.y;
    }

    public void Jump()
    {
        if (Physics.Raycast(jumpRay, groundDetectionDistance))
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hazard")
            health--;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "killzone")
            health = 0;

        if (other.tag == "health" && health < maxHealth)
        {
            health++;
            Destroy(other.gameObject);
        }
    }
}

