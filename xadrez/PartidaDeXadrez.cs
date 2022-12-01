using System.Collections.Generic;
using tabuleiro;
using tabuleiro.Enum;
using xadrez;
using xadrez_console.tabuleiro.Exceptions;

namespace xadrez_console.xadrez
{
     class PartidaDeXadrez
    {
        public Tabuleiro Tabuleiro { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas;
        private HashSet<Peca> Capturadas;
        public bool Xeque { get; private set; }

        public PartidaDeXadrez()
        {
            Tabuleiro = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.Branca;
            Terminada = false;
            Xeque = false;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            ColocarPecas();
        }

            
        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tabuleiro.RetirarPeca(origem);

            p.IncrementarQteMovimentos();

            Peca pecaCapturada = Tabuleiro.RetirarPeca(destino);

            Tabuleiro.ColocarPeca(p, destino);

            if (pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }

            return pecaCapturada;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em cheque");
            }

            if(EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            } 
            else
            {
                Xeque = false;
            }

            if (TesteXequemate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }

            Turno++;
            MudaJogador();
        }

        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tabuleiro.RetirarPeca(destino);
            p.DecrementarQteMovimentos();

            if(pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                Capturadas.Remove(pecaCapturada);
            }

            Tabuleiro.ColocarPeca(p, origem);
        }

        private void MudaJogador()
        {
            if (JogadorAtual == Cor.Branca)
            {
                JogadorAtual = Cor.Preta;
            } 
            else
            {
                JogadorAtual = Cor.Branca;
            }

            //JogadorAtual = Adversaria(JogadorAtual);
        }

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }

        private Peca Rei(Cor cor)
        {
            foreach (Peca peca in PecasEmjogo(cor))
            {
                if(peca is Rei)
                {
                    return peca;
                }
            }

            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca rei = Rei(cor);

            foreach(Peca peca in PecasEmjogo(Adversaria(cor)))
            {
                bool[,] mat = peca.MovimentosPossiveis();
                if (mat[rei.Posicao.Linha, rei.Posicao.Coluna])
                {
                    return true; 
                }
            }

            return false;
        }

        public bool TesteXequemate(Cor cor)
        {
            if (!EstaEmXeque(cor)) return false ;

            foreach (Peca peca in PecasEmjogo(cor))
            {
                bool[,] mat = peca.MovimentosPossiveis();

                for (int i = 0; i < Tabuleiro.Linhas; i++)
                {
                    for(int j = 0; j < Tabuleiro.Linhas; j++)
                    {
                        if(mat[i,j])
                        {
                            Posicao destino = new (i, j);
                            Peca pecaCapturada = ExecutaMovimento(peca.Posicao, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(peca.Posicao, destino, pecaCapturada);

                            if(!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void ValidarPosicaoDeOrigem(Posicao posicao)
        {
            if (Tabuleiro.Peca(posicao) == null)
            {
                throw new TabuleiroException("Não existe peça na posição escolhida!");
            }

            if (JogadorAtual != Tabuleiro.Peca(posicao).Cor)
            {
                throw new TabuleiroException("A peça escolhida não é sua");
            }

            if (!Tabuleiro.Peca(posicao).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos disponíveis para essa peça");
            }
        }

        public void ValidarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if(!Tabuleiro.Peca(origem).PodeMoverPara(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> pecasCapturadas = new HashSet<Peca>();

            foreach(Peca peca in Capturadas) {
                if(peca.Cor == cor) pecasCapturadas.Add(peca);
            }

            return pecasCapturadas;
        }

        public HashSet<Peca> PecasEmjogo(Cor cor)
        {
            HashSet<Peca> pecasEmJogo = new HashSet<Peca>();

            foreach (Peca peca in Pecas)
            {
                if (peca.Cor == cor) pecasEmJogo.Add(peca);
            }
            pecasEmJogo.ExceptWith(PecasCapturadas(cor));
            return pecasEmJogo;
        }


        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            ColocarNovaPeca('c', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarNovaPeca('c', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarNovaPeca('d', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarNovaPeca('e', 2, new Torre(Cor.Branca, Tabuleiro));
            ColocarNovaPeca('e', 1, new Torre(Cor.Branca, Tabuleiro));
            ColocarNovaPeca('d', 1, new Rei(Cor.Branca, Tabuleiro));

            ColocarNovaPeca('c', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarNovaPeca('c', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarNovaPeca('d', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarNovaPeca('e', 7, new Torre(Cor.Preta, Tabuleiro));
            ColocarNovaPeca('e', 8, new Torre(Cor.Preta, Tabuleiro));
            ColocarNovaPeca('d', 8, new Rei(Cor.Preta, Tabuleiro));

        }
    }
}
