using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event System.Action OnChangeBunny;
    public event System.Action OnUpdateCarrot;

    public Transform feedSpot;
    public Transform feederBox;
    [Range( 10, 200 )]
    public int maxPopulation = 10;
    //
    public int amountCarrots;
    public bool paused;
    public bool gameOver;
    //
    public BunniesManager bunniesManager;
    public TimeManager timeManager;
    public AudioManager audioManager;
    //
    private Bounds _feederBounds;
    private UIManager _uiManager;
    //
    private string _finalMsg;
    //
    public Bunny SelectedBunny
    {
        get { return bunniesManager.SelectedBunny; }
    }

    public Transform BunnyExplosionEffect
    {
        get { return bunniesManager.bunnyExplosionEffect; }
    }

    void Awake()
    {
        timeManager = ( TimeManager )FindObjectOfType( typeof( TimeManager ) );
        bunniesManager = ( BunniesManager )FindObjectOfType( typeof( BunniesManager ) );
        audioManager = ( AudioManager )FindObjectOfType( typeof( AudioManager ) );
        _uiManager = ( UIManager )FindObjectOfType( typeof( UIManager ) );
    }

    // Use this for initialization
    void Start()
    {
        // timeManager.OnNewMonth += OnNewMonth;
        bunniesManager.AddBunny( feedSpot.position, 0, Bunny.Gender.Female );
        bunniesManager.AddBunny( feedSpot.position, 0, Bunny.Gender.Female );
        bunniesManager.AddBunny( feedSpot.position, 0, Bunny.Gender.Female );
        bunniesManager.AddBunny( feedSpot.position, 0, Bunny.Gender.Male );
        bunniesManager.AddBunny( feedSpot.position, 0, Bunny.Gender.Male );

        timeManager.OnNewYear += OnNewYear;

        SelectNextBunny();

        UpdateCarrot( 10 );

        _feederBounds = feederBox.GetComponent<BoxCollider>().bounds;

        UnPause();

        gameOver = false;
    }

    public void SendDeathMsg( string msg )
    {
        _uiManager.AddDeathMsg( msg );
    }

    void UpdateCarrot( int amount )
    {
        amountCarrots += amount;
        if ( OnUpdateCarrot != null )
        {
            OnUpdateCarrot();
        }
    }

    public void UpdateBunnyInfo()
    {
        if ( OnChangeBunny != null )
        {
            OnChangeBunny();
        }
    }

    public void GiveCarrot( Bunny bunny )
    {
        if ( amountCarrots > 0 )
        {
            UpdateCarrot( -1 );
            bunny.InLove();
            if ( OnChangeBunny != null )
            {
                OnChangeBunny();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if ( !gameOver )
        {
            if ( ( timeManager.yearsAcums >= 5 && bunniesManager.Population < maxPopulation ) ||
                 !bunniesManager.HasMales ||
                 !bunniesManager.HasFemales )
            {
                _finalMsg = "";
                _finalMsg += timeManager.yearsAcums + " years \n";
                _finalMsg += bunniesManager.Population + " bunnies \n";
                if ( !bunniesManager.HasMales )
                {
                    _finalMsg += "No more males\n";
                }
                if ( !bunniesManager.HasFemales )
                {
                    _finalMsg += "No more females";
                }
                gameOver = true;
                _uiManager.GameOver( "GAME OVER", _finalMsg );
            }

            if ( bunniesManager.Population >= maxPopulation )
            {
                _finalMsg = "";
                _finalMsg += timeManager.yearsAcums + " years \n";
                _finalMsg += bunniesManager.Population + " bunnies \n";
                gameOver = true;
                _uiManager.GameOver( "CONGRATULATIONS", _finalMsg );
            }
        }
    }

    void OnNewYear()
    {
        UpdateCarrot( 3 );
    }

    public void SelectNextBunny()
    {
        if ( bunniesManager.bunnies.Count > 0 )
        { 
            bunniesManager.SelectNextBunny();

            if ( OnChangeBunny != null )
            {
                OnChangeBunny();
            }
        }
    }

    public bool InsideFeeder( Vector3 pos, float extents = 0 )
    {
        bool returnValue = false;

        if ( pos.x <= ( _feederBounds.center.x + _feederBounds.extents.x + extents ) && pos.x >= ( _feederBounds.center.x - _feederBounds.extents.x - extents ) )
        {
            if ( pos.z <= ( _feederBounds.center.z + _feederBounds.extents.z + extents ) && pos.z >= ( _feederBounds.center.z - _feederBounds.extents.z - extents ) )
            {  
                returnValue = true;
            }
        }

        return returnValue;
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
        _uiManager.Pause( true );
    }

    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1f;
        _uiManager.Pause( false );
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene( 0 );
    }
}
 