using UnityEngine;

public class Tile : MonoBehaviour
{
    //als de tile zijn eigen coordinate weet in zichzelf, dan maakt dit bepaalde algorithmes makkelijker
    [HideInInspector]
    public int xCoordinate { get; private set; }
    [HideInInspector]
    public int yCoordinate { get; private set; }

    //de 4 muren van een tile
    public GameObject northWall;
    public GameObject eastWall;
    public GameObject southWall;
    public GameObject westWall;

    //visitedColor is voor recursivebacktracker EN prims
    private Color visitedColor = Color.white;
    //markedcolor is alleen voor prims
    private Color markedColor = Color.green;
    //voor animatie doeleinden verander ik mijn geselecteerde tile tijdens het traversen naar geel
    private Color cursorColor = Color.yellow;


    //een simpele boolean waarmee ik de tile kan markeren als visited of niet visited
    //ik moet mijn boolean publicly kunnen accessen, maar niet direct kunnen setten, dus maak ik het public en schuil ik het van de inspector
    //ik maak het een private set, en alleen via mijn method SetVisited kan ik het op true zetten, het is nu daarom onmogelijk om het direct op false te kunnen zetten
    [HideInInspector]
    public bool visited { get; private set; }

    //zelfde logica als hierboven, marked gebruik ik voor Prim's algorithm
    [HideInInspector]
    public bool marked { get; private set; }
    
    public void SetVisited()
    {
        visited = true;
        GetComponent<SpriteRenderer>().color = visitedColor;
    }

    public void SetMarked()
    {
        marked = true;
        GetComponent<SpriteRenderer>().color = markedColor;

    }

    public void SetCursor()
    {
        GetComponent<SpriteRenderer>().color = cursorColor;
    }

    public void SetXCoordinate(int value)
    {
        xCoordinate = value;
    }
    public void SetYCoordinate(int value)
    {
        yCoordinate = value;
    }


}
