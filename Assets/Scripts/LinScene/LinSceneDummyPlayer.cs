using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LinSceneDummyPlayer : MonoBehaviour, PlayerInputAction.IPlayerActions
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    private Vector2 moveInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputAction userInput = new PlayerInputAction();
        userInput.Player.SetCallbacks(this);
        userInput.Player.Enable();

        StartCoroutine(Step());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private IEnumerator Step()
    {
        while(true)
        {
            yield return new WaitForSeconds(3.0f);
            VisualRangeGenerator.Instance?.Generate(transform.position, 1);
        }
    }

    private void Move()
    {
        Vector3 moveVector = Vector3.zero;
        moveVector += transform.forward * moveInput.y * _moveSpeed * Time.deltaTime;
        moveVector += transform.right * moveInput.x * _moveSpeed * Time.deltaTime;

        transform.position += moveVector;
    }

    #region PlayerInputAction.IPlayerActions

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Vector3 rotateVector = Vector3.zero;
        rotateVector.y += value.x * _rotateSpeed * Time.deltaTime;

        transform.Rotate(rotateVector);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    #endregion
}
