using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public GameObject piecePrefab;
    public Sprite[] pieces;
    public Transform grid;

    private PuzzlePiece selectedPiece = null;
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private List<ICommand> replayList = new List<ICommand>();
    private PuzzlePiece[] puzzlePieces;

    private bool cancelReplay = false;

    void Start()
    {
        
        
        GeneratePuzzle();
        ShufflePuzzle();
    }

    void GeneratePuzzle()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
        
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

    private void ShufflePuzzle()
    {
        List<Transform> children = new List<Transform>();

        // Coleta os filhos atuais do grid
        foreach (Transform child in grid)
            children.Add(child);

        // Embaralha a lista
        for (int i = 0; i < children.Count; i++)
        {
            Transform temp = children[i];
            int randomIndex = Random.Range(i, children.Count);
            children[i] = children[randomIndex];
            children[randomIndex] = temp;
        }

        // Reordena visualmente os filhos no grid
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);

            // Atualiza o currentIndex das peças
            PuzzlePiece piece = children[i].GetComponent<PuzzlePiece>();
            piece.currentIndex = i;
        }

        undoStack.Clear();
        replayList.Clear();
    }

    public void PieceClicked(PuzzlePiece piece)
    {
        if (selectedPiece == null)
        {
            selectedPiece = piece;
            return;
        }

        if (selectedPiece == piece)
        {
            selectedPiece = null;
            return;
        }

        ICommand cmd = new SwapCommand(selectedPiece, piece);
        cmd.Execute();
        undoStack.Push(cmd);
        replayList.Add(cmd);

        selectedPiece = null;

        CheckWin();
    }

    public void Undo()
    {
        if (selectedPiece != null) return;

        if (undoStack.Count > 0)
        {
            ICommand cmd = undoStack.Pop();
            cmd.Undo();
        }
    }

    void CheckWin()
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
        // Aqui você pode ativar um painel de vitória na interface
        Debug.Log("Mostrar UI de vitória");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Puzzle");
    }

    public void StartReplay()
    {
        StartCoroutine(PlayReplay());
    }

    public void CancelReplay()
    {
        cancelReplay = true;
    }

    IEnumerator PlayReplay()
    {
        cancelReplay = false;

        foreach (var cmd in replayList)
        {
            if (cancelReplay) break;

            cmd.Execute();
            yield return new WaitForSeconds(1f); // Espera 1 segundo entre comandos
        }

        if (cancelReplay)
        {
            // Executa o restante das jogadas instantaneamente
            foreach (var cmd in replayList)
                cmd.Execute();
        }
    }
}


