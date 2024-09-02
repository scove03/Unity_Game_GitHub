using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gamemanager;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D BoxCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        BoxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    //���� �Լ�
    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump; break;
            case "ATTACK":
                audioSource.clip = audioAttack; break;
            case "DAMAGED":
                audioSource.clip = audioDamaged; break;
            case "ITEM":
                audioSource.clip = audioItem; break;
            case "DIE":
                audioSource.clip = audioDie; break;
            case "FINISH":
                audioSource.clip = audioFinish; break;
        }
        audioSource.Play();
    }

    void Update()
    {
        //ĳ���� ����
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            //����
            PlaySound("JUMP");
        }

        //������ȯ
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //ĳ���� �ִϸ��̼�
        if (rigid.velocity.x == 0)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //ĳ���� �̵� �ӵ�
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);

        //�̵� �ִ� �ӵ� ����
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1))
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //����Ȯ�� (ĳ���� ���� �Ʒ��� Ray���)
        if (rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
                //Debug.Log(rayHit.collider.name);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") {
            //ĳ���Ͱ� ���͸� ����
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                OnAttack(collision.transform);
            else
                OnDamaged(collision.transform.position);
        }
    }

    //Ʈ���� �۵� �Լ�
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //���� ����Ʈ
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gamemanager.stagePoint += 50;
            else if (isSilver)
                gamemanager.stagePoint += 100;
            else if (isGold)
                gamemanager.stagePoint += 300;

            //������ ����
            collision.gameObject.SetActive(false);

            //����
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish") {
            //�������� Ŭ���� �� ���� �������� �̵�
            gamemanager.NextStage();
            //����
            PlaySound("FINISH");
        }

    }


    void OnAttack(Transform enemy)
    {
        //���� ����Ʈ
        gamemanager.stagePoint += 100;
        //���� �˹�
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //���� ���
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.OnDamaged();
        }
        else
        {
            Debug.LogWarning("EnemyMove ������Ʈ�� ã�� �� �����ϴ�.");
        }

        //����
        PlaySound("ATTACK");
    }

    //ĳ���Ͱ� ���ظ� �Ծ����� ȿ�� �Լ�
    void OnDamaged(Vector2 targetPos)
    {
        if (gamemanager.health > 1) {
            //ĳ���� �ǰݽ� ��� -1
            gamemanager.HealthDown();

            //ĳ���� ���̾� ����
            gameObject.layer = 8;

            //ĳ���� ���� ����
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);//�������� ����

            //ĳ���� ���س˹�
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

            //�������� ����
            anim.SetTrigger("doDamaged");
            Invoke("OffDamaged", 3);

            //����
            PlaySound("DAMAGED");
        }
        else
            gamemanager.HealthDown();
    }

    //�������� ���� �Լ�
    void OffDamaged()
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //�� �帮�� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //���� ��ȯ
        spriteRenderer.flipX = true;
        //��ü �浹 ����
        BoxCollider.enabled = false;
        //������ �������
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //����
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}