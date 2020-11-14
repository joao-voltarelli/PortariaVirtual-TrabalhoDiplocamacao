using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
 /**
 * Essa classe representa um token de uma expressão lógica.
 * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
 */
    public class Token
    {
        public const int NaoReconhecido = 0;
        public const int AbreParenteses = 1;
        public const int FechaParenteses = 2;
        public const int Variavel = 3;
        public const int ConstanteString = 4;
        public const int ConstanteInteira = 5;
        public const int ConstanteFlutuante = 6;
        public const int OperadorCondicional = 7;
        public const int OperadorLogico = 8;
        public const int ExpressaoCompilada = 9;
        public string token;
        public int tipo = 0;
        public int coluna = 0;
        public ExpressaoLogica el;
    }
}
