using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Theme", menuName="Scriptable Objects/Theme")]
public class Theme : ScriptableObject 
{
    [SerializeField] private List<Color> _brickColours;
    [SerializeField] Color _background;
    [SerializeField] Color _foreground;

    public List<Color> BrickColours { get => _brickColours; }
    public Color Background { get => _background; }
    public Color Foreground { get => _foreground; }
    public int RandomColourID { get => Random.Range(0, _brickColours.Count); }
}
