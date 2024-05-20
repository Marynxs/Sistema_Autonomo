﻿using MagicTrickServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaAutonomo.Entidades
{
    public class GerenciadorDeStrings
    {
        public static string[] ObterCartasDaPartida(int idPartida)
        {

            string CartasList = Jogo.ConsultarMao(idPartida);
            if (CartasList.Length > 4 && CartasList.Substring(0, 4) == "ERRO")
            {
                MessageBox.Show($"Ocorreu um erro ao verificar ao consultar mão:\n{CartasList.Substring(5)}", "MagicTrick", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            CartasList = CartasList.Replace("\r", "");
            if (CartasList.Length > 0)
                CartasList = CartasList.Substring(0, CartasList.Length - 1);

            return CartasList.Split('\n');
        }

        public static string[] ObterInformacaoDosJogadores(int idPartida)
        {
            string retornoBruto = Jogo.ListarJogadores(idPartida);
            if (retornoBruto.Length > 4 && retornoBruto.Substring(0, 4) == "ERRO")
            {
                MessageBox.Show($"Ocorreu um erro ao contar os jogadores:\n{retornoBruto.Substring(5)}", "MagicTrick", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            retornoBruto = retornoBruto.Replace("\r", "");
            if (retornoBruto.Length > 0)
                retornoBruto = retornoBruto.Substring(0, retornoBruto.Length - 1);

            return retornoBruto.Split('\n');

        }

        public static string ObterNomeDoJogador(int idJogador, int idPartida)
        {
            string[] retornoBruto = ObterInformacaoDosJogadores(idPartida);

            foreach(string jogador in retornoBruto)
            {
                string[] retornoTratado = jogador.Split(',');
                if (idJogador == int.Parse(retornoTratado[0]))
                {
                    return retornoTratado[1];
                }
            }
            return null;
        }

        public static string[] ObterJogadas(int idPartida)
        {
            string retornoBruto = Jogo.ExibirJogadas2(idPartida);
            if (retornoBruto.Length > 4 && retornoBruto.Substring(0, 4) == "ERRO")
            {
                MessageBox.Show($"Ocorreu um erro ao obter as jogadas:\n{retornoBruto.Substring(5)}", "MagicTrick", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            else if(retornoBruto == "")
            {
                return null;
            }
            retornoBruto = retornoBruto.Replace("\r", "");
            if (retornoBruto.Length > 0)
                retornoBruto = retornoBruto.Substring(0, retornoBruto.Length - 1); 

            return retornoBruto.Split('\n');
        }
    }
}
