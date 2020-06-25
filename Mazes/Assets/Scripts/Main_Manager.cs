using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Main_Manager : MonoBehaviour
{
    //hier maken wij onze manager static, en kunnen we door middel van bijvoorbeeld Manager.Instance.TilePrefab de tile callen
    public static Main_Manager Instance { get; private set; }

    //dit is de tile prefab, die voegen wij hier één keer toe en dan kunnen we een grid maken met meerdere tiles
    public Tile tile;
    //een reference naar mijn camera, zodat ik de size kan veranderen aan de hand van de maze size
    public Camera mainCamera;

    //hier zet ik de height en width met een default waarde, zodat de user zonder iets in te typen eigenlijk al een maze kan generaten
    private int height = 6, width = 6;
    private Algorithms currentAlgorithm;

    //dit is mijn grid die bestaat uit meerdere tiles, dit wordt een 2d array
    public Tile[,] grid;

    //snelheid van de maze generator, dit wordt gebruikt in Algorithms.cs in een coroutine
    [HideInInspector]
    public float animationSpeed = 0.1f;

    //als extratje heb ik een character gemaakt waarmee je kunt lopen in je maze, dit instantiate ik op het beginpunt van mijn algorithm
    public GameObject walkablePlayer;

    //bij iedere instantiate voeg ik mijn player toe in mijn playerCollection array, zodat ik bij het regeneraten mijn players weer kan verwijderen
    //normaal hoef je geen array te hebben hiervoor, maar aangezien mijn fun mode meerdere players instantiate moet ik ze in een array stoppen om erover te loopen tijdens het destroyen
    private List<GameObject> playerCollection;

    //de slider op mijn UI set dit
    public void SetAnimationSpeed(float value)
    {
        animationSpeed = value;
    }

    //dit zet de height variable
    public void SetHeight(TMP_InputField number)
    {
        //ik check of de user wel een integer heeft ingevuld, zoniet dan default ik het naar 6
        height = int.TryParse(number.text, out height) ? height : 6;
    }
    //dit zet de width variable
    public void SetWidth(TMP_InputField number)
    {
        width = int.TryParse(number.text, out width) ? width : 6;
    }
    //dit wordt geset door de dropdown menu
    public void SetAlgorithm(Algorithms option)
    {
        currentAlgorithm = option;
    }

    public void RegenerateMaze()
    {   //voordat de maze begint een extra check of er werkelijk logische getallen zijn ingevuld voor height en width
        if (width < 1 || height < 1)
        {
            UI_Manager.Instance.errorText.text = "Vul een getal groter dan 0 in voor Height en Width!";
        }
        else
        {
            //nu verwijder ik alle huidige players
            DeleteAllPlayers();
            UI_Manager.Instance.errorText.text = " ";
            //zijn er bestaande mazes die nog traversen? dan stop ik die
            StopAllCoroutines();
            //de vorige grid verwijder ik
            ClearGrid();
            //ik maak een nieuwe grid
            CreateGrid();
            //nu laten we mijn gekozen algorithm zijn gang gaan
            currentAlgorithm.GenerateMaze(this, width, height);
            
        }
        
    }

    public void InstantiateCharacter(int xPos, int yPos)
    {//z positie wordt -0.1f zodat mijn character niet achter mijn grid spawnt
        //hier voeg ik mijn character(s) toe in een list zodat ik het makkelijk kan deleten door te loopen bij iedere regeneration
        if (UI_Manager.Instance.funMode == false)
        {
            GameObject playerObject = Instantiate(walkablePlayer, new Vector3(xPos, yPos, -0.1f), Quaternion.identity);
            playerCollection.Add(playerObject);
        }
        else
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject playerObject = Instantiate(walkablePlayer, new Vector3(xPos, yPos, -0.1f), Quaternion.identity);
                playerCollection.Add(playerObject);
            }
        }
    }

    public void ClearGrid()
    {
        //hiermee destroy ik de gehele maze tijdens regeneration
        if (grid.Length > 0)
        {
            foreach (var item in grid)
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void CreateGrid()
    {
        //hier maak ik mijn 2d array, de 2d array zal als rijen en kolommen de height en de width hebben
        grid = new Tile[height, width];

        /*nu komt er een nested for loop, dit omdat mijn grid natuurlijk 2D is
        *voor iedere rij komt er een kolom.
        * Dit heeft een time complexity van O(n^2)
        */
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                /*hier instantiate ik nu individuele tiles, het is belangrijk dat de posities ook incrementeel van elkaar zullen liggen
                 *aan de hand van de i en j weet ik op welke positie ik zit van mijn loop, dus dan kan ik daarmee de positie
                 * van mijn tile tijdens het instantiaten bepalen
                 */
                //ik noem mijn instantiated tile ''cel'' en niet nogmaals ''tile''
                //de positie van mijn instantiate ligt aan de dimensions van mijn tile zelf, stel je veranderd de dimensions van je prefab later, dan kan deze loop alsnog werken
                //de rotatie moet 0 zijn, Instantiate werkt door Quaternions, dus vul je de identiteitsmatrix in (linear algebra)
                Tile cel = Instantiate(tile.gameObject, new Vector2(j * tile.transform.localScale.x, i * -tile.transform.localScale.y), Quaternion.identity).GetComponent<Tile>();
                //ik vul meteen de coordinaten van de cel in in zijn eigen Tile object, dit heb ik nodig voor Prism algorithm
                cel.SetXCoordinate(i);
                cel.SetYCoordinate(j);
                /*wat meer uitleg: mijn grid is 2d, dus bestaat dit uit een X en Y as.
                 * dus je ziet in mijn x parameter van mijn 2d Vector (hierboven in mijn instantiate) met iedere iteratie een X van= j * de x scale van tile opschuiven
                 * daarna op de volgende row schuift het een stapje omlaag, vandaar de ''-'' in mijn y parameter
                 */
                //met iedere iteratie moet ik wel mijn cel in zijn respectievelijke locatie stoppen:
                grid[i, j] = cel;
            
            }
        }

        ResizeCamera();
    }

    public void ResizeCamera()
    {
        //het eerste stap is mijn camera centeren op de grid, de tweede stap is de camera te resizen aan de hand van de grid

        //ik weet wat de width en height is van mijn maze, mijn 2d camera zal dan moeten focussen op het exacte middelpunt van mijn grid
        //dus dan doe ik simpelweg width/2 voor de x as en height/2 voor de y as

        //in unity kan ik niet direct de x en y as van een transform Setten, dus moet ik het een nieuwe Vector3 voeden
        //width en height doe ik -1 want de coordinaten gaan van 0-5 en niet 1-6
        mainCamera.transform.position = new Vector3((width-1) / 2.0f, (height-1) / -2.0f, -10f); //Z-as is -10f anders clipt de camera door mijn grid heen

        /*nu resize ik mijn camera
        *in Unity is orthographicsize maar 1 float getal, en dat staat proportioneel aan de helft van de verticale lengte.
        *ik kan dan heel simpel mijn orthographic size setten door (height/2.0f) + muurdikte te doen, maar dan houdt het geen rekening met de width
        *om wel rekening te houden met de width kunnen wij rekenen met de aspectratio van de camera (16:9)
        * dus nu komt een simpele trucje, ik check of het ratio van mijn object die ik moet waarnemen hoger of lager is dan mijn aspectratio
        * aan de hand daarvan kan ik bepalen of mijn width of height van mijn object leidend is.
        * ik tel ook de dikte van mijn muur op want dat steekt net uit de tile, zo fit nu perfect mijn camera.
        */
        if (width / height < mainCamera.aspect)
        {
            mainCamera.orthographicSize = ((height + tile.northWall.transform.localScale.x) / 2.0f);
        }
        else
        {
            mainCamera.orthographicSize = ((width + tile.eastWall.transform.localScale.x) / mainCamera.aspect / 2.0f);
        }
    }

    private void DeleteAllPlayers()
    {//de opgevulde list met players wordt hier verwijderd
        for (int i = 0; i < playerCollection.Count; i++)
        {
            if (playerCollection[i])
            {
                Destroy(playerCollection[i]);
            }
        }
    }

    private void Awake()
    {   //hierdoor kan ik alle public fields makkelijk callen vanuit andere scripts door Main_Manager.Instance. te doen
        Instance = this;
    }


    void Start()
    {//ik create alvast mijn default grid zodra mijn programma start
        CreateGrid();
        playerCollection = new List<GameObject>();
    }

}
