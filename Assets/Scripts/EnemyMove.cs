using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;

    public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        EnemyMoveing();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        Invoke("EnemyMoveing", 5);
    }

    void FixedUpdate()
    {
        //몬스터 이동(한칸앞에 Ray쏘기)
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //지형 확인
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) {
            EnemyTurn();
            //Debug.Log("경고 이 앞 낭떨어지!");
        }
    }

    //몬스터 랜덤 움직임 설정
    void EnemyMoveing()
    {
        //몬스터 다음 활동
        nextMove = Random.Range(-1, 2); //최솟값은 수치 포함O, 최댓값은 수치 포함X

        //몬스터 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);

        //몬스터 방향전환(Flip)
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //몬스터 다음활동 시간 랜덤
        float nextThinkTime = Random.Range(2f, 5f);

        //재귀(반복)
        Invoke("EnemyMoveing", nextThinkTime);
    }

    void EnemyTurn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("EnemyMoveing", 2);
    }

    //몬스터가 데미지 입었을때 함수
    public void OnDamaged()
    {
        //색 흐리게 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //방향 전환
        spriteRenderer.flipX= true;
        //물체 충돌 무시
        capsuleCollider.enabled = false;
        //데미지 모션점프
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //삭제
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}