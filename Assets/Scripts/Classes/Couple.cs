using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Couple
{
    public Bunny bunny1;
    public Bunny bunny2;
    public float timeInFeed;
    public float timeFemaleInFeed;
    public bool doneFeed;
    public int maleChance;

    public Bunny Female
    {
        get { return bunny1.bunnyGender.Equals( Bunny.Gender.Female ) ? bunny1 : bunny2; }
    }

    public Bunny Male
    {
        get { return bunny1.bunnyGender.Equals( Bunny.Gender.Male ) ? bunny1 : bunny2; }
    }

    public Couple( Bunny _bunny1, Bunny _bunny2 )
    {
        this.bunny1 = _bunny1;
        this.bunny2 = _bunny2;
        this.timeInFeed = 0;
        this.timeFemaleInFeed = 0;
        this.doneFeed = false;
    }

    public bool AnyIdle()
    {
        return ( bunny1.bunnyState.Equals( Bunny.State.Idle ) ||
        bunny2.bunnyState.Equals( Bunny.State.Idle ) );
    }

    public void SendToFeed()
    {
        if ( bunny1.bunnyState.Equals( Bunny.State.Idle ) )
        {
            bunny1.MoveToFeed();
        }

        if ( bunny2.bunnyState.Equals( Bunny.State.Idle ) )
        {
            bunny2.MoveToFeed();
        }
    }

    public bool AllInFeed()
    {
        return ( bunny1.bunnyState.Equals( Bunny.State.InFeed ) &&
        bunny2.bunnyState.Equals( Bunny.State.InFeed ) );
    }

    public int GetMediumCarrotValue()
    {
        return Mathf.RoundToInt( ( bunny1.currentCarrot + bunny2.currentCarrot ) / 2 );
    }
}
