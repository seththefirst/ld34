using UnityEngine;
using System.Collections;

public class IndicatorAnimation : MonoBehaviour
{
    private bool _showHearts;
    private bool _showArrow;
    private RectTransform _arrow;
    private RectTransform _h1;
    private RectTransform _h2;
    private RectTransform _h3;

    private float _arrowIniPosY;
    private float _h1IniPosY;
    private float _h2IniPosY;
    private float _h3IniPosY;


    void Awake()
    {
        _arrow = transform.FindChild( "Arrow" ).GetComponent<RectTransform>();
        _h1 = transform.FindChild( "Heart1" ).GetComponent<RectTransform>();
        _h2 = transform.FindChild( "Heart2" ).GetComponent<RectTransform>();
        _h3 = transform.FindChild( "Heart3" ).GetComponent<RectTransform>();

        _arrowIniPosY = _arrow.localPosition.y;
        _h1IniPosY = _h1.localPosition.y;
        _h2IniPosY = _h2.localPosition.y;
        _h3IniPosY = _h3.localPosition.y;

        ShowHearts( false );
        ShowArrow( false );

    }

    // Use this for initialization
    void Start()
    {
	
    }

    public void ShowHearts( bool enabled )
    {
        _showHearts = enabled;
        _h1.gameObject.SetActive( enabled );
        _h2.gameObject.SetActive( enabled );
        _h3.gameObject.SetActive( enabled );
    }

    public void ShowArrow( bool enabled )
    {
        _showArrow = enabled;
        _arrow.gameObject.SetActive( enabled );
    }

    // Update is called once per frame
    void Update()
    {
        if ( _showArrow )
        { 
            _arrow.localPosition = new Vector3( _arrow.localPosition.x, PingPong( Time.time * 100, -_arrowIniPosY, 0 ), _arrow.localPosition.z );
        }

        if ( _showHearts )
        {
            _h1.localPosition = new Vector3( _h1.localPosition.x, PingPong( Time.time * 60, _h1IniPosY, 0 ), _h1.localPosition.z );
            _h2.localPosition = new Vector3( _h2.localPosition.x, PingPong( Time.time * 60, _h2IniPosY, 0 ), _h2.localPosition.z );
            _h3.localPosition = new Vector3( _h3.localPosition.x, PingPong( Time.time * 60, _h3IniPosY, 0 ), _h3.localPosition.z );
        }
    }

    float PingPong( float time, float minLenght, float maxLenght )
    {
        return Mathf.PingPong( time, maxLenght - minLenght ) + minLenght;
    }
}
