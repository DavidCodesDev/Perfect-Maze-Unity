using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/*Om mijn project scalable te maken en dus makkelijk later nieuwe algorithmes toe te kunnen voegen, maak ik gebruik van 
 * slimme Object Oriented Programming (OOP) technieken, in dit geval Polymorphism, waarbij ik een abstract class maak
 * die iedere nieuwe algorithm zal moeten inheriten, abstract variables en methods forceren de subclasses om ze te implementeren.
 * Mijn project is hierdoor dus zo geprogrammeerd waardoor ik of een nieuwe user later gemakkelijk een nieuwe algorithme kan implementeren in deze Algorithms.cs file
 * Het enige wat je hoeft te doen is dan een nieuwe class te maken met jouw Algoritme en dat de class Algorithms te inheriten.
 * Daarna druk je rechtermuisklik op de naam van jouw nieuwe class -> quick actions -> implement abstract class
 * Ik behoud mijn MonoBehaviour in de base class zodat ik Coroutines kan gebruiken voor de maze animatie
 */

public abstract class Algorithms
{
    //GetAlgorithmName populate de dropdown menu in mijn UI
    public abstract String GetAlgorithmName();

    //dit is de algorithm logic die de grid zal veranderen naar een maze
    //Ik voed dit een MonoBehaviour zodat ik een coroutine kan callen in de child class
    public abstract void GenerateMaze(MonoBehaviour mono, int width, int height);

}


public class RecursiveBacktracker : Algorithms
{
    /*hier is de algorithm van de Recursive Backtracker
     * Bron: http://people.cs.ksu.edu/~ashley78/wiki.ashleycoleman.me/index.php/Recursive_Backtracker.html
     * Ik kopieër geen code, ik lees de pseudo code en aan de hand daarvan programmeer ik mijn algorithme
     */

    /*
    1.Close all cells
    2.Choose starting cell and open it. This is the current cell
    3.Pick a cell adjacent to the current cell that hasn’t been visited and open it. It becomes the current cell
    4.Repeat 2 until no adjacent wall can be selected
    5.The previous cell becomes the current cell. If this cell is the starting cell, then we are done. Else go to 2
    */



    public override void GenerateMaze(MonoBehaviour mono, int width, int height)
    {
        //Begin coordinaten.
        int i = 0, j = 0;

        /*ik maak een stack die ik kan vullen met iedere cel die ik gevisit heb, hitten wij een dead end? dan kan ik mijn laatst toegevoegde cel eruit
         * halen en weer verder gaan met traversen. Hiermee kan ik ook checken of mijn algorithm weer terug bij het begin is
         * Is het weer terug bij het begin? dan is mijn algorithm klaar.
         */
        Stack<int[]> allVisitedCells = new Stack<int[]>();
        allVisitedCells.Push(new int[] { i, j });

        //als hulpmiddel tijdens het traversen mark ik in deze script de visited cells, als alle 4 de booleans true aangeven,
        //dan weet ik dat mijn current cel omringd is door al visited cells, dus nu moet hij terug itereren
        bool northAlreadyVisited = false;
        bool eastAlreadyVisited = false;
        bool southAlreadyVisited = false;
        bool westAlreadyVisited = false;

        //''Close all cells'' wordt gedaan in mijn ClearGrid() method in de Main_Manager
        //Choose starting cell: Ik kies hierbij cel (0,0). Dus linksboven de grid.
        Main_Manager.Instance.grid[i, j].SetVisited();

        Main_Manager.Instance.InstantiateCharacter(i, j);

        //hier gebruik ik mijn mono (Monobehaviour) object zodat ik een StartCoroutine kan callen
        mono.StartCoroutine(GenerateMazeAnimation());
        
        IEnumerator GenerateMazeAnimation()
        {
            do
            {

                if (northAlreadyVisited && eastAlreadyVisited && southAlreadyVisited && westAlreadyVisited)
                {
                    if (allVisitedCells.Count == 0)
                    {
                        yield break;
                    }
                    else
                    {
                        //voor animatie purposes delay ik dit met 0.1 seconden, dit kun je variabel in je UI veranderen
                        Main_Manager.Instance.grid[i, j].SetCursor();

                        yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);
                        Main_Manager.Instance.grid[i, j].SetVisited();


                        ResetAllVisitedBooleans();
                        allVisitedCells.Pop();
                        if (allVisitedCells.Count > 0)
                        {
                            
                            i = allVisitedCells.Peek()[0];
                            j = allVisitedCells.Peek()[1];

                        }

                    }
                }
                //Ik kies nu een direction, ik heb 4 directions; noord,oost,zuid,west. Dus kies in een random getal tussen 0 (inclusief) en 4 (exclusief)
                int direction = UnityEngine.Random.Range(0, 4);
                switch (direction)
                {

                    /*Ik beschrijf hier eenmaal hoe mijn algorithme werkt, zodat ik dit niet copy paste bij iedere switch case:
                     * 
                     * bij iedere direction die de random number generator heeft gekozen check ik eerst of mijn momenteel geselecteerde cel
                     * aan één van de randen zit van mijn grid, stel de huidige cel is 0,0 maar hij wil naar het noorden, dan kan dat niet
                     * dus dan kiezen wij een nieuwe direction. Is de cel waar ik naartoe wil gaan al gevisit? dan Traversen wij opnieuw en zet ik *alreadyVisited op true
                     * 
                     * Wijst mijn direction niet naar een rand en is de aangewezen cel niet gevisit? Dan haal ik de respectievelijke muren weg,
                     * verkleur ik die cel binnen de tile.cs en wordt dat mijn nieuwe huidige cel
                     *Ook reset ik alle visitedbooleans in deze script want wij zijn nu in een nieuwe cel beland en weten we niks over de buren
                     * de belande cel zet ik in mijn stack, zodat als ik een dead end bereik dat ik de top van mijn stack kan weghalen en vervolgens
                     * verder itereren met de nieuwe top van de stack, dit gaat zolang door tot wij het beginpunt van mijn algorithme bereiken
                     */
                    case 0:
                        if (i == 0 || Main_Manager.Instance.grid[i - 1, j].visited == true)
                        {
                            northAlreadyVisited = true;

                        }
                        else
                        {
                            i--;
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i + 1, j].northWall.gameObject);
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j].southWall.gameObject);
                            Main_Manager.Instance.grid[i, j].SetCursor();
                            ResetAllVisitedBooleans();
                            allVisitedCells.Push(new int[] { i, j });
                            //voor animatie doeleinden delay ik dit met 0.1 seconden, dit kun je in je UI variabel veranderen
                            yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);
                            Main_Manager.Instance.grid[i, j].SetVisited();

                        }
                        break;
                    case 1:
                        if (j == width - 1 || Main_Manager.Instance.grid[i, j + 1].visited == true)
                        {
                            eastAlreadyVisited = true;
                        }
                        else
                        {
                            j++;
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j - 1].eastWall.gameObject);
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j].westWall.gameObject);
                            Main_Manager.Instance.grid[i, j].SetCursor();
                            ResetAllVisitedBooleans();
                            allVisitedCells.Push(new int[] { i, j });
                            yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);
                            Main_Manager.Instance.grid[i, j].SetVisited();
                        }
                        break;

                    case 2:
                        if (i == height - 1 || Main_Manager.Instance.grid[i + 1, j].visited == true)
                        {
                            southAlreadyVisited = true;
                        }
                        else
                        {
                            i++;
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i - 1, j].southWall.gameObject);
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j].northWall.gameObject);
                            Main_Manager.Instance.grid[i, j].SetCursor();
                            ResetAllVisitedBooleans();
                            allVisitedCells.Push(new int[] { i, j });
                            yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);
                            Main_Manager.Instance.grid[i, j].SetVisited();

                        }
                        break;

                    case 3:
                        if (j == 0 || Main_Manager.Instance.grid[i, j - 1].visited == true)
                        {
                            westAlreadyVisited = true;
                        }
                        else
                        {
                            j--;
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j + 1].westWall.gameObject);
                            UnityEngine.Object.Destroy(Main_Manager.Instance.grid[i, j].eastWall.gameObject);
                            Main_Manager.Instance.grid[i, j].SetCursor();
                            ResetAllVisitedBooleans();
                            allVisitedCells.Push(new int[] { i, j });
                            yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);
                            Main_Manager.Instance.grid[i, j].SetVisited();

                        }
                        break;

                    default:
                        break;
                }

            } while (allVisitedCells.Count > 0);
            void ResetAllVisitedBooleans()
            {
                northAlreadyVisited = false;
                eastAlreadyVisited = false;
                southAlreadyVisited = false;
                westAlreadyVisited = false;
            }
        }
    }
    
    

    public override string GetAlgorithmName()
{
    return "RecursiveBacktracker";
}
}



public class Prims : Algorithms
{
    /*hier is de algorithm van de Recursive Backtracker
     * Bron: http://people.cs.ksu.edu/~ashley78/wiki.ashleycoleman.me/index.php/Prim's_Algorithm.html
     * Ik kopieër geen code, ik lees de pseudo code en aan de hand daarvan programmeer ik mijn algorithme
     */

//    1.Choose a cell at random and add it to the list of visited cells.This is the current cell.
//    2.Mark all cells adjacent to the current cell
//    3.Randomly select a marked cell and remove its connecting edge to a cell from the list of visited cells.This is now the current cell.
//    4.Repeat 2 until no adjacent wall can be selected
//    5.While there are marked cells remaining go to 2

    //volgens de bron hierboven is dit medium-hard, mijn recursive backtracker was medium-easy.
   /*Edit (bericht nadat ik dit afkreeg): Gezien de tijd is mijn implementatie hiervan misschien niet zo mooi, maar het werkt 
    * Ik denk dat het mooier zou zijn geweest als ik een dictionary had gebruikt voor mijn marked en visitedcells, maar dat weet ik niet zeker
    * ik ga in mijn vrije tijd dit verbeteren en meerdere algorithms toevoegen
    */

        //ik maak alvast 2 lists aan zodat mijn programma kan onthouden welke cellen gevisit zijn en welke gemarked zijn
    private List<Tile> visitedCells;
    private List<Tile> markedCells;
    private Tile currentCell;

    public override void GenerateMaze(MonoBehaviour mono, int width, int height)
{   
        //nogmaals hulp booleans voor mijn do while loop
        bool northAlreadyVisited = false;
        bool eastAlreadyVisited = false;
        bool southAlreadyVisited = false;
        bool westAlreadyVisited = false;

        visitedCells = new List<Tile>();
        markedCells = new List<Tile>();

        //instance van System's Random class voor random nummers voor later
        System.Random random = new System.Random();


        //ik kies een random starting point
        int x = UnityEngine.Random.Range(0, height);
        int y = UnityEngine.Random.Range(0, width);

        //mijn current cell is de cell die randomly is gekozen
        currentCell = Main_Manager.Instance.grid[x, y];

        //nu instantiate ik mijn playercharacter, voor iedere nieuwe algorithme kun je deze line copy pasten
        Main_Manager.Instance.InstantiateCharacter(y, -x);

        //ik zet mijn eerste cel op visited en voeg het toe aan mijn visited cells
        currentCell.SetVisited();
        visitedCells.Add(currentCell);

        //ik start een coroutine voor mijn loop voor de animatie
        mono.StartCoroutine(GenerateMazeAnimation());

        IEnumerator GenerateMazeAnimation()
        {
            do
            {
                x = currentCell.xCoordinate;
                y = currentCell.yCoordinate;

                //alle cellen naast die cel worden gemarkeerd en in een list gezet
                //voordat ik de cellen naast mijn huidige cel mark, check ik of ze wel echt bestaan (randen checken) en of ze niet visited zijn (visited cellen moet je natuurlijk niet markeren)
                if (x != 0)
                {
                    if (Main_Manager.Instance.grid[x - 1, y].marked == false && Main_Manager.Instance.grid[x - 1, y].visited == false)
                    {
                        Main_Manager.Instance.grid[x - 1, y].SetMarked();
                        markedCells.Add(Main_Manager.Instance.grid[x - 1, y]);
                    }
                }
                if (x != height - 1)
                {
                    if (Main_Manager.Instance.grid[x + 1, y].marked == false && Main_Manager.Instance.grid[x + 1, y].visited == false)
                    {

                        Main_Manager.Instance.grid[x + 1, y].SetMarked();
                        markedCells.Add(Main_Manager.Instance.grid[x + 1, y]);
                    }

                }
                if (y != 0)
                {
                    if (Main_Manager.Instance.grid[x, y - 1].marked == false && Main_Manager.Instance.grid[x, y - 1].visited == false)
                    {

                        Main_Manager.Instance.grid[x, y - 1].SetMarked();
                        markedCells.Add(Main_Manager.Instance.grid[x, y - 1]);
                    }

                }
                if (y != width - 1)
                {
                    if (Main_Manager.Instance.grid[x, y + 1].marked == false && Main_Manager.Instance.grid[x, y + 1].visited == false)
                    {

                        Main_Manager.Instance.grid[x, y + 1].SetMarked();
                        markedCells.Add(Main_Manager.Instance.grid[x, y + 1]);
                    }

                }

                //dit kiest een random marked cell van mijn list
                int rMarked = random.Next(markedCells.Count);
                currentCell = markedCells[rMarked];
                //ik reset de 4 booleans die ik hierboven had gezet, want we beginnen mijn do while loop opnieuw
                ResetAllVisitedBooleans();
                //voor de animatie expres een delay
                yield return new WaitForSeconds(Main_Manager.Instance.animationSpeed);

                do
                {
                    int direction = UnityEngine.Random.Range(0, 4);
                    switch (direction)
                    {//hier loop ik door iedere direction randomly, als ik een visited cel vind die naast mijn markedcell bevind
                     //dan kan ik die openen, in visitedcells stoppen en opnieuw de naaste cellen markeren.
                        case 0:
                            if (currentCell.xCoordinate == 0 || Main_Manager.Instance.grid[currentCell.xCoordinate - 1, currentCell.yCoordinate].visited == false)
                            {
                                break;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].northWall.gameObject);
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate - 1, currentCell.yCoordinate].southWall.gameObject);
                                visitedCells.Add(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate]);
                                Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].SetVisited();
                                markedCells.Remove(currentCell);

                                northAlreadyVisited = true;
                            }
                            break;
                        case 1:
                            if (currentCell.yCoordinate == width - 1 || Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate + 1].visited == false)
                            {
                                break;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].eastWall.gameObject);
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate + 1].westWall.gameObject);
                                visitedCells.Add(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate]);
                                Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].SetVisited();
                                markedCells.Remove(currentCell);

                                eastAlreadyVisited = true;
                            }
                            break;
                        case 2:
                            if (currentCell.xCoordinate == height - 1 || Main_Manager.Instance.grid[currentCell.xCoordinate + 1, currentCell.yCoordinate].visited == false)
                            {
                                break;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].southWall.gameObject);
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate + 1, currentCell.yCoordinate].northWall.gameObject);
                                visitedCells.Add(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate]);
                                Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].SetVisited();
                                markedCells.Remove(currentCell);

                                southAlreadyVisited = true;
                            }
                            break;
                        case 3:
                            if (currentCell.yCoordinate == 0 || Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate - 1].visited == false)
                            {
                                break;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].westWall.gameObject);
                                UnityEngine.Object.Destroy(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate - 1].eastWall.gameObject);
                                visitedCells.Add(Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate]);
                                Main_Manager.Instance.grid[currentCell.xCoordinate, currentCell.yCoordinate].SetVisited();
                                markedCells.Remove(currentCell);

                                westAlreadyVisited = true;
                            }
                            break;
                        default:
                            break;
                    }//je ziet hieronder mijn conditions voor mijn inner en outer while loop, mijn inner loop die de muren vernietigd runt hierdoor 1 keer
                    //en kan daarna de naaste cellen markeren, daarna komt hij weer in de inner loop
                    //de onderste while loop is mijn outer while loop, dit zorgt er simpelweg voor dat de mazegenerator stopt zodra alle cellen zijn geopend
                } while (northAlreadyVisited == false && eastAlreadyVisited == false && southAlreadyVisited == false && westAlreadyVisited == false);
            } while (visitedCells.Count < width * height);
        }
        void ResetAllVisitedBooleans()
        {
            northAlreadyVisited = false;
            eastAlreadyVisited = false;
            southAlreadyVisited = false;
            westAlreadyVisited = false;
        }

    }
   
    public override string GetAlgorithmName()
{
    return "PrimsAlgorithm";
}
}


