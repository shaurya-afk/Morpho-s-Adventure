using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class Morpho : MonoBehaviour
{
    public float speed;
    Rigidbody2D morpho;
    float moveX;
    float moveY;

    //jump
    private bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public float jumpForce = 10f;
    float jumpTimeCounter;
    public float jumpTime;
    public bool isJumping;

    //Flip
    public bool facingRight = true;

    //Shape Shifting
    public SpriteRenderer spriteRenderer;
    public Sprite balloon, stone, key, boat, morpe;

    //health and reward
    public float health;
    public float coins;
    public TextMeshProUGUI coinText;
    public GameObject coinParticles, wonParticles, waterSplash;

    // Start is called before the first frame update
    void Start()
    {
        health = 100f;
        morpho = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = morpe;
        MorphoS();
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "X" + coins.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        isGrounded=Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping=true;
            jumpTimeCounter = jumpTime;
            morpho.velocity = Vector2.up * jumpForce;
        }
        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                morpho.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping= false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            spriteRenderer.sprite = balloon;
            Balloon();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            spriteRenderer.sprite = morpe;
            MorphoS();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            spriteRenderer.sprite = stone;
            Rock();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            spriteRenderer.sprite = key;
            Key();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            spriteRenderer.sprite = boat;
            Boat();
        }
        
    }
    private void FixedUpdate()
    {
        float moveIp = Input.GetAxisRaw("Horizontal");
        morpho.velocity = new Vector2(moveIp * speed, morpho.velocity.y);
        if (facingRight && moveIp < 0)
        {
            Flip();
        }
        else if (!facingRight && moveIp > 0)
        {
            Flip();
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    void MorphoS()
    {
        spriteRenderer.flipX= false;
        jumpForce = 5f;
        speed = 5f;
        morpho.gravityScale = 9.8f;
        GetComponent<CapsuleCollider2D>().size = new Vector2(.43f, .7f);
        transform.localScale = new Vector2(2f, 2f);
        facingRight= true;
        transform.position = new Vector2(transform.position.x, transform.position.y+.5f);
    }
    void Balloon()
    {
        spriteRenderer.flipX= false;
        jumpForce = 1.5f;
        morpho.gravityScale = 0.001f;
        GetComponent<CapsuleCollider2D>().size = new Vector2(.4f, 1f);
        transform.localScale = new Vector2(2f, 2f);
        facingRight= true;
        transform.position = new Vector2(transform.position.x, transform.position.y+.5f);
    }
    void Rock()
    {
        spriteRenderer.flipX = false;
        speed = 12f;
        morpho.gravityScale = 10f;
        jumpForce = 15f;
        GetComponent<CapsuleCollider2D>().size = new Vector2(.4f, .4f);
        transform.localScale = new Vector2(1.5f, 1.5f);
        facingRight= true;
        transform.position = new Vector2(transform.position.x, transform.position.y + .5f);
    }
    void Key()
    {
        spriteRenderer.flipX = false;
        speed = 5f;
        morpho.gravityScale = 9.8f;
        jumpForce = 0f;
        facingRight= true;
        transform.localScale = new Vector2(.25f, .25f);
        GetComponent<CapsuleCollider2D>().size = new Vector2(2.4f, 6.6f);
        transform.position = new Vector2(transform.position.x, transform.position.y + .5f);
    }
    void Boat()
    {
        spriteRenderer.flipX = true;
        speed = 12f;
        morpho.gravityScale = 9.8f;
        jumpForce = 0f;
        facingRight= true;
        transform.localScale = new Vector2(5f, 5f);
        GetComponent<CapsuleCollider2D>().size = new Vector2(1.3f, .46f);
        transform.position = new Vector2(transform.position.x, transform.position.y + .5f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (spriteRenderer.sprite == stone && collision.gameObject.CompareTag("Obs"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Respawn"))
        {
            health -= health;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            Instantiate(waterSplash, new Vector2(transform.position.x,transform.position.y - 3f), Quaternion.identity);
            AudioManager.PlayAudio("splash");
        }
        if (collision.gameObject.CompareTag("Water") && spriteRenderer.sprite != boat)
        {
            health -= health;
        }
        if (collision.gameObject.CompareTag("Coin"))
        {
            coins++;
            Instantiate(coinParticles, collision.gameObject.transform.position, Quaternion.identity);
            AudioManager.PlayAudio("point");
            Destroy(collision.gameObject);
        }
        if (spriteRenderer.sprite == key && collision.gameObject.CompareTag("Finish"))
        {
            StartCoroutine(Won());
        }
        if (spriteRenderer.sprite == key && collision.gameObject.CompareTag("Next"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    IEnumerator Won()
    {
        AudioManager.PlayAudio("won");
        Instantiate(wonParticles,transform.position, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
}
