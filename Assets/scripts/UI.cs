using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public NetworkTicTacToe Game;
    public Button[] Buttons;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI endGameText;



    private void Start()
    {
        InvokeRepeating(nameof(FindGame), 0.5f, 0.5f);
        for (int i = 0; i < Buttons.Length; i++)
        {
            int index = i; // Necesario para evitar el problema de cierre en el bucle
            Buttons[i].onClick.AddListener(() => Game.TryTurn(index));
        }
    }
    void Update()
    {
        if (Game == null) return;

        for (int i = 0; i < 9; i++)
        {
            int var = Game.Board.Get(i);
            Buttons[i].GetComponentInChildren<TMP_Text>().text = var == 0 ? "" : (var == 1 ? "X" : "O");
        }

        turnText.text = Game.EndGame ? "Final de partida" : $"turno: { Game.TurnoActual }";
        
    }

    void FindGame()
    {
        if (Game == null)
        { 
            Game = FindObjectOfType<NetworkTicTacToe>();
        }
    }

}
