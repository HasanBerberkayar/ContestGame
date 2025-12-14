using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 75f;
    private float _vInput;
    private float _hInput;
    private Rigidbody _rb;

    public float JumpVelocity = 5f;
    private bool _isJumping;
    public float DistanceToGround = 0.1f;
    public LayerMask GroundLayer;
    private CapsuleCollider _col;

    public GameObject Bullet;
    public float BulletSpeed = 100f;
    private bool _isShooting;
    public GameObject Cam;
    public float ShoulderOffset = 1f;
    public float BulletHeight = 0.5f;

    public float FireRate = 0.3f;
    private float _fireTimer = 0f;
    // Unity Message | 0 references
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
    }

    // Unity Message | 0 references
    void Update()
    {
        _vInput = Input.GetAxis("Vertical") * MoveSpeed;
        _hInput = Input.GetAxis("Horizontal") * RotateSpeed;
        //this.transform.Translate(Vector3.forward * _vInput * Time.deltaTime);
        //this.transform.Rotate(Vector3.up * _hInput * Time.deltaTime);
        _isJumping |= Input.GetKeyDown(KeyCode.Space);
        _isShooting |= Input.GetMouseButton(0);

        if (_fireTimer > 0f)
        {
            _fireTimer -= Time.deltaTime;
        }
    }

    // Unity Message | 0 references
    void FixedUpdate()
    {
        if (IsGrounded() && _isJumping)
        {
            _rb.AddForce(Vector3.up * JumpVelocity, ForceMode.Impulse);
        }
        /*
        if (_isJumping)
        {
            _rb.AddForce(Vector3.up * JumpVelocity, ForceMode.Impulse);
        }*/
        _isJumping = false;

        Vector3 rotation = Vector3.up * _hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        _rb.MovePosition(this.transform.position + this.transform.forward * _vInput * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * angleRot);

        if (_isShooting && _fireTimer <= 0f) 
        {
            Vector3 spawnPos;

            if (Cam.GetComponent<CameraBehavior>().isLeftShoulder)
            {
                spawnPos = transform.position
                 - transform.right * ShoulderOffset
                 + Vector3.up * BulletHeight;
            }
            else
            {
                spawnPos = transform.position
                         + transform.right * ShoulderOffset
                         + Vector3.up * BulletHeight;
            }
            GameObject newBullet = Instantiate(Bullet, spawnPos, Quaternion.identity);

            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.linearVelocity = Cam.transform.forward * BulletSpeed;

            /*
            GameObject newBullet = Instantiate(Bullet, this.transform.position + new Vector3(0,0,1), this.transform.rotation);
            Rigidbody BulletRB = newBullet.GetComponent<Rigidbody>();
            BulletRB.linearVelocity = this.transform.forward * BulletSpeed;*/

            _fireTimer = FireRate;
        }
        _isShooting = false;
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(_col.bounds.center.x, _col.bounds.min.y, _col.bounds.center.y);

        bool grounded = Physics.CheckCapsule(_col.bounds.center, capsuleBottom, DistanceToGround, GroundLayer, QueryTriggerInteraction.Ignore);

        return grounded;
    }
}