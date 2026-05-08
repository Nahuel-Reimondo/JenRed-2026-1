using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private IMovementInput movementInput;

    private void Awake()
    {
        movementInput = new PlayerInput();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        Move();
    }

    private void Move()
    {
        float horizontal = movementInput.Horizontal;
        float vertical = movementInput.Vertical;
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveRotation = Vector3.forward;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        
    }
}
