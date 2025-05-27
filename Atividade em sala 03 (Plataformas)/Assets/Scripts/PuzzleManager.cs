using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public GameObject piecePrefab;
    public Sprite[] pieces;
    public Transform grid;

    private PuzzlePiece selectedPiece = null;
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private List<ICommand> replayList = new List<ICommand>();

    private PuzzlePiece[] puzzlePieces;

    void Start()
    {
        GeneratePuzzle();
        ShufflePuzzle();
    }

    void GeneratePuzzle() // Posiciona as imagens com os índices corretos
    {
        puzzlePieces = new PuzzlePiece[16];
        for (int i = 0; i < 16; i++)
        {
            GameObject obj = Instantiate(piecePrefab, grid);
            PuzzlePiece piece = obj.GetComponent<PuzzlePiece>();
            
            piece.correctIndex = i;
            piece.currentIndex = i;
            piece.SetImage(pieces[i]);
            
            puzzlePieces[i] = piece;
        }
    }
    
    private void ShufflePuzzle() // Troca aleatória
    {
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            int ranIndex = Random.Range(0, puzzlePieces.Length);
            if (i != ranIndex)
            {
                PuzzlePiece a = puzzlePieces[i];
                PuzzlePiece b = puzzlePieces[ranIndex];

                int temp = a.currentIndex;
                a.currentIndex = b.currentIndex;
                b.currentIndex = temp;

                int indexA = a.transform.GetSiblingIndex();
                int indexB = b.transform.GetSiblingIndex();

                a.transform.SetSiblingIndex(indexB);
                b.transform.SetSiblingIndex(indexA);
            }
        }
    }


    public void Undo()
    {
        if (selectedPiece == null && undoStack.Count > 0)
        {
            ICommand cmd = undoStack.Pop();
            cmd.Undo();
        }
    }

    void CheckWin() // Verifica se as peças estão corretas
    {
        foreach (var piece in puzzlePieces)
        {
            if (piece.currentIndex != piece.correctIndex)
                return;
        }

        Debug.Log("Parabéns! Você venceu!");
        ShowWinUI();
    }

    public void ShowWinUI()
    {

    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Puzzle");
    }

    public void StartGame()
    {
        StartCoroutine(PlayRepaly());
    }

    private bool cancelRepaly = false;

    public void CancelRepaly()
    {
        cancelRepaly = true;
    }

    IEnumerator PlayRepaly()
    {
        cancelRepaly = false;
        foreach (var cmd in replayList)
        {
            if (cancelRepaly)
                break;

            cmd.Execute();
            yield return new WaitForSeconds(0.1f);
        }

        if (cancelRepaly)
        {
            for (int i = replayList.IndexOf(replayList.Find(c => !cancelRepaly)); i < replayList.Count; i++)
            {
                replayList[i].Execute();
            }
        }

        ShowWinUI();
    }

    public void PieceCliked(PuzzlePiece piece)
    {
        if (selectedPiece == null)
        {
            selectedPiece = piece;
        }
        else
        {
            if (selectedPiece != piece)
            {
                ICommand cmd = new SwapCommand(selectedPiece, piece);
                cmd.Execute();
                undoStack.Push(cmd);
                replayList.Add(cmd);
                CheckWin();
            }

            selectedPiece = null;
        }
    }
}


