using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
 /**
 * Essa classe representa uma variável lógica
 * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
 */
    public class ResultadoLogico : ValorLogico
    {
        private bool valor;
        public void setValor(bool valor)
        {
            this.valor = valor;
        }
        public bool getValor()
        {
            try
            {
                return valor;
            }
            catch(ExecutionException ex)
            {
                throw ex;
            }
        }
        public void setContexto(Dictionary<string, object> contexto)
        { }
        public string toString()
        {
            return "" + valor;
        }
    }
}
