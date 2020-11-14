using ProcessadorLogicoCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortariaVirtual.Processador_Logico
{
    public class Processador
    {
        public string tipoEntidade;
        public string expressao;

        public bool Compila(string tipo, string expressaoArquivo)
        {
            try
            {
                Dictionary<string, object> contexto = new Dictionary<string, object>();
                /*
                Toda variável é precedida por @
                Tipos suportados: int, flot e string
                Operadores lógicos suportados: and or
                Operadores relacionais suportados: <, >, <=, >=, !=
                Aceita o uso de parenteses para determinar a precedência
                */
                tipoEntidade = tipo;

                contexto.Add("@tipoEntidadeReconhecida", tipoEntidade);

                //string expressao = "@tipoEntidadeReconhecida = \"NomePessoa\"";
                //string expressao = "@a=@b or @nome=\"Joao\"";

                expressao = expressaoArquivo;

                Compilador c = new Compilador();
                ExpressaoLogica el = c.compila(expressao);
                el.setContexto(contexto);

                //Console.WriteLine("Resultado: " + el.getValor());

                return el.getValor();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}