using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
    /**
    * Essa classe representa uma expressão lógica pronta para ser executada utilizando as
    * variáveis existentes em um contexto de execução, que é representado por uma tabela hash.
     * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
    */
    public class ExpressaoLogica : ValorLogico
    {
        private bool sempreTrue = false;

        public bool isSempreTrue()
        {
            return sempreTrue;
        }
        public void setSempreTrue(bool sempreTrue)
        {
            this.sempreTrue = sempreTrue;
        }

        /**
        * Operações lógicas existentes na expressão
        */
        private List<ValorLogico> operacoesLogicas = new List<ValorLogico>();
        /**
        * Operadores lógicos existentes na expressão.
        * Um operador na posição x da lista está posicionado na expressão entre as operações
        * lógicas da posição x e x + 1 da lista de operações.
        */
        private List<string> operadoresLogicos = new List<string>();
        /**
        * Tabela hash contendo as variáveis do contexto de execução
        */
        private Dictionary<string, object> contexto;
        /**
        * Adiciona uma nova operação lógica à expressão
        * @param operacao Operação a ser adicionada
        */
        public void adicionaOperacao(ValorLogico operacao)
        {
            operacoesLogicas.Add(operacao);
        }
        /**
        * Adiciona um novo operador lógico à expressão
        * @param operador Operador (and ou or) a ser adicionado
        */
        public void adicionaOperadorLogico(String operador)
        {
            operadoresLogicos.Add(operador);
        }
        /**
        * Realiza a resolução de duas operações lógicas interligadas por um operador lógico.
        * Quando o resultado (true ou false) é obtido as duas operações lógicas e o operador
        * são retirados das listas e apenas o resultado é inserido no lugar das operações.
        * @param posicao Posição da primeira operação lógica a ser resolvida
        * @throws ExecutionException
        */
        public void resolveReduz(int posicao)
        {
            try
            {
                ResultadoLogico r = new ResultadoLogico();
                ValorLogico operando1, operando2;
                operando1 = operacoesLogicas[posicao];
                operando2 = operacoesLogicas[posicao + 1];
                operando1.setContexto(contexto);
                operando2.setContexto(contexto);
                string operador = operadoresLogicos[posicao];
                //Resolução...
                if (operador.Equals("and"))
                    r.setValor(operando1.getValor() && operando2.getValor());
                else if (operador.Equals("or"))
                    r.setValor(operando1.getValor() || operando2.getValor());
                else
                    throw new ExecutionException("O operador " + operador + " não é reconhecido");
                //Redução...
                operadoresLogicos.RemoveAt(posicao);
                operacoesLogicas.RemoveAt(posicao);
                operacoesLogicas.RemoveAt(posicao);
                operacoesLogicas.Insert(posicao, r);
            }
            catch (ExecutionException ex)
            {
                throw ex;
            }
        }
        /**
        * Realiza a resolução de uma expressão lógica. Primeiro são processados os operadores
        * "and" e posteriormente os operadores "or"
        * @return Resultado lógico da expressão
        * @throws ExecutionException
        */
        public bool getValor()
        {
            try
            {
                if (sempreTrue)
                    return true;
                List<ValorLogico> bkpOperacoes = new List<ValorLogico>(operacoesLogicas);
                List<string> bkpOperadores = new List<string>(operadoresLogicos);
                //Resolve AND...
                for (int i = 0; i < operadoresLogicos.Count; i++)
                {
                    string atual = operadoresLogicos[i];
                    if (atual.Equals("and"))
                    {
                        resolveReduz(i);
                        i--;
                    }
                }
                //Resolve OR...
                for (int i = 0; i < operadoresLogicos.Count; i++)
                {
                    String atual = operadoresLogicos[i];
                    if (atual.Equals("or"))
                    {
                        resolveReduz(i);
                        i--;
                    }
                }
                operacoesLogicas[0].setContexto(contexto);
                bool retorno = operacoesLogicas[0].getValor();
                operacoesLogicas = bkpOperacoes;
                operadoresLogicos = bkpOperadores;
                return retorno;
            }
            catch(ExecutionException ex)
            {
                throw ex;
            }
        }
        /**
        * Gera a expressão no formato de uma String
        * @return Expressão lógica
        */
        public string toString()
        {
            String retorno = "(";
            int ol = 0;
            for (int i = 0; i < operacoesLogicas.Count; i++)
            {
                retorno = retorno + operacoesLogicas[i];
                if (i < operacoesLogicas.Count - 1)
                    retorno = retorno + " " + operadoresLogicos[ol] + " ";
                ol++;
            }
            return retorno + ")";
        }
        /**
        * Define o contexto de execução da expressão lógica.
        * O nome da variável é a chave para o valor da tabela. Sendo assim, se a variável de nome
        * &idade possui o valor 10, é preciso inseri-la na tabela conforme o exemplo abaixo:
        * HashMap contexto = new HashMap();
        * contexto.put("&idade", 10);
        * @param contexto Tabela hash contendo o contexto de execução.
        */
        public void setContexto(Dictionary<string, object> contexto)
        {
            this.contexto = contexto;
        }
        /**
        * Retorna o contexto de execução da expressão lógica
        * @return
        */
        public Dictionary<string, object> getContexto()
        {
            return contexto;
        }

        public List<ValorLogico> getOperacoesLogicas()
        {
            return operacoesLogicas;
        }
    }
}
