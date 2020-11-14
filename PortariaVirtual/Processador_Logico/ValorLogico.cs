using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
    /**
    * Essa interface define as caracteristicas que uma classe que pode retorna um valor
    * lógico deve conter
    * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
    */
    public interface ValorLogico
    {
        bool getValor();
        void setContexto(Dictionary<string, object> contexto);
    }
}
