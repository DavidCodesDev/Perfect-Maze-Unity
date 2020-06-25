using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorManager : MonoBehaviour
{
    //doorzichtige kleuren voor mijn hierarchy
    private Color red = new Color(1f, 0f, 0f, 0.2f);
    private Color green = new Color(0f, 1f, 0f, 0.2f);
    public Color blue = new Color(0f, 0f, 1f, 0.2f);
    private Color white = new Color(1f, 1f, 1f, 0.2f);

    static EditorManager() //static begint zodra de editor laadt
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyObject;
    }

    static void OnHierarchyObject(int ID, Rect r)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(ID) as GameObject;
        ApplyHierarchyCustomization(gameObject, r);


    }

    static void ApplyHierarchyCustomization(GameObject GO, Rect objectFrame)
    {
        if (GO == null) { return; } //we checken de gameobject, is het null? dan return
         
        //rest spreekt voorzich, aan de hand van zijn naam geven we het een kleur
        if (GO.name.Contains("Manager"))
        {
            GUI.DrawTexture(objectFrame, new Texture2D(Mathf.RoundToInt(objectFrame.width), Mathf.RoundToInt(objectFrame.height)), ScaleMode.ScaleToFit, true, 0f, new Color(0f, 0.6f, 1f, 0.3f), 0f, 0f);
        }
        else if (GO.name.Contains("Camera"))
        {
            GUI.DrawTexture(objectFrame, new Texture2D(Mathf.RoundToInt(objectFrame.width), Mathf.RoundToInt(objectFrame.height)), ScaleMode.ScaleToFit, true, 0f, new Color(1f, 1f, 0f, 0.3f), 0f, 0f);

        }

    }
}
