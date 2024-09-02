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

    //사운드 함수
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
        //캐릭터 점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            //사운드
            PlaySound("JUMP");
        }

        //방향전환
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //캐릭터 애니메이션
        if (rigid.velocity.x == 0)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //캐릭터 이동 속도
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);

        //이동 최대 속도 지정
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1))
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //지형확인 (캐릭터 기준 아래로 Ray쏘기)
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
            //캐릭터가 몬스터를 공격
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                OnAttack(collision.transform);
            else
                OnDamaged(collision.transform.position);
        }
    }

    //트리거 작동 함수
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //코인 포인트
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gamemanager.stagePoint += 50;
            else if (isSilver)
                gamemanager.stagePoint += 100;
            else if (isGold)
                gamemanager.stagePoint += 300;

            //아이템 삭제
            collision.gameObject.SetActive(false);

            //사운드
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish") {
            //스테이지 클리어 및 다음 스테이지 이동
            gamemanager.NextStage();
            //사운드
            PlaySound("FINISH");
        }

    }


    void OnAttack(Transform enemy)
    {
        //공격 포인트
        gamemanager.stagePoint += 100;
        //공격 넉백
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //몬스터 사망
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        if (enemyMove != null)
        {
            enemyMove.OnDamaged();
        }
        else
        {
            Debug.LogWarning("EnemyMove 컴포넌트를 찾을 수 없습니다.");
        }

        //사운드
        PlaySound("ATTACK");
    }

    //캐릭터가 피해를 입었을때 효과 함수
    void OnDamaged(Vector2 targetPos)
    {
        if (gamemanager.health > 1) {
            //캐릭터 피격시 목숨 -1
            gamemanager.HealthDown();

            //캐릭터 레이어 변경
            gameObject.layer = 8;

            //캐릭터 투명도 변경
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);//마지막은 투명도

            //캐릭터 피해넉백
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

            //무적상태 해제
            anim.SetTrigger("doDamaged");
            Invoke("OffDamaged", 3);

            //사운드
            PlaySound("DAMAGED");
        }
        else
            gamemanager.HealthDown();
    }

    //무적상태 해제 함수
    void OffDamaged()
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //색 흐리게 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //방향 전환
        spriteRenderer.flipX = true;
        //물체 충돌 무시
        BoxCollider.enabled = false;
        //데미지 모션점프
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //사운드
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}