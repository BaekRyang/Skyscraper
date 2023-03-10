using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject spriteBox;
    [SerializeField] Material playerMat;

    [SerializeField] Texture[] playerTexture;

    private const float RAY_DISTANCE = 2f;
    
    private RaycastHit slopeHit;
    private float maxSlopeAngle = 90f;

    [SerializeField] Transform groundCheck;

    private int groundLayer;

    [SerializeField] bool isOnSlope;
    [SerializeField] bool isGrounded;

    public float MoveSpeed { get { return MoveSpeed; } }
    [SerializeField] float moveSpeed;
    Rigidbody rigid;
    Vector3 direction;
    int faceing;

    Vector3 camPos;

    Vector3 camRot;
    [SerializeField] float camRotX;

    float horizontal;
    float vertical;
    private void Start()
    {
        playerTexture = Resources.LoadAll<Texture>("Sprites/Player");
    }

    private void Update()
    {

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = transform.forward * vertical + transform.right * horizontal;
        direction.Normalize();
        Debug.DrawRay(transform.position, direction, Color.yellow);

        camRot = Camera.main.transform.rotation.eulerAngles;
        camRot.x = camRotX;
        rigid.rotation = Quaternion.Euler(camRot);

        LookAt();  
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        camPos = Camera.main.transform.position;
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }

    private void FixedUpdate()
    {
        if (horizontal == 0 && vertical == 0)
        {
            rigid.velocity = Vector3.zero;
        } else
        {
            Move();
        }
    }

    protected void Move()
    {
        Camera.main.transform.position = camPos + transform.position;
        float currentMoveSpeed = moveSpeed;

        isOnSlope = IsOnSlope();
        isGrounded = IsGrounded();
        Vector3 velocity = new Vector3(direction.x, rigid.velocity.y, direction.z);
        Vector3 gravity = Vector3.down * Mathf.Abs(rigid.velocity.y);

        if (isGrounded && isOnSlope)
        {
            velocity = AdjustDirectionToSlope(direction);
            gravity = Vector3.zero;
            rigid.useGravity = false;
        }
        else
        {
            rigid.useGravity = true;
        }
        rigid.velocity = velocity * currentMoveSpeed + gravity;
    }

    protected void LookAt()
    {
        if (horizontal == 0 && vertical == 0)
        {
            // ?????????? ?????? ??????
            faceing = 5;
        }
        else if (horizontal == 1 && vertical == 1)
        {
            // player is moving right and up (diagonal)
            faceing = 9;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[1];
            spriteBox.transform.localScale = new Vector3(-1, 1.7f, 0.01f);
        }
        else if (horizontal == 1 && vertical == -1)
        {
            // player is moving right and down (diagonal)
            faceing = 3;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[3];
            spriteBox.transform.localScale = new Vector3(-1, 1.7f, 0.01f);
        }
        else if (horizontal == -1 && vertical == 1)
        {
            // player is moving left and up (diagonal)
            faceing = 7;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[1];
            spriteBox.transform.localScale = new Vector3(1, 1.7f, 0.01f);
        }
        else if (horizontal == -1 && vertical == -1)
        {
            // player is moving left and down (diagonal)
            faceing = 1;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[3];
            spriteBox.transform.localScale = new Vector3(1, 1.7f, 0.01f);
        }
        else if (horizontal == 1)
        {
            // player is moving right
            faceing = 6;
        }
        else if (horizontal == -1)
        {
            // player is moving left
            faceing = 4;
        }
        else if (vertical == 1)
        {
            // player is moving up
            faceing = 8;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[0];
            spriteBox.transform.localScale = new Vector3(1, 1.7f, 0.01f);
        }
        else if (vertical == -1)
        {
            // player is moving down
            faceing = 2;
            spriteBox.GetComponent<Renderer>().material.mainTexture = playerTexture[2];
            spriteBox.transform.localScale = new Vector3(1, 1.7f, 0.01f);
        }
    }
    
    protected bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, RAY_DISTANCE, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction)
    { //???????? ???? ???? Vector?? ?????? ???? Normal Vector?? ???? ?????? ???? ???????? ?????? Vector?? ????????.
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    protected bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(transform.lossyScale.x/2, 0.05f, transform.lossyScale.z/2);
        return Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxSize = new Vector3(transform.lossyScale.x/2, 0.05f, transform.lossyScale.z/2);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }
}
