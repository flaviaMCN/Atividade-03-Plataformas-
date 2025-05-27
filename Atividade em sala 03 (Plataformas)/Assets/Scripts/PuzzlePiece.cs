using UnityEngine;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour
{
    public int correctIndex; // Posição final
    public int currentIndex; //Posição atual
    
    private Button button;
    private PuzzleManager manager;
   
    void Start()
    {
        button = GetComponent<Button>();
        manager = FindObjectOfType<PuzzleManager>();
        button.onClick.AddListener(OnPieceClicked);
    }

    private void OnPieceClicked()
    {
        manager.PieceCliked(this);
    }

    public void SetImage(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite; // Troca a imadem da peça
    }
}


