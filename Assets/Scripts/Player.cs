using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IKitchenObjectParent
{

    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private VariableJoystick variableJoystick; // Joystick input
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private Rigidbody rb; // Rigidbody for movement

    private bool isWalking;
    private Vector3 lastInteractDir;
    public BaseCounter selectedCounter;
    public KitchenObject kitchenObject;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button alternateInteractButton;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start()
    {
        interactButton.onClick.AddListener(HandleInteractAction);
        alternateInteractButton.onClick.AddListener(HandleAlternateInteractAction);
    }
        private void FixedUpdate()
    {
        HandleMovement();
        HandleInteractions();
    }


    public bool IsWalking()
    {
        return isWalking;
    }

    public void HandleInteractAction()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        Debug.Log("pickup");
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    public void HandleAlternateInteractAction()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }
    private void HandleInteractions()
    {
        Vector3 moveDir = new Vector3(variableJoystick.Horizontal, 0f, variableJoystick.Vertical);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(variableJoystick.Horizontal, 0f, variableJoystick.Vertical);

        if (direction.magnitude >= 0.1f)
        {
            float moveDistance = moveSpeed * Time.fixedDeltaTime;
            float playerRadius = 0.7f;
            float playerHeight = 2f;

            // Kiểm tra va chạm với lớp counterLayerMask bằng Raycast
            Ray ray = new Ray(transform.position + Vector3.up * (playerHeight / 2), direction);
            bool canMove = !Physics.SphereCast(ray, playerRadius, moveDistance, countersLayerMask);

            if (canMove)
            {
                // Sử dụng MovePosition thay vì AddForce để di chuyển
                Vector3 targetPosition = transform.position + direction.normalized * moveDistance;
                rb.MovePosition(targetPosition);
            }
        }

        isWalking = direction != Vector3.zero;

        // Xoay nhân vật theo hướng di chuyển
        if (isWalking)
        {
            float rotateSpeed = 10f;
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            transform.forward = Vector3.Slerp(transform.forward, lookDirection, Time.deltaTime * rotateSpeed);
        }
    }



    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
