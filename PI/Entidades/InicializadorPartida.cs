﻿using lobby;
using MagicTrickServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaAutonomo.Entidades
{
    public class InicializadorPartida
    {
        private Dictionary<int, Jogador> jogadores;
        private Jogador jogadorNaMaquina;
        public Partida partida;
        public Bot Bot;
        public int IdPartida { get; set; }

        public InicializadorPartida(string[] jogadorNaMaquinaString,int idPartida,Form game)
        {
            jogadores = new Dictionary<int, Jogador>();
            IdPartida = idPartida;
            IniciarPartida(idPartida, jogadorNaMaquinaString,game);
        }
            
        private void IniciarPartida(int idPartida, string[] jogadorNaMaquinaString,Form game)
        {
            CriarJogadores(jogadorNaMaquinaString);
            DistribuirCartas();
            partida = new Partida(jogadores,jogadores.Keys.ToList(),game,idPartida);
        }

        private void CriarJogadores(string[] jogadorNaMaquinaString)
        {            
            List<int>IdJogadores = ConfiguracaoPartida.ObterOrdemMesa(jogadorNaMaquinaString, IdPartida);
            List<PosicaoCartas> Maos = ConfiguracaoPartida.PosicaoCartas(IdPartida);
            string senhaJogadorMaquina = jogadorNaMaquinaString[1];
            int IdJogadorMaquina = int.Parse(jogadorNaMaquinaString[0]);


            for (int i = 0;i < IdJogadores.Count; i++)
            {
                string nomeJogador = GerenciadorStrings.ObterNomeDoJogador(IdJogadores[i], IdPartida);
                if (IdJogadores[i] == IdJogadorMaquina)
                {
                    jogadorNaMaquina = new Jogador(IdJogadorMaquina, Maos[i], nomeJogador, senhaJogadorMaquina);
                    jogadores.Add(IdJogadores[i], jogadorNaMaquina);
                }
                else
                {
                    jogadores.Add(IdJogadores[i], new Jogador(IdJogadores[i], Maos[i], nomeJogador));
                }
            }
        }

        public void DistribuirCartas()
        {
            string[] todasAsCartas = GerenciadorStrings.ObterCartasDaPartida(IdPartida);

            foreach(string carta in todasAsCartas)
            {
                string[] informacoesDaCarta = carta.Split(',');
                int idJogador = int.Parse(informacoesDaCarta[0]);
                int idCarta = int.Parse(informacoesDaCarta[1]);
                char naipe = char.Parse(informacoesDaCarta[2]);

                jogadores[idJogador].Cartas.AdicionarCarta(naipe,idCarta);
            }
        }

        public void InicializarBot(Timer trmTimer)
        {
            Bot = new Bot(partida,IdPartida, trmTimer, jogadores);
        }
    }
}
