using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
 /**
 * Essa classe representa um erro ocorrido durante o processo de execução de uma expressão lógica
 * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
 */
    public class ExecutionException : Exception
    {
        public ExecutionException(string mensagem) : base(mensagem)
        { }
    }
}
