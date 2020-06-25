//ik gebruik zodat ik de Enum static method kan accessen
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    //hier maak ik weer een static instance van mijn uimanager zodat ik makkelijker methods en variables kan referencen vanuit andere scripts
    public static UI_Manager Instance { get; private set; }

    //hier zet ik mijn dropdown menu, dit gebruik ik om mijn dropdown menu dynamically to populaten
    public TMP_Dropdown algorithmDropdown;

    //als een user iets verkeerds intypt, zal hij/zij een error message zien
    public TMP_Text errorText;

    //dit heb ik nodig zodat ik mijn UI kan translaten
    public RectTransform canvasPanel;
    private Vector2 panelVisiblePos;
    private Vector2 panelHiddenPos;
    private bool panelIsVisible = false;

    //geheime funmode ;) activeer het in je UI en regenerate je maze en bestuur je character om te zien wat dit doet
    [HideInInspector]
    public bool funMode = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //hier set ik de UI waarden voor de animaties
        panelVisiblePos = canvasPanel.anchoredPosition;
        panelHiddenPos = new Vector2(-canvasPanel.anchoredPosition.x, canvasPanel.anchoredPosition.y);
        
        //hier voed ik mijn dropdown menu alle algorithmes
        PopulateDropdown();     
    }

    private void PopulateDropdown()
    {
        algorithmDropdown.ClearOptions(); //ik delete eerst alle bestaande options

        /*Nu komt iets interessants:
         * 
         * Nu moet ik de dropdown menu populaten met alle algorithm namen,  wat ik graag wil doen is
         * dat ik maar 1 keer in Algorithms.cs alle algorithms implementeer en daarna nergens anders meer een code aanraak.
         * Normaal zou ik mijn dropdown options populaten door alle classes die de class Algorithms inheriten te instantiaten
         * en daarna de GetAlgorithmName() functie te callen op iedere instantiated object, en dit in een array stoppen, maar het probleem is:
         * stel ik heb dan 50 algorithms geïmplementeerd, dan moet ik alle 50 algorithms nogmaals hardcoded instantiated en in een array stoppen
         * om mijn dropdown te populaten. Dit is anti-scalability. 
         * Daarom gebruik ik Reflection in C#. Hiermee kan ik alle childclasses van de parent class ''Algorithms'' instantiaten, zonder de namen te hardcoden
         * Edit: Blijkbaar hoeft System.Reflection niet, alleen System... en met Linq kan ik de exacte Types selecteren
         */

        Algorithms[] algorithms = typeof(Algorithms).Assembly.GetTypes()
        .Where(e => e.GetType() != typeof(Algorithms) && typeof(Algorithms).IsAssignableFrom(e) && !e.IsAbstract)
        .Select(e => Activator.CreateInstance(e) as Algorithms)
        .ToArray();

        //ik gebruik hier een list ipv een array zodat ik simpelweg .Add kan doen
        List<String> algorithmNames = new List<string>();

        for (int i = 0; i < algorithms.Length; i++)
        {
            //ik trigger hier de GetAlgorithmName function van iedere algorithm, zodat we daarna de dropdown kunnen populaten met de namen
            algorithmNames.Add(algorithms[i].GetAlgorithmName());
        }
        //hier voeg ik alle namen toe aan de dropdown
        algorithmDropdown.AddOptions(algorithmNames);

        //hier zet ik de default value van de currentAlgorithm op de eerste algorithm.
        Main_Manager.Instance.SetAlgorithm(algorithms[0]);

        //hier add ik een listener aan de onvaluechanged event van de dropdown, dus als de user een algorithme kiest dan wordt dat in de main manager gezet als de currentAlgorithm
        algorithmDropdown.onValueChanged.AddListener(delegate { Main_Manager.Instance.SetAlgorithm(algorithms[algorithmDropdown.value]); });
    }
    
    //UI panel button toggle voor de animatie, animatie doe ik in Update zodat ik een mooie Lerp kan gebruiken
    public void ToggleUiPanel()
    {
        panelIsVisible = !panelIsVisible;
    }

    public void ToggleFunMode(bool value)
    {//spreekt voorzich
        if (value == true)
        {
            funMode = true;
        }
        else
        {
            funMode = false;
        }
    }
    private void Update()
    {//mooie lerp ''animatie'' om mijn UI panel te hiden en te unhiden
        if (panelIsVisible)
        {
            canvasPanel.anchoredPosition = Vector2.Lerp(canvasPanel.anchoredPosition, panelHiddenPos, 0.1f);
        }
        else
        {
            canvasPanel.anchoredPosition = Vector2.Lerp(canvasPanel.anchoredPosition, panelVisiblePos, 0.1f);
        }

        //ook kun je je UI panel met je ESC button weghalen en weer terug brengen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUiPanel();
        }
    }
}
