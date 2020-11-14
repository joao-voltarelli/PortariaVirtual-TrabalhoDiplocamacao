using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
 /**
 * Essa classe representa um erro encontrado durante o processo de compilação de uma expressão lógica
 * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
 */
    public class CompilationException : Exception
    {
        public CompilationException(string mensagem) : base(mensagem)
        { }
    }
}
