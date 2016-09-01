using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public event System.Action OnNewDay;
    public event System.Action OnNewWeek;
    public event System.Action OnNewMonth;
    public event System.Action OnNewYear;
    public event System.Action OnTimeChange;

    public float secsPerDay = 2;
    public int daysPerWeek = 7;
    public int weeksPerMonth = 4;
    public int monthsPerYear = 12;

    private float _secsAcum;
    public int daysAcum;
    public int weeksAcum;
    public int monthsAcum;
    public int yearsAcums;

    void Awake()
    {
        _secsAcum = Mathf.Infinity;
        daysAcum = 0;
        weeksAcum = 0;
        monthsAcum = 0;
        yearsAcums = 0;
    }

    void Update()
    {
        _secsAcum += Time.deltaTime;
        if ( _secsAcum >= secsPerDay )
        {
            _secsAcum = 0;
            daysAcum++;
            if ( OnNewDay != null )
                OnNewDay();
            
            if ( daysAcum >= daysPerWeek )
            {
                daysAcum = 0;
                weeksAcum++;
                if ( OnNewWeek != null )
                    OnNewWeek();

                if ( weeksAcum >= weeksPerMonth )
                {
                    weeksAcum = 0;
                    monthsAcum++;
                    if ( OnNewMonth != null )
                        OnNewMonth();

                    if ( monthsAcum >= monthsPerYear )
                    {
                        monthsAcum = 0;
                        yearsAcums++;
                        if ( OnNewYear != null )
                            OnNewYear();
                    }
                }
            }

            if ( OnTimeChange != null )
                OnTimeChange();
        }
    }
}
