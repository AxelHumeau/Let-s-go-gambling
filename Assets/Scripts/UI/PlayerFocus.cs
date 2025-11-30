using UnityEngine;

public class PlayerFocus : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Camera cameraToControl;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private bool smoothFollow = true;

    void Start()
    {
    }

    void Update()
    {
        if (turnManager.SelectedAction == SelectedAction.FreeCam)
        {
            return;
        }
        Player toFollow = turnManager.CurrentPlayer;
        if (turnManager.SelectedAction == SelectedAction.SelectItemTarget)
        {
            toFollow = turnManager.Players[turnManager.SelectedItemTargetIndex];
        }
        if (toFollow != null && cameraToControl != null)
        {
            Vector3 targetPosition = toFollow.transform.position + offset;

            if (smoothFollow)
            {
                cameraToControl.transform.position = Vector3.Lerp(
                    cameraToControl.transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );
            }
            else
            {
                cameraToControl.transform.position = targetPosition;
            }
        }
    }
}
