using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
    /**
    * Essa classe representa uma operação lógica.
    * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
    */
    public class OperacaoLogica : ValorLogico
    {
        /**
        * Primeiro operando
        */
        private Token operando1;
        /**
         * Operador lógico
         */
        private Token operador;
        /**
         * Segundo operando
         */
        private Token operando2;
        /**
         * Contexto de execução contendo as variáveis
         */
        private Dictionary<string, object> contexto;
        public OperacaoLogica() { }
        public OperacaoLogica(Dictionary<string, object> contexto, Token operando1, Token operador, Token operando2)
        {
            this.contexto = contexto;
            this.operando1 = operando1;
            this.operador = operador;
            this.operando2 = operando2;
        }
        public void setContexto(Dictionary<string, object> contexto)
        {
            this.contexto = contexto;
        }
        public void setOperando1(Token operando1)
        {
            this.operando1 = operando1;
        }
        public Token getOperando1()
        {
            return operando1;
        }
        public void setOperador(Token operador)
        {
            this.operador = operador;
        }
        public Token getOperador()
        {
            return operador;
        }
        public void setOperando2(Token operando2)
        {
            this.operando2 = operando2;
        }
        public Token getOperando2()
        {
            return operando2;
        }

        /**
        * Resolve e retorna o resultado da operação lógica
        * @return Resultado da resolução
        * @throws ExecutionException
        */
        public bool getValor()
        {
            try
            {
                Object operando1 = contexto[this.operando1.token];
                Object operando2 = null;
                if (operando1 == null)
                    throw new ExecutionException("Variável " + this.operando1.token + " não encontrada no ambiente de execução");
                if (this.operando2.tipo == Token.Variavel)
                {
                    //Segundo operando é variavel...
                    operando2 = contexto[this.operando2.token];
                    if (operando2 == null)
                        throw new ExecutionException("Variável " + this.operando2.token + " não encontrada no ambiente de execução");
                    if (!operando1.GetType().IsInstanceOfType(operando2))
                        throw new ExecutionException("As variáveis " + this.operando1.token + " e " + this.operando2.token + " são de tipos diferentes");
                }
                else
                {
                    //Segundo operando é constante...
                    switch (this.operando2.tipo)
                    {
                        case Token.ConstanteFlutuante:
                            operando2 = float.Parse(this.operando2.token);
                            break;
                        case Token.ConstanteInteira:
                            operando2 = int.Parse(this.operando2.token);
                            break;
                        case Token.ConstanteString:
                            operando2 = this.operando2.token;
                            if (!operando1.GetType().IsInstanceOfType(operando2))
                                throw new ExecutionException("A variável " + this.operando1.token + " e a constante " + this.operando2.token + " são de tipos diferentes");
                            break;
                    }
                }
                if (operador.token.Equals("="))
                {
                    return operando1.Equals(operando2);
                }
                if (operador.token.Equals("!="))
                    return !operando1.Equals(operando2);
                int tipoOperando2 = this.operando2.tipo;
                if (tipoOperando2 == Token.Variavel)
                {
                    if (operando2 is int)
                        tipoOperando2 = Token.ConstanteInteira;
                    else if (operando2 is float)
                        tipoOperando2 = Token.ConstanteFlutuante;
                    else if (operando2 is string)
                        tipoOperando2 = Token.ConstanteString;
                     else
                        throw new ExecutionException("Operando " + this.operando1.token + " não possui um tipo de dado válido");
                }
                if (operador.token.Equals("<"))
                    switch (tipoOperando2)
                    {
                        case Token.ConstanteFlutuante:
                            return (float)operando1 < (float)operando2;
                        case Token.ConstanteInteira:
                            return (int)operando1 < (int)operando2;
                        case Token.ConstanteString:
                            throw new ExecutionException("O operador < não pode ser aplicado em Strings");
                    }
                if (operador.token.Equals("<="))
                    switch (tipoOperando2)
                    {
                        case Token.ConstanteFlutuante:
                            return (float)operando1 <= (float)operando2;
                        case Token.ConstanteInteira:
                            return (int)operando1 <= (int)operando2;
                        case Token.ConstanteString:
                            throw new ExecutionException("O operador <= não pode ser aplicado em Strings");
                    }
                if (operador.token.Equals(">"))
                {
                    switch (tipoOperando2)
                    {
                        case Token.ConstanteFlutuante:
                            return (float)operando1 > (float)operando2;
                        case Token.ConstanteInteira:
                            return (int)operando1 > (int)operando2;
                        case Token.ConstanteString:
                            throw new ExecutionException("O operador > não pode ser aplicado em Strings");
                    }
                }
                if (operador.token.Equals(">="))
                    switch (tipoOperando2)
                    {
                        case Token.ConstanteFlutuante:
                            return (float)operando1 >= (float)operando2;
                        case Token.ConstanteInteira:
                            return (int)operando1 >= (int)operando2;
                        case Token.ConstanteString:
                            throw new ExecutionException("O operador < não pode ser aplicado em Strings");
                    }
                throw new ExecutionException("O motor de execução não conseguiu executar: " + this.operando1.token + this.operador.token + this.operando2.token);
            }
            catch (ExecutionException ex)
            {
                throw ex;
            }
        }
        public string toString()
        {
            if (operando2.tipo == 4)
                return operando1.token + " " + operador.token + " \"" + operando2.token + "\"";
            else
                return operando1.token + " " + operador.token + " " + operando2.token;
        }
    }
}
