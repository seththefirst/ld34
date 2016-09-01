using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BunniesManager : MonoBehaviour
{
    public event System.Action OnChangeBunnyList;

    public GameObject bunnyPrefab;
    public Transform bunnyExplosionEffect;
    public int distanceToFallInlove = 3;
    public float timeInFeed = 6f;
    public float timeFemaleInFeed = 10f;
    public List<Sprite> maleSprites;
    public List<Sprite> femaleSprites;
    //
    public List<Bunny> bunnies;
    public List<Couple> couples;

    //
    private int _bunniesToBorn;
    private int _selectedBunnyIndex;
    private Queue<string> _femaleNames;
    private Queue<string> _maleNames;
    private Queue<int> _couplesDone;
    public List<Bunny> _bunniesInLove;
    //
    private GameManager _gameManager;

    //
    public int Population
    {
        get { return bunnies.Count; }
    }

    public bool HasMales
    {
        get
        {
            return bunnies.Exists( i => i.bunnyGender.Equals( Bunny.Gender.Male ) );
        }
    }

    public bool HasFemales
    {
        get
        {
            return bunnies.Exists( i => i.bunnyGender.Equals( Bunny.Gender.Female ) );
        }
    }

    public Bunny SelectedBunny
    {
        get
        {
            if ( _selectedBunnyIndex < 0 || _selectedBunnyIndex >= bunnies.Count )
            {
                _selectedBunnyIndex = 0;
            }
            return bunnies[_selectedBunnyIndex];
        }
    }

    void Awake()
    {
        _gameManager = (GameManager)FindObjectOfType( typeof( GameManager ) );
        bunnies = new List<Bunny>();
        couples = new List<Couple>();
        _bunniesInLove = new List<Bunny>();
        _couplesDone = new Queue<int>();
        _selectedBunnyIndex = -1;
        SetupNames();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distance;

        // Check for bunnies in love, and create couples if they are close enough
        _bunniesInLove.RemoveAll( i => !i.bunnyState.Equals( Bunny.State.InLove ) );
        if ( _bunniesInLove.Count > 1 )
        {
            Bunny bunny1 = _bunniesInLove[Random.Range( 0, _bunniesInLove.Count - 1 )];
            for ( int i = 0; i < _bunniesInLove.Count; i++ )
            {
                if ( _bunniesInLove[i] != bunny1 )
                {
                    distance = Vector3.Distance( bunny1.transform.position, _bunniesInLove[i].transform.position );
                    if ( distance <= distanceToFallInlove && bunny1.bunnyGender != _bunniesInLove[i].bunnyGender )
                    {
                        if ( !couples.Exists( ibunny => ibunny.bunny1 == bunny1 || ibunny.bunny2 == bunny1 ) )
                        {
                            CreateCouple( bunny1, _bunniesInLove[i] );
                        }
                        break;
                    }
                }
            }
        }

        //Check for couples to mate
        for ( int icouple = 0; icouple < couples.Count; icouple++ )
        {
            Couple couple = couples[icouple];

            if ( couple.AnyIdle() && !couple.doneFeed )
            {
                couple.SendToFeed();
            }

            if ( couple.AllInFeed() || couple.doneFeed )
            {
                if ( !couple.doneFeed )
                {
                    couple.timeInFeed += Time.deltaTime;
                    if ( couple.timeInFeed >= timeInFeed )
                    {
                        couple.doneFeed = true;
                        couple.maleChance = couple.GetMediumCarrotValue();
                        couple.Male.GoIdle();
                    }
                }

                couple.timeFemaleInFeed += Time.deltaTime;
                if ( couple.timeFemaleInFeed >= timeFemaleInFeed )
                {
                    _bunniesToBorn = Random.Range( 2, 5 );
                    for ( int ibunny = 0; ibunny < _bunniesToBorn; ibunny++ )
                    {
                        AddBunny( _gameManager.feedSpot.position, 1, couple.maleChance );
                    }
                    couple.Female.GoIdle();
                    _couplesDone.Enqueue( icouple );
                }
            }
        }

        while ( _couplesDone.Count > 0 )
        {
            couples.RemoveAt( _couplesDone.Dequeue() );
        }
    }

    public void RemoveBunny( Bunny bunny, bool explosion )
    {
        bunnies.Remove( bunny );
        _gameManager.SendDeathMsg( "Bunny " + bunny.bunnyName + " died - " + ( explosion ? "overdose" : "too old." ) );
        if ( _gameManager.SelectedBunny == bunny )
        {
            _gameManager.SelectNextBunny();
        }
        Destroy( bunny.gameObject );

        if ( OnChangeBunnyList != null )
        {
            OnChangeBunnyList();
        }
    }


    public Bunny AddBunny( Vector3 spawnPoint, int age = 0, int maleChance = 0 )
    {
        return AddBunny( spawnPoint, age, Random.Range( (int)0 + maleChance, (int)( 10 ) ) > 5f ? Bunny.Gender.Male : Bunny.Gender.Female );
    }

    public Bunny AddBunny( Vector3 spawnPoint, int age, Bunny.Gender gender )
    {
        GameObject goNewBunny;
        Bunny newBunny;

        goNewBunny = Instantiate( bunnyPrefab, spawnPoint, bunnyPrefab.transform.localRotation ) as GameObject;
        newBunny = goNewBunny.AddComponent<Bunny>();

        newBunny.bunnyGender = gender;
        newBunny.bunnyName = RandomBunnyName( newBunny.bunnyGender );
        newBunny.name = newBunny.bunnyName;
        newBunny.bunnyAge = age;
        newBunny.maxAge = Random.Range( 2, 4 );
        newBunny.maxCarrot = Random.Range( 5f, 6f );

        newBunny.GetComponent<SpriteRenderer>().sprite = GetRandomSprite( gender );
        bunnies.Add( newBunny );
        if ( OnChangeBunnyList != null )
        {
            OnChangeBunnyList();
        }
        return newBunny;
    }

    public Sprite GetRandomSprite( Bunny.Gender gender )
    {
        Sprite returnSprite = null;
        switch ( gender )
        {
            case Bunny.Gender.Female:
                returnSprite = femaleSprites[Random.Range( 0, femaleSprites.Count )];
                break;
            case Bunny.Gender.Male:
                returnSprite = maleSprites[Random.Range( 0, maleSprites.Count )];
                break;
        }
        return returnSprite;
    }

    public string RandomBunnyName( Bunny.Gender gender )
    {
        string returnName = "";
        switch ( gender )
        {
            case Bunny.Gender.Female:
                returnName = _femaleNames.Dequeue();
                _femaleNames.Enqueue( returnName );
                break;

            case Bunny.Gender.Male:
                returnName = _maleNames.Dequeue();
                _maleNames.Enqueue( returnName );
                break;
        }
        return returnName;
    }

    public void AddBunnyInLove( Bunny newBunny )
    {
        if ( !_bunniesInLove.Contains( newBunny ) )
        {
            _bunniesInLove.Add( newBunny );
        }
    }

    public void SelectNextBunny()
    {
        if ( _selectedBunnyIndex >= 0 )
        {
            bunnies[_selectedBunnyIndex].OnSelect( false );
        }
        _selectedBunnyIndex++;
        if ( _selectedBunnyIndex > bunnies.Count - 1 )
        {
            _selectedBunnyIndex = 0;
        }
        bunnies[_selectedBunnyIndex].OnSelect( true );
    }

    public void CreateCouple( Bunny bunny1, Bunny bunny2 )
    {
        Couple newCouple = new Couple( bunny1, bunny2 );
        bunny1.GoIdle();
        bunny2.GoIdle();
        couples.Add( newCouple );
    }

    void SetupNames()
    {
        _femaleNames = new Queue<string>();
        _maleNames = new Queue<string>();

        // Male Names
        _maleNames.Enqueue( "James" );
        _maleNames.Enqueue( "John" );
        _maleNames.Enqueue( "Robert" );
        _maleNames.Enqueue( "Michael" );
        _maleNames.Enqueue( "William" );
        _maleNames.Enqueue( "david" );
        _maleNames.Enqueue( "Richard" );
        _maleNames.Enqueue( "Charles" );
        _maleNames.Enqueue( "Joseph" );
        _maleNames.Enqueue( "Thomas" );
        _maleNames.Enqueue( "Christopher" );
        _maleNames.Enqueue( "Daniel" );
        _maleNames.Enqueue( "Paul" );
        _maleNames.Enqueue( "Mark" );
        _maleNames.Enqueue( "Donald" );
        _maleNames.Enqueue( "George" );
        _maleNames.Enqueue( "Kenneth" );
        _maleNames.Enqueue( "Steven" );
        _maleNames.Enqueue( "Edward" );
        _maleNames.Enqueue( "Brian" );
        _maleNames.Enqueue( "Ronald" );
        _maleNames.Enqueue( "Anthony" );
        _maleNames.Enqueue( "Kevin" );
        _maleNames.Enqueue( "Jason" );
        _maleNames.Enqueue( "Jeff" );

        // Female Names
        _femaleNames.Enqueue( "Mary" );
        _femaleNames.Enqueue( "Patricia" );
        _femaleNames.Enqueue( "Linda" );
        _femaleNames.Enqueue( "Barbara" );
        _femaleNames.Enqueue( "Elizabeth" );
        _femaleNames.Enqueue( "Jennifer" );
        _femaleNames.Enqueue( "Maria" );
        _femaleNames.Enqueue( "Susan" );
        _femaleNames.Enqueue( "Margaret" );
        _femaleNames.Enqueue( "Dorothy" );
        _femaleNames.Enqueue( "Lisa" );
        _femaleNames.Enqueue( "Nancy" );
        _femaleNames.Enqueue( "Karen" );
        _femaleNames.Enqueue( "Betty" );
        _femaleNames.Enqueue( "Helen" );
        _femaleNames.Enqueue( "Sandra" );
        _femaleNames.Enqueue( "Donna" );
        _femaleNames.Enqueue( "Carol" );
        _femaleNames.Enqueue( "Ruth" );
        _femaleNames.Enqueue( "Sharon" );
        _femaleNames.Enqueue( "Michelle" );
        _femaleNames.Enqueue( "Laura" );
        _femaleNames.Enqueue( "Sarah" );
        _femaleNames.Enqueue( "Kimberly" );
        _femaleNames.Enqueue( "Deborah" );
    }


}
