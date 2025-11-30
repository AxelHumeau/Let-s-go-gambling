using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Camera))]
public class FreeCam : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector2 navigateInput;
    private Camera cam;
    [SerializeField] private TurnManager turnManager;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void OnNavigate(CallbackContext ctx)
    {
        if (turnManager.SelectedAction != SelectedAction.FreeCam)
        {
            return;
        }
        navigateInput = ctx.ReadValue<Vector2>();
    }

    void Update()
    {
        if (turnManager.SelectedAction != SelectedAction.FreeCam)
        {
            return;
        }
        Vector3 worldMove =
            cam.transform.right * navigateInput.x +
            cam.transform.up * navigateInput.y;


        cam.transform.position += worldMove * moveSpeed * Time.deltaTime;
    }
}
