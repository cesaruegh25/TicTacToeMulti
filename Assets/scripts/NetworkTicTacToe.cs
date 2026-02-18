using UnityEngine;
using Fusion;
using System.Linq;
using UnityEngine.Rendering;

public class NetworkTicTacToe : NetworkBehaviour
{
    // 0 = casilla vacia, 1 = player 1, 2 = player 2

    [Networked, Capacity(9)]
        public NetworkArray<int> Board => default;
    [Networked]
        public PlayerRef TurnoActual { get; set; }
    [Networked]
        public bool EndGame { get; set; }


    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            TurnoActual = Runner.LocalPlayer;
            EndGame = false;
            for(int i = 0; i < 9; i++)
            {
                Board.Set(i, 0);
            }
        }
    }

    public void TryTurn(int index)
    {
        if (EndGame) // si la partida termina no hacemos nada
        {
            return;
        }

        RPC_Play(index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Play(int index, RpcInfo info = default)
    {
        if (EndGame) { return; }

        if (Board.Get(index) != 0) return;

        PlayerRef playerCaller = info.Source; // player que hizo la llamada al RPC

        if (playerCaller != TurnoActual) return;

        int player = (TurnoActual == Runner.ActivePlayers.ToList()[0]) ? 1 : 2;

        Board.Set(index, player);

        if (CheckWin(player))
        {
            EndGame = true;
        }
        else
        {
            ChangeTurn();
        }


    }

    public void ChangeTurn()
    {
        foreach(var player in Runner.ActivePlayers)
        {
            if (player != TurnoActual)
            {
                TurnoActual = player;
                break;
            }
        }
    }

    public bool CheckWin(int player)
    {
        int[,] combination = new int[,]
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 } 
        };

        for(int i = 0; i < 8 ; i++)
        {
            if (Board.Get(combination[i, 0]) == player
                && Board.Get(combination[i, 1]) == player
                && Board.Get(combination[i, 2]) == player)
            {
                return true;
            }
        }
        return false;
    }
}
