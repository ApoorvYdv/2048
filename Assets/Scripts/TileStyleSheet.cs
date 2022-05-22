using UnityEngine;

public class TileStyleSheet : MonoBehaviour
{
    public static TileStyleSheet Instance;
    public StyleSheet[] tileStyles;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


}

[System.Serializable]
public class StyleSheet
{
    public int number = 2;
    public Color tileColor = Color.white;
    public Color numberColor = Color.black;
}