using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Color theme")]
public class Theme : ScriptableObject {

    public List<Color> brickColours;
    public Color background;
    public Color foreground;

    public int RandomColourID {
        get { return Random.Range(0, brickColours.Count); }
    }

}
