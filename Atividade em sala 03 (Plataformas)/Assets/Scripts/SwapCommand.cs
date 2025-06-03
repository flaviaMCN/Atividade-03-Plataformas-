using UnityEngine;

public class SwapCommand : ICommand
{
    private PuzzlePiece a, b;

    public SwapCommand(PuzzlePiece a, PuzzlePiece b)
    {
        this.a = a;
        this.b = b;
    }

    public void Execute()
    {
        Swap();
    }

    public void Undo()
    {
        Swap(); // Trocar de novo desfaz a troca
    }

    private void Swap()
    {
        int temp = a.currentIndex;
        a.currentIndex = b.currentIndex;
        b.currentIndex = temp;

        // Corrigir ordem visual no GridLayoutGroup
        int indexA = a.transform.GetSiblingIndex();
        int indexB = b.transform.GetSiblingIndex();

        a.transform.SetSiblingIndex(indexB);
        b.transform.SetSiblingIndex(indexA);
    }
}