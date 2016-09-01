using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bunny : MonoBehaviour
{
    public enum Gender
    {
        Male,
        Female
    }

    ;

    public enum State
    {
        Idle,
        InLove,
        GoingFeed,
        InFeed
    }

    ;

    public string bunnyName;
    public Gender bunnyGender;
    public int bunnyAge;
    public int maxAge;
    public float maxCarrot;
    public float currentCarrot;
    public State bunnyState;

    private float _timeInLove;
    private float _acumTimeInLove;

    private float _timeIdler;
    private float _acumTimeIdler;

    static Vector3 _minBoundsPoint;
    static Vector3 _maxBoundsPoint;
    static float _boundsSize = float.NegativeInfinity;

    private NavMeshAgent _navAgent;
    private GameManager _gameManager;
    private IndicatorAnimation _indicators;
    private SpriteRenderer _sprite;


    public Bunny( string _name, Gender _gender, int _age, int _maxAge )
    {
        this.name = _name;
        this.bunnyGender = _gender;
        this.bunnyAge = _age;
        this.maxAge = _maxAge;
    }

    void Awake()
    {
        _gameManager = (GameManager)FindObjectOfType( typeof( GameManager ) );
        _indicators = transform.FindChild( "GameIndicators" ).GetComponent<IndicatorAnimation>();
        _navAgent = GetComponent<NavMeshAgent>();
        _gameManager.timeManager.OnNewYear += CheckAge;
        _gameManager.timeManager.OnNewMonth += DeplishCarrot;

        _sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _navAgent.updateRotation = false;
        _navAgent.speed = Random.Range( 5f, 7f );
        _navAgent.avoidancePriority = Random.Range( 0, 100 );

        ChangeState( State.Idle );
        _acumTimeIdler = Mathf.Infinity;
        _timeInLove = Random.Range( 6f, 10f );
        _timeIdler = Random.Range( 3f, 6f );
        currentCarrot = 0;
    }

    void Update()
    {
        CheckSpriteDir();

        switch ( bunnyState )
        {
            case State.Idle:
                _acumTimeIdler += Time.deltaTime;
                if ( _acumTimeIdler >= _timeIdler )
                {
                    _acumTimeIdler = 0;
                    MoveRandom();
                }
                break;

            case State.InLove:
                _acumTimeInLove += Time.deltaTime;
                if ( _acumTimeInLove >= _timeInLove )
                {
                    ChangeState( State.Idle );
                }
                break;

            case State.GoingFeed:
                if ( _gameManager.InsideFeeder( transform.position ) )
                {
                    ChangeState( State.InFeed );
                }
                break;

            case State.InFeed:
                break;

            default:
                break;
        }
    }

    void MoveTo( Vector3 pos )
    {
        _navAgent.ResetPath();
        _navAgent.SetDestination( pos );
        _navAgent.Resume();
    }

    void CheckSpriteDir()
    {
        float dir = _navAgent.destination.x - transform.position.x;
        if ( dir > 0 && !_sprite.flipX )
        {
            _sprite.flipX = !_sprite.flipX;
        } else if ( dir < 0 && _sprite.flipX )
        {
            _sprite.flipX = !_sprite.flipX;
        }
    }

    bool IsMoving()
    {
        bool returnValue = true;
        if ( !_navAgent.pathPending )
        {
            if ( _navAgent.remainingDistance <= _navAgent.stoppingDistance )
            {
                if ( !_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f )
                {
                    returnValue = false;
                }
            }
        }

        return returnValue;
    }

    void MoveRandom()
    {
        if ( !IsMoving() )
        {
            Vector3 newPos = GetRandomTargetPoint();
            while ( _gameManager.InsideFeeder( newPos, 5 ) )
            {
                newPos = GetRandomTargetPoint();
            }
            MoveTo( newPos );
        }
    }

    public void OnSelect( bool enabled )
    {
        _indicators.ShowArrow( enabled );
    }

    public void InLove()
    {
        currentCarrot += 1f;
        if ( bunnyState.Equals( State.Idle ) )
        {
            _gameManager.bunniesManager.AddBunnyInLove( this );
            _indicators.ShowHearts( enabled );
            ChangeState( State.InLove );
        }
    }

    public void MoveToFeed()
    {
        ChangeState( State.GoingFeed );
    }

    public void GoIdle()
    {
        ChangeState( State.Idle );
        _acumTimeIdler = Mathf.Infinity;
    }

    void DeplishCarrot()
    {
        if ( currentCarrot > 0 )
        {
            currentCarrot -= .1f;
            _gameManager.UpdateBunnyInfo();
        }
    }

    void CheckAge()
    {
        bunnyAge++;
        if ( bunnyAge > maxAge )
        {
            Die();
        }
    }

    public void Die( bool explosion = false )
    {
        if ( explosion )
        {
            Destroy( Instantiate( _gameManager.BunnyExplosionEffect, transform.position, _gameManager.BunnyExplosionEffect.rotation
            ) as GameObject
                , _gameManager.BunnyExplosionEffect.GetComponent<ParticleSystem>().startLifetime );
            _gameManager.audioManager.PlayExplosion();
        }
        _gameManager.bunniesManager.RemoveBunny( this, explosion );
    }

    void ChangeState( State newState )
    {
        switch ( newState )
        {
            case State.Idle:
                _acumTimeIdler = 0;
                _indicators.ShowHearts( false );
                if ( bunnyState.Equals( State.InFeed ) && currentCarrot >= maxCarrot )
                {

                    Die( true );
                }
                bunnyState = newState;
                break;

            case State.InLove:
                _acumTimeInLove = 0;
                bunnyState = newState;
                break;

            case State.GoingFeed:
                MoveTo( _gameManager.feedSpot.position );
                bunnyState = newState;
                break;

            case State.InFeed:
                _indicators.ShowHearts( true );
                bunnyState = newState;
                break;

            default:
                break;
        }

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label( transform.position, ( bunnyGender.Equals( Gender.Male ) ? "(M)" : "(F)" ) + bunnyState.ToString() );
    }
#endif

    private Vector3 GetRandomTargetPoint()
    {
        if ( _boundsSize < 0 )
        {
            _minBoundsPoint = Vector3.one * float.PositiveInfinity;
            _maxBoundsPoint = -_minBoundsPoint;
            var vertices = NavMesh.CalculateTriangulation().vertices;
            foreach ( var point in vertices )
            {
                if ( _minBoundsPoint.x > point.x )
                    _minBoundsPoint = new Vector3( point.x, _minBoundsPoint.y, _minBoundsPoint.z );
                if ( _minBoundsPoint.y > point.y )
                    _minBoundsPoint = new Vector3( _minBoundsPoint.x, point.y, _minBoundsPoint.z );
                if ( _minBoundsPoint.z > point.z )
                    _minBoundsPoint = new Vector3( _minBoundsPoint.x, _minBoundsPoint.y, point.z );
                if ( _maxBoundsPoint.x < point.x )
                    _maxBoundsPoint = new Vector3( point.x, _maxBoundsPoint.y, _maxBoundsPoint.z );
                if ( _maxBoundsPoint.y < point.y )
                    _maxBoundsPoint = new Vector3( _maxBoundsPoint.x, point.y, _maxBoundsPoint.z );
                if ( _maxBoundsPoint.z < point.z )
                    _maxBoundsPoint = new Vector3( _maxBoundsPoint.x, _maxBoundsPoint.y, point.z );
            }
            _boundsSize = Vector3.Distance( _minBoundsPoint, _maxBoundsPoint );
        }
        Vector3 randomPoint = new Vector3(
                                  Random.Range( _minBoundsPoint.x, _maxBoundsPoint.x ),
                                  Random.Range( _minBoundsPoint.y, _maxBoundsPoint.y ),
                                  Random.Range( _minBoundsPoint.z, _maxBoundsPoint.z )
                              );
        NavMeshHit hit;
        NavMesh.SamplePosition( randomPoint, out hit, _boundsSize, 1 );
        return hit.position;
    }
}
