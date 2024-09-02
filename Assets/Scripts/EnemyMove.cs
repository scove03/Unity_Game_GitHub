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
        //���� �̵�(��ĭ�տ� Ray���)
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //���� Ȯ��
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) {
            EnemyTurn();
            //Debug.Log("��� �� �� ��������!");
        }
    }

    //���� ���� ������ ����
    void EnemyMoveing()
    {
        //���� ���� Ȱ��
        nextMove = Random.Range(-1, 2); //�ּڰ��� ��ġ ����O, �ִ��� ��ġ ����X

        //���� �ִϸ��̼�
        anim.SetInteger("WalkSpeed", nextMove);

        //���� ������ȯ(Flip)
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //���� ����Ȱ�� �ð� ����
        float nextThinkTime = Random.Range(2f, 5f);

        //���(�ݺ�)
        Invoke("EnemyMoveing", nextThinkTime);
    }

    void EnemyTurn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("EnemyMoveing", 2);
    }

    //���Ͱ� ������ �Ծ����� �Լ�
    public void OnDamaged()
    {
        //�� �帮�� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //���� ��ȯ
        spriteRenderer.flipX= true;
        //��ü �浹 ����
        capsuleCollider.enabled = false;
        //������ �������
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //����
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}