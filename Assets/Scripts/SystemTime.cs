using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SystemTime : MonoBehaviour
{
    Animator animator;
    public GameObject Display;
    public int hour;
    public int meridiemHour;
    public string meridiem = "AM";
    public int minutes;
    public int seconds;
    public int total_seconds;

    [Range(0,86400)][SerializeField] private int DebugAddSeconds;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime tempTime = DateTime.Now.AddSeconds(DebugAddSeconds);
        hour = tempTime.Hour;
        if (tempTime.Hour == 0)
        {
            meridiemHour = 12;
            meridiem = "AM";
        }
        else if (tempTime.Hour < 12)
        {
            meridiemHour = tempTime.Hour;
            meridiem = "AM";
        }
        else if (tempTime.Hour == 12)
        {
            meridiemHour = 12;
            meridiem = "PM";
        }
        else
        {
            meridiemHour = tempTime.Hour-12;
            meridiem = "PM";
        }
        minutes = tempTime.Minute;
        seconds = tempTime.Second;
        total_seconds = (((hour * 60) + minutes) * 60) + seconds;
        if (Display.gameObject.activeSelf)
        {
            Display.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00} {2}", meridiemHour, minutes, meridiem);
        }
        animator.SetFloat("SystemTime", (float)total_seconds);
    }
}
