using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class gamePlayer : MonoBehaviour
{
    GameObject[] cone;
    float longDist = 300f;
    Vector3 target;
    float hitCone = 0;
    float hitWall = 0;
    public TMP_Text gameText;
    public TMP_Text timerText;
    private float timeElapsed = 0f; // �����w�g�L���ɶ�

    string  gameString = string.Empty;
    KartGame.KartSystems.ArcadeKart kart;


    // Start is called before the first frame update
    void Start()
    {
        kart = GetComponent<KartGame.KartSystems.ArcadeKart>();
    }
    void hitSomething() 
    {
        hitCone = kart.hitConeNumber;
        hitWall = kart.hitWallNumber;
        gameString = "Hit Cones:" + hitCone + "\nHit Wall:" + hitWall;
        gameText.text = gameString;
    }
    void playTime()
    {
        // �C�V��s�ɶ�
        timeElapsed += Time.deltaTime;

        // �p������M���
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        // �榡�Ʈɶ������
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // Update is called once per frame
    void Update()
    {
        playTime();
        hitSomething();
        //trafficCone_prefab
        if (Input.GetButtonUp("Pause Menu"))
        {
            searchCone();
        }
    }
    void searchCone()
    {
        float dist;
        cone = GameObject.FindGameObjectsWithTag("cone");
        if (cone != null) {
            foreach (GameObject go in cone) { 
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist < longDist) { 
                    longDist = dist;
                    target = go.transform.position;
                }
            }
            target.y = target.y + 10f;
            this.transform.position = target;
            longDist = 300f;
        }
    }
}
