using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public Slider populationProgress;
    public Text timeDisplay;
    public Text carrotCounter;
    public Transform panelBunnyInfo;
    public Sprite femaleSymbol;
    public Sprite maleSymbol;
    //
    public float timeDeathMsgOnScreen = 5f;
    public List<Text> deathMessages;
    //
    public Canvas gameUI;
    public Canvas pauseCanvas;
    public Canvas gameOverCanvas;
    //
    private Text _lblBunnyName;
    private Text _lblBunnyAge;
    private Image _imgBunnyGender;
    private Text _lblBunnyCarrot;
    //
    private float [] _deathMsgsCounter;
    //
    private GameObject lblPressA;
    private float blinkAcum;


    private GameManager _gameManager;

    void Awake()
    {
        _gameManager = (GameManager)FindObjectOfType( typeof( GameManager ) );
        SetupBunnyInfoPanel();
        _deathMsgsCounter = new float[deathMessages.Count];
        gameOverCanvas.gameObject.SetActive( false );
    }

    // Use this for initialization
    void Start()
    {
        _gameManager.timeManager.OnTimeChange += UpdateTimeDisplay;
        _gameManager.bunniesManager.OnChangeBunnyList += UpdatePopulationProgress;
        _gameManager.OnChangeBunny += UpdateBunnyInfo;
        _gameManager.OnUpdateCarrot += UpdateCarrotCounter;

        foreach ( Text deathMsg in deathMessages )
        {
            deathMsg.gameObject.SetActive( false );
        }

    }

    // Update is called once per frame
    void Update()
    {
        if ( _gameManager.gameOver )
        {
            blinkAcum += Time.deltaTime;
            Debug.Log( "Game Over " + blinkAcum );
            if ( blinkAcum >= .3f )
            {
                blinkAcum = 0;
                lblPressA.SetActive( !lblPressA.activeSelf );
            }
        } else
        {
            for ( int ideathMsg = 0; ideathMsg < deathMessages.Count; ideathMsg++ )
            {
                if ( deathMessages[ideathMsg].gameObject.activeSelf )
                {
                    _deathMsgsCounter[ideathMsg] += Time.deltaTime;
                    if ( _deathMsgsCounter[ideathMsg] >= timeDeathMsgOnScreen )
                    {
                        deathMessages[ideathMsg].gameObject.SetActive( false );
                    }
                }
            }
        }
    }

    public void GameOver( string gameOverMsg, string gameOverResult )
    {
        gameUI.gameObject.SetActive( false );
        gameOverCanvas.gameObject.SetActive( true );
        GameObject panel = gameOverCanvas.transform.FindChild( "Panel" ).gameObject;
        panel.transform.FindChild( "lblFinalGame" ).GetComponent<Text>().text = gameOverMsg;
        panel.transform.FindChild( "lblFinalStatus" ).GetComponent<Text>().text = gameOverResult;
        lblPressA = panel.transform.FindChild( "lblpressA" ).gameObject;
        blinkAcum = 0;
    }

    public void Pause( bool paused )
    {
        if ( paused )
        {
            gameUI.gameObject.SetActive( false );
            pauseCanvas.gameObject.SetActive( true );
        } else
        {
            gameUI.gameObject.SetActive( true );
            pauseCanvas.gameObject.SetActive( false );
        }
    }

    void UpdateCarrotCounter()
    {
        carrotCounter.text = _gameManager.amountCarrots.ToString();
    }

    void UpdateTimeDisplay()
    {
        timeDisplay.text = String.Format( "{0} Months {1} Years", _gameManager.timeManager.monthsAcum, _gameManager.timeManager.yearsAcums );
    }

    void UpdatePopulationProgress()
    {
        populationProgress.value = ( ( _gameManager.bunniesManager.Population * populationProgress.maxValue ) / _gameManager.maxPopulation );
    }

    void SetupBunnyInfoPanel()
    {
        if ( panelBunnyInfo != null )
        {
            _lblBunnyName = panelBunnyInfo.FindChild( "lblBunnyName" ).GetComponent<Text>();
            _lblBunnyAge = panelBunnyInfo.FindChild( "lblBunnyAge" ).GetComponent<Text>();
            _imgBunnyGender = panelBunnyInfo.FindChild( "imgGender" ).GetComponent<Image>();
            _lblBunnyCarrot = panelBunnyInfo.FindChild( "lblBunnyCarrot" ).GetComponent<Text>();

            _lblBunnyName.text = "";
            _lblBunnyAge.text = "";
            _lblBunnyCarrot.text = "";
            _imgBunnyGender.sprite = null;
        }

    }

    void UpdateBunnyInfo()
    {
        _lblBunnyName.text = _gameManager.SelectedBunny.bunnyName;
        _lblBunnyAge.text = _gameManager.SelectedBunny.bunnyAge.ToString();
        _lblBunnyCarrot.text = Mathf.RoundToInt( _gameManager.SelectedBunny.currentCarrot ).ToString();
        _imgBunnyGender.sprite = _gameManager.SelectedBunny.bunnyGender.Equals( Bunny.Gender.Female ) ? femaleSymbol : maleSymbol;
    }

    public void AddDeathMsg( string msg )
    {
        for ( int ideathmsg = 0; ideathmsg < deathMessages.Count; ideathmsg++ )
        {
            if ( !deathMessages[ideathmsg].gameObject.activeSelf )
            {
                deathMessages[ideathmsg].text = msg;
                deathMessages[ideathmsg].gameObject.SetActive( true );
                _deathMsgsCounter[ideathmsg] = 0f;
                break;
            }
        }
    }
}
