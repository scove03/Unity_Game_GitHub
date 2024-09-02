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

    //다음 스테이지 넘어갈때 함수
    public void NextStage()
    {
        //스테이지 이동
        if (stageIndex < Stages.Length-1) {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        //게임 클리어
        else {
            //시간 멈춤
            Time.timeScale = 0;
            Debug.Log("게임 클리어!");
            //재시작 버튼 활성화
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }
         
        //점수 저장
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    //목숨 감소 함수
    public void HealthDown()
    {
        //목숨 감소
        if (health > 1) {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
            //사운드
            player.PlaySound("DAMAGED");
        }
        else
        {

            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //캐릭터 사망 이펙트 
            player.OnDie();
            
            Debug.Log("죽었습니다.");

            //재시작 버튼 활성화
            UIRestartBtn.SetActive(true);
        }
    }

    //트리거 적용
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player") {
          
            //캐릭터 위치 재조정
            if (health > 1) {
                PlayerReposition();
            }

            //캐릭터 목숨 -1
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
