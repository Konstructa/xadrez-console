using tabuleiro;
using tabuleiro.Enum;
using xadrez;
using xadrez_console.tabuleiro.Exceptions;

namespace xadrez_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Tabuleiro tabuleiro = new(8, 8);

                Peca peca = new(Cor.Preta, tabuleiro);
                tabuleiro.ColocarPeca(new Torre(Cor.Preta, tabuleiro), new Posicao(0, 0));
                tabuleiro.ColocarPeca(new Rei(Cor.Preta, tabuleiro), new Posicao(2, 9));
                tabuleiro.ColocarPeca(new Torre(Cor.Preta, tabuleiro), new Posicao(0, 0));

                //Console.WriteLine("Posicao: " + peca.Posicao);

                Tela.ImprimirTabuleiro(tabuleiro);
            }
            catch (TabuleiroException e)
            {

                Console.WriteLine(e.Message);
            }
        }
    }
}