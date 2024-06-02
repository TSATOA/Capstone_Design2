using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
 public class Timer : MonoBehaviour
 {
    public TextMeshProUGUI Minute;
    public TextMeshProUGUI Second;
    //public Text colon;
    float limit_time = 120; // 제한 시간 120초
    int min, sec;
    
    void Start()
    {
        //제한 시간 02:00
        Minute.text = "02";
        Second.text = "00";

    }
    
        void Update()
    { 
        limit_time -= Time.deltaTime;

        min = (int)limit_time / 60;
        sec = ((int)limit_time - min * 60) % 60;

        if (min <= 0 && sec <= 0)
        {
            Minute.text = 0.ToString();
            Second.text = 0.ToString();
        }

        else
        {
            Minute.text = min.ToString();
            Second.text = sec.ToString();
        }
   }
 }