using UnityEngine;

public class WalkablePlayer : MonoBehaviour
{
    //dit is als bonus een player die je kunt besturen in jouw maze
    //door middel van physics kun je je player besturen
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    /* Nu even iets interessants over physics in Unity:
     * Veel mensen coden player movement door een transform.translate te gebruiken, dit is SLECHT, omdat je hiermee eigenlijk direct een 
     * positie set, dit is niet goed omdat je als het ware je player ''teleport'' door direct de position van je player te setten, als je dan
     * een fast paced game hebt, kan jouw player door muren heen gaan. Daarom is het beter om direct je player's rigidbody velocity (snelheid) te setten
     * of een force (addForce) te geven, hiermee zorgt je physics engine voor een veiligere en vloeibare movement.
     * Update() runt iedere frame, FixedUpdate() is perfect voor physics omdat dat framerate independent is.
     * Wil je alsnog Update() gebruiken? Dan moet je je snelheidsvariabelen vermenigvuldigen met Time.deltaTime, dit zorgt ervoor dat op iedere machine
     * je snelheid hetzelfde zal zijn (framerate independent)
     * Dit kun je testen als je een heavy performance game hebt gemaakt en je trekt de charger van je laptop uit, dan gaat je laptop in besparings mode en zul je merken
     * dat je snelheid ingame opeens heel erg sloom is, als je dan je snelheid met time.deltatime vermenigvuldigt dan is het gefixt. 
     * Time.deltaTime geeft de tijd in seconden tussen je huidige frame en vorige frame.
     * Aangezien ik addforce zal gebruiken - wat physics is - gebruik ik FixedUpdate. (Addforce gebruikt in zichzelf al trouwens een * time.deltatime)
     */
    void FixedUpdate()
    {
        if (Input.GetKey("w"))
        {
            rb.AddForce(new Vector2(0f, 20f));
        }
        if (Input.GetKey("a"))
        {
            rb.AddForce(new Vector2(-20f, 0f));
        }
        if (Input.GetKey("s"))
        {
            rb.AddForce(new Vector2(0f, -20f));
        }
        if (Input.GetKey("d"))
        {
            rb.AddForce(new Vector2(20f, 0f));
        }
    }
}
