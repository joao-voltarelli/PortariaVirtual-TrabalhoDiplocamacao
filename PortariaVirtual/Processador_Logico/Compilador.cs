using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessadorLogicoCSharp
{
    class ResultadoProcessamento
    {
        public int posicaoFinal;
        public ExpressaoLogica el;
    }

    /**
    * Essa classe representa um compilador de expressões lógicas. Ela permite trabalhar com
    * expressões lógicas envolvendo variávels e constantes dos tipos: Integer, Float e string.
    * As variáveis são especificadas em uma tabela Hash que tem a função de exibir o contexto
    * onde a execução está ocorrendo.
    * No momento de especificar uma expressão as variáveis devem ser precedidas pelo caracter @.
    * Exemplo:@x=5.
    * São aceitos os seguintes operadores condicionais: =, !=, <, >,<=, >= e os seguintes
    * operadores lógicos: and e or.
    * @author Fabiano Costa Teixeira (Doutorando-LASDPC-ICMC-USP)
    */

    public class Compilador
    {
        /**
        * Determina se um determinado token representa uma variável
        * @param token Token a ser analisado
        * @return true se foi reconhecido como variável
        */
        public bool variavel(string token)
        {
            int estado = 0; bool fim = false;
            string letras = "abcdefghijklmnopqrstuvxzwy";
            string numeros = "01234567890";
            string letra;
            bool flag;
            for (int i = 0; i < token.Length && !fim; i++)
            {
                letra = Substring.Java(token, i, i + 1);
                switch (estado)
                {
                    case 0:
                        if (letra.Equals("@"))
                            estado = 1;
                        else
                            fim = true;
                        break;
                    case 1:
                        for (int j = 0; j < letras.Length; j++)
                            if (letra.ToLower().Equals(Substring.Java(letras, j, j + 1)))
                                estado = 2;
                        if (estado == 1)
                            fim = true;
                        break;
                    case 2:
                        flag = false;
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                flag = true;
                        if (!flag)
                        {
                            for (int j = 0; j < letras.Length; j++)
                                if (letra.ToLower().Equals(Substring.Java(letras, j, j + 1)))
                                    flag = true;
                        }
                        if (!flag)
                            if (letra.Equals("_"))
                                estado = 3;
                            else
                            {
                                fim = true;
                                estado = -1;
                            }
                        break;
                    case 3:
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                estado = 2;
                        if (estado == 3)
                        {
                            for (int j = 0; j < letras.Length; j++)
                                if (letra.ToLower().Equals(Substring.Java(letras, j, j + 1)))
                                    estado = 2;
                        }
                        if (estado == 3)
                            fim = true;
                        break;
                }
            }
            if (estado == 2)
                return true;
            return false;
        }

        /**
        * Determina se um token representa uma constante de uma string
        * @param token Token a ser analisado
        * @return True se foi reconhecido como uma constante de uma string
        */
        public bool constanteString(string token)
        {
            int estado = 0;
            string letra;
            for (int i = 0; i < token.Length && estado != 3; i++)
            {
                letra = Substring.Java(token, i, i + 1);
                switch (estado)
                {
                    case 0:
                        if (letra.Equals("\""))
                            estado = 1;
                        else
                            estado = 3;
                        break;
                    case 1:
                        if (letra.Equals("\""))
                            estado = 2;
                        break;
                    case 2:
                        estado = 3;
                        break;
                }
            }
            if (estado == 2) return true;
            return false;
        }

        /**
        * Determina se um token representa uma constante inteira
        * @param token Token a ser analisado
        * @return True se foi reconhecido como uma constante inteira
        */
        public bool constanteInteira(string token)
        {
            int estado = 0;
            string numeros = "01234567890";
            string letra;
            bool flag;
            for (int i = 0; i < token.Length && estado != 1; i++)
            {
                letra = Substring.Java(token, i, i + 1);
                switch (estado)
                {
                    case 0:
                        flag = false;
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                flag = true;
                        if (!flag)
                            estado = 1;
                        break;
                }
            }
            if (estado == 0) return true;
            return false;
        }

        /**
        * Determina se um token representa uma constante ponto flutuante
        * @param token Token a ser analisado
        * @return True se foi reconhecido como uma constante ponto flutuante
        */
        public bool constanteFlutuante(string token)
        {
            int estado = 0;
            string numeros = "01234567890";
            string letra;
            bool flag;
            for (int i = 0; i < token.Length && estado != 3; i++)
            {
                letra = Substring.Java(token, i, i + 1);
                switch (estado)
                {
                    case 0:
                        flag = false;
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                flag = true;
                        if (!flag)
                            if (letra.Equals("."))
                                estado = 1;
                            else
                                estado = 3;
                        break;
                    case 1:
                        flag = false;
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                flag = true;
                        if (!flag)
                            estado = 3;
                        else
                            estado = 2;
                        break;
                    case 2:
                        flag = false;
                        for (int j = 0; j < numeros.Length; j++)
                            if (letra.Equals(Substring.Java(numeros, j, j + 1)))
                                flag = true;
                        if (!flag)
                            estado = 3;
                        break;

                }
            }
            if (estado == 2)
                return true;
            return false;
        }

        /**
        * Realiza as análises léxica e sintátiga de uma expressão lógica.
        * Se nenhum erro for entrado é gerada a expressão lógica no formato pronto
        * para ser executado, bastando apenas fornecer o contexto de execução
        * @param expressao Expressão lógica escrita pelo usuário
        * @return Expressão lógica pronta para ser executada
        * @throws CompilationException
        */
        public ExpressaoLogica compila(string expressao)
        {
            try
            {
                String token = "";
                int estado;

                //Análise léxica...
                List<Token> tokens = new List<Token>();
                String letra;
                int coluna = 1;
                bool isString = false;
                for (int i = 0; i < expressao.Length; i++)
                {
                    letra = Substring.Java(expressao, i, i + 1);
                    if (letra.Equals("\""))
                        isString = !isString;
                    if (letra.Equals(" "))
                    {
                        if (token.Length > 0)
                        {
                            Token t = new Token();
                            t.token = token;
                            t.coluna = coluna;
                            tokens.Add(t);
                        }
                        token = "";
                        coluna = i + 2;
                    }
                    else if (!isString && (letra.Equals("(") || letra.Equals(")") || letra.Equals("=") || letra.Equals(">") || letra.Equals("<") || letra.Equals("!")))
                    {
                        if (token.Length > 0)
                        {
                            Token t2 = new Token();
                            t2.token = token;
                            t2.coluna = coluna;
                            tokens.Add(t2);
                            coluna = i + 2;
                        }
                        token = letra;
                        if (letra.Equals("!"))
                        {
                            if (i < expressao.Length - 1)
                                if (Substring.Java(expressao, i + 1, i + 2).Equals("="))
                                {
                                    i++;
                                    token = "!=";
                                }

                        }
                        else if (letra.Equals("<"))
                        {
                            if (i < expressao.Length - 1)
                                if (Substring.Java(expressao, i + 1, i + 2).Equals("="))
                                {
                                    i++;
                                    token = "<=";
                                }
                        }
                        else if (letra.Equals(">"))
                        {
                            if (i < expressao.Length - 1)
                                if (Substring.Java(expressao, i + 1, i + 2).Equals("="))
                                {
                                    i++;
                                    token = ">=";
                                }
                        }
                        Token t = new Token();
                        t.token = token;
                        t.coluna = i + 1;
                        tokens.Add(t);
                        token = "";
                    }
                    else
                        token = token + letra;
                }
                if (token.Length > 0)
                {
                    Token t = new Token();
                    t.token = token;
                    t.coluna = coluna;
                    tokens.Add(t);
                }
                //Reconhecimento dos tokens...
                Token atual;
                for (int i = 0; i < tokens.Count; i++)
                {
                    atual = tokens[i];
                    token = atual.token;
                    if (token.Equals("("))
                        atual.tipo = Token.AbreParenteses;
                    else if (token.Equals(")"))
                        atual.tipo = Token.FechaParenteses;
                    else if (token.Equals("=") || token.Equals("!=") || token.Equals(">") || token.Equals(">=") || token.Equals("<") || token.Equals("<="))
                        atual.tipo = Token.OperadorCondicional;
                    else if (token.Equals("and") || token.Equals("or"))
                        atual.tipo = Token.OperadorLogico;
                    else
                    {
                        //Verifica se é variável...
                        if (variavel(token))
                            atual.tipo = Token.Variavel;
                        else if (constanteString(token))
                        {
                            atual.tipo = Token.ConstanteString;
                            //Retira as aspas...
                            atual.token = Substring.Java(atual.token, 1, atual.token.Length - 1);
                        }
                        else if (constanteInteira(token))
                            atual.tipo = Token.ConstanteInteira;
                        else if (constanteFlutuante(token))
                            atual.tipo = Token.ConstanteFlutuante;
                    }
                }
                bool erro = false;
                String mensagemErro = "";
                for (int i = 0; i < tokens.Count; i++)
                {
                    //            System.out.println("Token: " + tokens.get(i).token + " Tipo: " + tokens.get(i).tipo);
                    if (tokens[i].tipo == 0)
                    {
                        erro = true;
                        mensagemErro = mensagemErro + "Token (" + tokens[i].token + ") não reconhecido na coluna " + tokens[i].coluna + ";\n";
                    }
                }
                if (erro)
                    throw new CompilationException(mensagemErro);
                estado = 0;
                bool sair = false;
                //Análise sintática...
                int abrep = 0;
                for (int i = 0; i < tokens.Count && !sair; i++)
                {
                    atual = tokens[i];
                    switch (estado)
                    {
                        case 0:
                            if (atual.tipo == Token.AbreParenteses)
                                estado = 1;
                            else if (atual.tipo == Token.Variavel)
                                estado = 2;
                            else
                            {
                                sair = true;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperado: ( ou variável";
                            }
                            break;
                        case 1:
                            abrep++;
                            if (atual.tipo == Token.Variavel)
                                estado = 2;
                            else
                                if (atual.tipo != Token.AbreParenteses)
                            {
                                sair = true;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperado Variável ou (";
                            }
                            break;
                        case 2:
                            if (atual.tipo == Token.OperadorCondicional)
                                estado = 3;
                            else
                            {
                                sair = true;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperado um operador condicional";
                            }
                            break;
                        case 3:
                            if (atual.tipo == Token.Variavel || atual.tipo == Token.ConstanteFlutuante || atual.tipo == Token.ConstanteInteira || atual.tipo == Token.ConstanteString)
                                estado = 4;
                            else
                            {
                                sair = true;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperada uma variável ou uma constante";
                            }
                            break;
                        case 4:
                            if (atual.tipo == Token.OperadorLogico)
                                estado = 5;
                            else if (atual.tipo == Token.FechaParenteses)
                                estado = 6;
                            else
                            {
                                sair = true;
                                estado = 0;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperado um operador lógico ou um )";
                            }
                            break;
                        case 5:
                            if (atual.tipo == Token.Variavel)
                                estado = 2;
                            else if (atual.tipo == Token.AbreParenteses)
                                estado = 1;
                            else
                            {
                                sair = true;
                                mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperada uma variável ou um (";
                            }
                            break;
                        case 6:
                            if (abrep == 0)
                            {
                                sair = true;
                                estado = 0;
                                mensagemErro = ") inesperado";
                            }
                            else
                            {
                                abrep--;
                                if (atual.tipo == Token.OperadorLogico)
                                    estado = 5;
                                else
                                    if (atual.tipo != Token.FechaParenteses)
                                {
                                    estado = 0;
                                    sair = true;
                                    mensagemErro = "Coluna " + atual.coluna + "(" + atual.token + "): Era esperado um operador lógico ou um )";
                                }
                            }
                            break;
                    }
                }
                if (estado == 6)
                    if (abrep == 0)
                    {
                        estado = 0;
                        mensagemErro = ") inesperado";
                    }
                    else
                        abrep--;
                if (abrep > 0)
                {
                    estado = 0;
                    mensagemErro = "Era esperado um )";
                }
                //        System.out.println("Estado final: " + estado);
                if (estado != 4 && estado != 6)
                {
                    if (mensagemErro.Length == 0)
                    {
                        switch (estado)
                        {
                            case 1:
                                mensagemErro = "Era esperado um ( ou uma variavel";
                                break;
                            case 2:
                                mensagemErro = "Era esperado um operador condicional";
                                break;
                            case 3:
                                mensagemErro = "Era esperada uma variável ou uma constante";
                                break;
                            case 5:
                                mensagemErro = "Era esperada uma variável";
                                break;
                            default:
                                mensagemErro = "Erro desconhecido";
                                break;
                        }
                    }
                    throw new CompilationException(mensagemErro);
                }
                else
                {
                    //Análise léxica e sintática ocorreu perfeitamente. Geração do expressão lógica;
                    ResultadoProcessamento rp = geraExpressaoLogica(tokens, 0);
                    return rp.el;
                }
            }
            catch (CompilationException ex)
            {
                throw ex;
            }
        }

        /**
        * Gera uma expressão lógica com base em um conjunto de tokens.
        * A geração é feita iniciando-se a partir de um determinado ponto do conjunto de tokens até
        * encontrar o símbolo de fecha parenteses.
        * Cada conjunto de tokens delimitados por parentêses formam uma expressão lógica independente.
        * Sendo assim, uma mesma expressão escrita por um usuário pode ser composta recursivamente
        * por diversas expressões lógicas geradas por esse método.
        * @param tokens Conjunto de tokens
        * @param posicao Posição inicial para análise do conjunto de tokens
        * @return Retorna a expressão gerada e o ponto final processado (pode ser o fim da expressão
        * ou um fecha parenteses
        */
        private ResultadoProcessamento geraExpressaoLogica(List<Token> tokens, int posicao)
        {
            ResultadoProcessamento retorno = new ResultadoProcessamento();
            ExpressaoLogica el = new ExpressaoLogica();
            retorno.el = el;
            Token atual;
            for (int i = posicao; i < tokens.Count; i++)
            {
                atual = tokens[i];
                if (atual.tipo == Token.AbreParenteses)
                {
                    ResultadoProcessamento tmp = geraExpressaoLogica(tokens, i + 1);
                    el.adicionaOperacao(tmp.el);
                    i = tmp.posicaoFinal;
                }
                else if (atual.tipo == Token.Variavel)
                {
                    OperacaoLogica ol = new OperacaoLogica();
                    ol.setOperando1(atual);
                    ol.setOperador(tokens[i + 1]);
                    ol.setOperando2(tokens[i + 2]);
                    el.adicionaOperacao(ol);
                    i = i + 2;
                }
                else if (atual.tipo == Token.OperadorLogico)
                {
                    el.adicionaOperadorLogico(atual.token);
                }
                else if (atual.tipo == Token.FechaParenteses)
                {
                    retorno.posicaoFinal = i;
                    i = tokens.Count; //Quebra o laço....
                }
            }
            return retorno;
        }
    }
}
