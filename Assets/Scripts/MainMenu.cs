using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Canvas splashCanvas;
    public Canvas instructionsCavas;
    public Text lvlPressAMainMenu;
    public Text lvlPressAInstructions;
    //
    float blinkAcum;
    bool inInstructions;

    // Use this for initialization
    void Start()
    {
        blinkAcum = 0;
        inInstructions = false;
        instructionsCavas.gameObject.SetActive( false );
        splashCanvas.gameObject.SetActive( true );
    }
	
    // Update is called once per frame
    void Update()
    {

        blinkAcum += Time.deltaTime;
        if ( blinkAcum >= .3f )
        {
            blinkAcum = 0;
            if ( !inInstructions )
            { 
                lvlPressAMainMenu.gameObject.SetActive( !lvlPressAMainMenu.IsActive() );
            }
            else
            { 
                lvlPressAInstructions.gameObject.SetActive( !lvlPressAInstructions.IsActive() );
            }
        }

        if ( Input.GetKeyDown( KeyCode.A ) )
        {
            if ( inInstructions )
            {
                SceneManager.LoadScene( 1 );
            }
            else
            {
                LoadInstructions();
            }
        }
    }

    void LoadInstructions()
    {
        instructionsCavas.gameObject.SetActive( true );
        splashCanvas.gameObject.SetActive( false );
        inInstructions = true;
    }
}
