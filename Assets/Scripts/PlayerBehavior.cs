using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float strafeMultiplier = 0.5f;

    private float vInput;
    private float hInput;
    private Rigidbody rb;

    public float jumpVelocity = 5f;
    private bool isJumping;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    private CapsuleCollider col;
    public int maxJump = 2;
    public int jumpCount;

    public GameObject bullet;
    public float bulletSpeed = 100f;
    private bool isShooting;

    public GameObject cam;
    public float shoulderOffset = 1f;
    public float bulletHeight = 0.5f;

    public float fireRate = 0.3f;
    private float fireTimer = 0f;

    public int bulletMax = 10;
    public int bulletRemaining;
    public float reloadTime = 2f;
    public float reloadTimer = 0f;
    private bool isReloading = false;

    private Animator anim;
    public TMP_Text bulletText;
    bool isWalking;

    private int hidePressAmount = 0;
    public bool canEnemysSee = true;

    private int maxHealth = 10;
    private int currentHealth;
    public TMP_Text healthText;

    public GameObject dieScreen;

    public bool isMelee;
    private bool isAttacking;
    private float attackTimer;
    public int currentAttack;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        bulletRemaining = bulletMax;
        jumpCount = maxJump;
        currentHealth = maxHealth;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Hide();
            hidePressAmount++;
        }

        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        isJumping |= Input.GetKeyDown(KeyCode.Space);

        if (!isMelee)
        {
            isShooting |= Input.GetMouseButton(0);
        }
        else
        {
            Attack();
        }
        attackTimer += Time.deltaTime;

        isWalking = Mathf.Abs(vInput) > 0.01f || Mathf.Abs(hInput) > 0.01f;

        if (!isWalking)
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsHiding", false);
        }
        else
        {
            if (!canEnemysSee)
            {
                anim.SetBool("IsHiding", true);
                anim.SetBool("IsWalking", false);
            }
            else
            {
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsHiding", false);
            }
        }

        if (fireTimer > 0f)
            fireTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            isReloading = true;

        if ((bulletRemaining <= 0 || isReloading) && reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
            isReloading = true;
        }
        else if (reloadTimer <= 0)
        {
            bulletRemaining = bulletMax;
            reloadTimer = reloadTime;
            isReloading = false;
            UpdateBulletText();
        }

        if (isReloading)
        {
            MoveSpeed = canEnemysSee ? 5f : 2.5f;
        }
        else
        {
            MoveSpeed = canEnemysSee ? 10f : 5f;
        }
    }

    void FixedUpdate()
    {
        RotateWithCamera();
        Move();
        Jump();
        Shoot();
    }

    private void RotateWithCamera()
    {
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(camForward);
        rb.MoveRotation(targetRot);
    }

    private void Move()
    {
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        float strafeSpeed = MoveSpeed * strafeMultiplier;

        Vector3 moveDir =
            camForward * vInput * MoveSpeed +
            camRight * hInput * strafeSpeed;

        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
        return Physics.CheckCapsule(col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void Jump()
    {
        if (IsGrounded())
            jumpCount = maxJump;

        if (jumpCount > 1 && isJumping)
        {
            anim.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
            jumpCount--;
        }
        isJumping = false;
    }

    private void Shoot()
    {
        if (isShooting && fireTimer <= 0f && bulletRemaining > 0)
        {
            Vector3 spawnPos;

            if (cam.GetComponent<CameraBehavior>().isLeftShoulder)
                spawnPos = transform.position - transform.right * shoulderOffset + Vector3.up * bulletHeight;
            else
                spawnPos = transform.position + transform.right * shoulderOffset + Vector3.up * bulletHeight;

            GameObject newBullet = Instantiate(bullet, spawnPos, Quaternion.LookRotation(cam.transform.forward));
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.linearVelocity = cam.transform.forward * bulletSpeed;

            fireTimer = fireRate;
            bulletRemaining--;
            UpdateBulletText();
        }
        isShooting = false;
    }

    private void UpdateBulletText()
    {
        bulletText.text = "Bullet: " + bulletRemaining;
    }

    private void Hide()
    {
        canEnemysSee = hidePressAmount % 2 != 0;
    }

    public void TakeDamage()
    {
        currentHealth--;
        healthText.text = "Health: " + currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        dieScreen.SetActive(true);
    }

    public void Attack()
    {
        if(Input.GetMouseButton(0) && attackTimer > 0.8f)
        {
            currentAttack++;
            if (currentAttack > 3)
            {
                Debug.Log("a");
                currentAttack = 1;
            }
            if (attackTimer > 1)
            {
                Debug.Log("b");
                currentAttack = 1;
            }
            Debug.Log(currentAttack);
            anim.SetTrigger("Attack"+currentAttack);
            attackTimer = 0;
        }
    }
}