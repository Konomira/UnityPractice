using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    [Range(100f,500.0f)]
    private float mouseSensitivity = 250.0f;
    
    [SerializeField]
    Transform head;

    bool showingCursor = true;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float moveSpeed = 2.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            showingCursor = !showingCursor;

        float actualSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            actualSpeed = moveSpeed * 2;
        else
            actualSpeed = moveSpeed;

        if(showingCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        head.Rotate(new Vector3(mouseInput.y,0,0) * mouseSensitivity * Time.deltaTime);

        transform.Rotate(new Vector3(0, mouseInput.x) * mouseSensitivity * Time.deltaTime);

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        transform.Translate(new Vector3(input.x, 0.0f, input.y) * actualSpeed * Time.deltaTime,Space.Self);
    }
}
