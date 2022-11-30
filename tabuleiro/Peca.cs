using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tabuleiro.Enum;

namespace tabuleiro
{
    abstract class Peca
    {
        public Posicao Posicao { get; set; }
        public Cor Cor { get; protected set; }
        public int QteMovimentos { get; protected set; }
        public Tabuleiro Tabuleiro { get; protected set; }

        public Peca(Cor cor, Tabuleiro tabuleiro)
        {
            Cor = cor;
            Tabuleiro = tabuleiro;
        }

        public void IncrementarQteMovimentos()
        {
            QteMovimentos++;
        }

        public void DecrementarQteMovimentos()
        {
            QteMovimentos--;
        }

        public bool PodeMoverPara(Posicao posicao)
        {
            return MovimentosPossiveis()[posicao.Linha, posicao.Coluna];
        }

        public bool ExisteMovimentosPossiveis()
        {
            bool[,] mat = MovimentosPossiveis();
            for (int i = 0; i < Tabuleiro.Linhas; i++)
            {
                for(int j = 0; j < Tabuleiro.Linhas; j++)
                {
                    if (mat[i, j]) return true;
                }
            }

            return false;
        }

        public abstract bool[,] MovimentosPossiveis();
    }
}
