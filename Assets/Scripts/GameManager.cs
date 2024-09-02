using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIpoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

    void Update()
    {
        UIpoint.text = (totalPoint + stagePoint).ToString();
    }

    //���� �������� �Ѿ�� �Լ�
    public void NextStage()
    {
        //�������� �̵�
        if (stageIndex < Stages.Length-1) {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        //���� Ŭ����
        else {
            //�ð� ����
            Time.timeScale = 0;
            Debug.Log("���� Ŭ����!");
            //����� ��ư Ȱ��ȭ
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }
         
        //���� ����
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    //��� ���� �Լ�
    public void HealthDown()
    {
        //��� ����
        if (health > 1) {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
            //����
            player.PlaySound("DAMAGED");
        }
        else
        {

            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //ĳ���� ��� ����Ʈ 
            player.OnDie();
            
            Debug.Log("�׾����ϴ�.");

            //����� ��ư Ȱ��ȭ
            UIRestartBtn.SetActive(true);
        }
    }

    //Ʈ���� ����
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player") {
          
            //ĳ���� ��ġ ������
            if (health > 1) {
                PlayerReposition();
            }

            //ĳ���� ��� -1
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

  
}
