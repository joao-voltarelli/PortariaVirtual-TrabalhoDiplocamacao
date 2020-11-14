using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LeitorXML;
using PortariaVirtual.Processador_Logico;

namespace PortariaVirtual
{
    class Bot
    {
        public static int estado = 0;
        public static int timeout = 0;
        public static Boolean controle_interacao = true;
        public static string scoreIntent = null;
        public static double valorScore;
        public static string valorEntidadeReconhecida = "";
        public static string tipoEntidadeReconhecida = "";
        public static int solicitacao = 0;
        public static Boolean autorizacao_morador;
        public static int interacao_morador = 0;
        public static int tipo_requisicao;

        public static string resposta_final;

        public static SpeechConfig configSpeech;
        public static SpeechConfig configLuis;

        //ATRIBUTOS METODO RECOGNIZE
        public static IntentRecognizer recognizer;
        public static LanguageUnderstandingModel model;

        //ATRIBUTOS METODO RESPOSTA
        public static SpeechSynthesizer synthesizer;

        //ATRIBUTO LEITOR DO ARQUIVO XML
        public static Leitor leitorXML;

        //ATRIBUTO DO PROCESSADOR LOGICO
        public static Processador processador;

        public static async Task RecognizeIntentAsync()
        {
            if (solicitacao == 0)
            {
                Resposta("Pois não!").Wait();
                solicitacao = 1;
            }

            Console.WriteLine("Escutando...");

            // Starts recognizing.
            // Starts intent recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result. 
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query. 
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.

            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            //SALVANDO O JSON DOS RESULTADOS EM UM ARQUIVO
            var json = result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
            Console.WriteLine(json);

            string dados = result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
            StreamWriter salvar = new StreamWriter(@"dados-json.json");
            salvar.WriteLine(dados);
            salvar.Close();

            //LENDO O JSON PARA RECUPERAR O SCORE DA INTENT
            var arquivo2 = File.ReadAllText(@"dados-json.json");
            var arquivoLido2 = JsonConvert.DeserializeObject<Intents>(arquivo2);

            scoreIntent = arquivoLido2.topScoringIntent.score;
            valorScore = float.Parse(scoreIntent, CultureInfo.InvariantCulture.NumberFormat);

            Console.WriteLine("Score da intent = " + valorScore);

            // Checks result.
            if (result.Reason == ResultReason.RecognizedIntent && valorScore > 0.6)
            {
                //LENDO O JSON PARA RECUPERAR VALOR E TIPO DA ENTIDADE
                var arquivo = File.ReadAllText(@"dados-json.json");
                var arquivoLido = JsonConvert.DeserializeObject<Entidades>(arquivo);

                foreach (EntidadeUnica entidade in arquivoLido.entities)
                {
                    valorEntidadeReconhecida = entidade.entity;
                    tipoEntidadeReconhecida = entidade.type;
                }

                if (interacao_morador == 0)
                {
                    switch (estado)
                    {
                        case 0: //ESTADO = 0 - INICIO DA INTERAÇÃO

                            switch (result.IntentId)
                            {
                                case "cumprimento":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_cumprimento.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_0.case_cumprimento.se.frase).Wait();
                                        estado = leitorXML.regras.estado_0.case_cumprimento.se.prox_estado;
                                    }
                                    else
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_0.case_cumprimento.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_0.case_cumprimento.prox_estado;
                                    }
                                    break;

                                case "entregar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_entrega.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_0.case_entrega.se.frase).Wait();
                                        estado = leitorXML.regras.estado_0.case_entrega.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_entrega.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_0.case_entrega.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_0.case_entrega.senao.prox_estado_se;
                                        }
                                        else if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_entrega.senao.regra_senao))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_0.case_entrega.senao.frase_senao, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_0.case_entrega.senao.prox_estado_senao;
                                        }
                                    }
                                    break;

                                case "visitar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_visita.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_0.case_visita.se.frase).Wait();
                                        estado = leitorXML.regras.estado_0.case_visita.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_visita.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_0.case_visita.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_0.case_visita.senao.prox_estado_se;
                                        }
                                        else if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_visita.senao.regra_senao))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_0.case_visita.senao.frase_senao, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_0.case_visita.senao.prox_estado_senao;
                                        }
                                    }

                                    break;

                                case "avisar_chegada":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_chegada.se.regra_se))
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_0.case_chegada.se.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_0.case_chegada.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_0.case_chegada.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_0.case_chegada.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_0.case_chegada.senao.prox_estado_se;
                                        }
                                    }
                                    break;

                                default:
                                    Resposta(leitorXML.regras.estado_0.case_default.frase).Wait();
                                    estado = leitorXML.regras.estado_0.case_default.prox_estado;
                                    break;
                            }
                            break;

                        case 1: //ESTADO = 1 - PESSOA CUMPRIMENTOU E JÁ FOI CUMPRIMENTADA 

                            switch (result.IntentId)
                            {

                                case "cumprimento":
                                    Resposta(leitorXML.regras.estado_1.case_cumprimento.frase).Wait();
                                    estado = leitorXML.regras.estado_1.case_cumprimento.prox_estado;
                                    break;

                                case "entregar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_entrega.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_1.case_entrega.se.frase).Wait();
                                        estado = leitorXML.regras.estado_1.case_entrega.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_entrega.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_1.case_entrega.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_1.case_entrega.senao.prox_estado_se;
                                        }
                                        else
                                        {
                                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_entrega.senao.regra_senao))
                                            {
                                                resposta_final = string.Format(leitorXML.regras.estado_1.case_entrega.senao.frase_senao, valorEntidadeReconhecida);
                                                Resposta(resposta_final).Wait();
                                                estado = leitorXML.regras.estado_1.case_entrega.senao.prox_estado_senao;
                                            }
                                        }
                                    }
                                    break;

                                case "visitar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_visita.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_1.case_visita.se.frase).Wait();
                                        estado = leitorXML.regras.estado_1.case_visita.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_visita.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_1.case_visita.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_1.case_visita.senao.prox_estado_se;
                                        }
                                        else if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_visita.senao.regra_senao))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_1.case_visita.senao.frase_senao, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_1.case_visita.senao.prox_estado_senao;
                                        }

                                    }
                                    break;

                                case "avisar_chegada":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_chegada.se.regra_se))
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_1.case_chegada.se.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_1.case_chegada.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_1.case_chegada.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_1.case_chegada.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_1.case_chegada.senao.prox_estado_se;
                                        }
                                    }
                                    break;

                                case "despedida":
                                    estado = leitorXML.regras.estado_1.case_despedida.prox_estado;
                                    break;

                                default:
                                    resposta_final = string.Format(leitorXML.regras.estado_1.case_default.frase, valorEntidadeReconhecida);
                                    Resposta(resposta_final).Wait();
                                    estado = leitorXML.regras.estado_1.case_default.prox_estado;
                                    break;
                            }
                            break;

                        case 5: //ESTADO = 5 - PESSOA JÁ FOI RESPONDIDA COM O CUMPRIMENTO PELA SEGUNDA VEZ

                            switch (result.IntentId)
                            {

                                case "cumprimento":
                                    Resposta(leitorXML.regras.estado_5.case_cumprimento.frase).Wait();
                                    estado = leitorXML.regras.estado_5.case_cumprimento.prox_estado;
                                    break;

                                case "entregar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_entrega.se.regra_se))
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_5.case_entrega.se.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_5.case_entrega.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_entrega.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_5.case_entrega.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_5.case_entrega.senao.prox_estado_se;
                                        }
                                        else
                                        {
                                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_entrega.senao.regra_senao))
                                            {
                                                resposta_final = string.Format(leitorXML.regras.estado_5.case_entrega.senao.frase_senao, valorEntidadeReconhecida);
                                                Resposta(resposta_final).Wait();
                                                estado = leitorXML.regras.estado_5.case_entrega.senao.prox_estado_senao;
                                            }
                                        }
                                    }
                                    break;

                                case "visitar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_visita.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_5.case_visita.se.frase).Wait();
                                        estado = leitorXML.regras.estado_5.case_visita.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_visita.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_5.case_visita.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_5.case_visita.senao.prox_estado_se;
                                        }
                                        else if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_visita.senao.regra_senao))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_5.case_visita.senao.frase_senao, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_5.case_visita.senao.prox_estado_senao;
                                        }
                                    }
                                    break;

                                case "avisar_chegada":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_chegada.se.regra_se))
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_5.case_chegada.se.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_5.case_chegada.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_5.case_chegada.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_5.case_chegada.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_5.case_chegada.senao.prox_estado_se;
                                        }
                                    }
                                    break;

                                case "despedida":
                                    estado = leitorXML.regras.estado_5.case_despedida.prox_estado;
                                    break;

                                default:
                                    Resposta(leitorXML.regras.estado_5.case_default.frase).Wait();
                                    estado = leitorXML.regras.estado_5.case_default.prox_estado;
                                    break;
                            }
                            break;

                        case 7: //ESTADO = 7 - ENTREGADOR INFORMOU PARA QUEM É A ENTREGA

                            switch (result.IntentId)
                            {
                                case "destino":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_7.case_destino.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_7.case_destino.se.frase).Wait();
                                        estado = leitorXML.regras.estado_7.case_destino.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_7.case_destino.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_7.case_destino.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_7.case_destino.senao.prox_estado_se;
                                        }
                                        else
                                        {
                                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_7.case_destino.senao.regra_senao))
                                            {
                                                resposta_final = string.Format(leitorXML.regras.estado_7.case_destino.senao.frase_senao, valorEntidadeReconhecida);
                                                Resposta(resposta_final).Wait();
                                                estado = leitorXML.regras.estado_7.case_destino.senao.prox_estado_senao;
                                            }
                                        }
                                    }
                                    break;

                                case "entregar":
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_7.case_solicitacao.se.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_7.case_solicitacao.se.frase, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_7.case_solicitacao.se.prox_estado;
                                        }
                                        else
                                        {
                                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_7.case_solicitacao.senao.regra_se))
                                            {
                                                resposta_final = string.Format(leitorXML.regras.estado_7.case_solicitacao.senao.frase_se, valorEntidadeReconhecida);
                                                Resposta(resposta_final).Wait();
                                                estado = leitorXML.regras.estado_7.case_solicitacao.senao.prox_estado_se;
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    Resposta(leitorXML.regras.estado_7.case_default.frase).Wait();
                                    estado = leitorXML.regras.estado_7.case_default.prox_estado;
                                    break;
                            }
                            break;

                        case 8: //ESTADO = 8 - VISITANTE SE IDENTIFICOU PARA FAZER UMA SOLICITAÇÃO DE AVISO DE CHEGADA

                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_8.se.regra_se))
                            {
                                Resposta(leitorXML.regras.estado_8.se.frase).Wait();
                                estado = leitorXML.regras.estado_8.se.prox_estado;
                            }
                            else
                            {
                                Resposta(leitorXML.regras.estado_8.frase).Wait();
                                estado = leitorXML.regras.estado_8.prox_estado;
                            }
                            break;

                        case 9: //ESTADO = 9 - VISITANTE SE IDENTIFICOU PARA FAZER UMA SOLICITAÇÃO DE VISITA

                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_9.se.regra_se))
                            {
                                Resposta(leitorXML.regras.estado_9.se.frase).Wait();
                                estado = leitorXML.regras.estado_9.se.prox_estado;
                            }
                            else
                            {
                                Resposta(leitorXML.regras.estado_9.frase).Wait();
                                estado = leitorXML.regras.estado_9.prox_estado;
                            }
                            break;

                        case 10: //ESTADO = 10 - PESSOA INFORMOU PARA QUEM É A VISITA

                            switch (result.IntentId)
                            {
                                case "destino":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_10.case_destino.se.regra_se))
                                    {
                                        Resposta(leitorXML.regras.estado_10.case_destino.se.frase).Wait();
                                        estado = leitorXML.regras.estado_10.case_destino.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_10.case_destino.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_10.case_destino.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_10.case_destino.senao.prox_estado_se;
                                        }
                                        else
                                        {
                                            if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_10.case_destino.senao.regra_senao))
                                            {
                                                resposta_final = string.Format(leitorXML.regras.estado_10.case_destino.senao.frase_senao, valorEntidadeReconhecida);
                                                Resposta(resposta_final).Wait();
                                                estado = leitorXML.regras.estado_10.case_destino.senao.prox_estado_senao;
                                            }
                                        }
                                    }
                                    break;

                                case "visitar":
                                    if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_10.case_solicitacao.se.regra_se))
                                    {
                                        resposta_final = string.Format(leitorXML.regras.estado_10.case_solicitacao.se.frase, valorEntidadeReconhecida);
                                        Resposta(resposta_final).Wait();
                                        estado = leitorXML.regras.estado_10.case_solicitacao.se.prox_estado;
                                    }
                                    else
                                    {
                                        if (processador.Compila(tipoEntidadeReconhecida, leitorXML.regras.estado_10.case_solicitacao.senao.regra_se))
                                        {
                                            resposta_final = string.Format(leitorXML.regras.estado_10.case_solicitacao.senao.frase_se, valorEntidadeReconhecida);
                                            Resposta(resposta_final).Wait();
                                            estado = leitorXML.regras.estado_10.case_solicitacao.senao.prox_estado_se;
                                        }
                                    }
                                    break;

                                default:
                                    Resposta(leitorXML.regras.estado_10.case_default.frase).Wait();
                                    estado = leitorXML.regras.estado_10.case_default.prox_estado;
                                    break;
                            }
                            break;
                    }

                }
                else if (interacao_morador == 1) //INTERACAO COM O MORADOR
                {
                    switch (tipo_requisicao)
                    {
                        case 2:
                            switch (result.IntentId)
                            {
                                case "autorizado":
                                    Resposta("Ok, então irei autorizar a subida do entregador. Obrigado").Wait();
                                    autorizacao_morador = true;
                                    break;

                                case "nao_autorizado":
                                    Resposta("Ok, irei avisar o entregador que ele não está autorizado a subir. Obrigado").Wait();
                                    autorizacao_morador = false;
                                    break;

                                default:
                                    Resposta("Desculpe, não entendi. Posso autorizar a subida do entregador?").Wait();
                                    tipo_requisicao = 2;
                                    break;
                            }
                            break;

                        case 3:
                            switch (result.IntentId)
                            {
                                case "autorizado":
                                    Resposta("Ok, então irei autorizar a subida do visitante. Obrigado").Wait();
                                    autorizacao_morador = true;
                                    break;

                                case "nao_autorizado":
                                    Resposta("Ok, irei avisar o visitante que ele não está autorizado a subir. Obrigado").Wait();
                                    autorizacao_morador = false;
                                    break;

                                default:
                                    Resposta("Desculpe, não entendi. Posso autorizar a subida do visitante?").Wait();
                                    tipo_requisicao = 3;
                                    break;
                            }
                            break;

                        case 4:
                            switch (result.IntentId)
                            {
                                case "autorizado":
                                    Resposta("Ok, então irei avisar o visitante que logo você estará descendo. Obrigado").Wait();
                                    autorizacao_morador = true;
                                    break;

                                case "nao_autorizado":
                                    Resposta("Ok, irei avisar o visitante que você não irá descer agora. Obrigado").Wait();
                                    autorizacao_morador = false;
                                    break;

                                default:
                                    Resposta("Desculpe, não entendi. Posso informar o visitante que você irá descer?").Wait();
                                    tipo_requisicao = 4;
                                    break;
                            }
                            break;
                    }
                }
            }
            else if (valorScore < 0.6 && controle_interacao == true)
            {
                Resposta("Desculpe, não entendi. Pode falar novamente?").Wait();
            }
            else if (result.Reason == ResultReason.RecognizedSpeech)
            {
                //MessageBox.Show($"RECOGNIZED: Text={result.Text}");
                //MessageBox.Show($"    Intent not recognized.");
                Resposta("Desculpe, não entendi. Pode falar novamente?").Wait();
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                //MessageBox.Show($"NOMATCH: Speech could not be recognized.");
                Resposta("Desculpe, não entendi. Pode falar novamente?").Wait();
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                //MessageBox.Show($"CANCELED: Reason={cancellation.Reason}");
                controle_interacao = false;

                if (cancellation.Reason == CancellationReason.Error)
                {
                    //MessageBox.Show($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    //MessageBox.Show($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    //MessageBox.Show($"CANCELED: Did you update the subscription info?");
                    controle_interacao = false;
                }
            }
        }

        public static async Task Resposta(String text)
        {
            using (var result = await synthesizer.SpeakTextAsync(text))
            {
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    //Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

        public static void ligacao_morador()
        {
            Resposta("Ligando para o morador").Wait();
            interacao_morador = 1;
            tipo_requisicao = estado;

            if (tipo_requisicao == 2)
            {
                Resposta("Olá, tudo bem? Foi requisitado uma entrega para você, posso mandar subir?").Wait();
            }
            else if (tipo_requisicao == 3)
            {
                Resposta("Olá, tudo bem? O " + valorEntidadeReconhecida + " solicitou uma visita para você, posso mandar subir?").Wait();
            }
            else if (tipo_requisicao == 4)
            {
                Resposta("Olá, tudo bem? O " + valorEntidadeReconhecida + " pediu para avisar que já chegou e está te esperando. Posso avisar que já vai descer?").Wait();
            }

            RecognizeIntentAsync().Wait();
            Resposta("Retornando para o visitante").Wait();

            if (autorizacao_morador == true)
            {
                if (tipo_requisicao == 2)
                {
                    Resposta("Tudo certo, a sua solicitação de entrega foi confirmada. O portão já está sendo aberto. Espero ter ajudado, até mais!").Wait();
                }
                else if (tipo_requisicao == 3)
                {
                    Resposta("Tudo certo, a sua solicitação de visita foi confirmada. O portão já está sendo aberto. Espero ter ajudado, até mais!").Wait();
                }
                else if (tipo_requisicao == 4)
                {
                    Resposta("Tudo certo, o morador já foi avisado da sua chegada e logo estará descendo. Espero ter ajudado, até mais!").Wait();
                }
            }
            else
            {
                if (tipo_requisicao == 2)
                {
                    Resposta("Desculpe, mas o morador não autorizou a solicitação de entrega. Espero ter ajudado, até mais!").Wait();
                }
                else if (tipo_requisicao == 3)
                {
                    Resposta("Desculpe, mas o morador não autorizou a solicitação de visita. Espero ter ajudado, até mais!").Wait();
                }
                else if (tipo_requisicao == 4)
                {
                    Resposta("Desculpe, mas o morador avisou que não irá descer agora. Espero ter ajudado, até mais!").Wait();
                }
            }
        }

        public static void despedida()
        {
            Resposta("Obrigado por falar comigo!. Espero ter ajudado, tchau e até mais!").Wait();
            controle_interacao = false;
        }

        //CLASSES PARA MAPEAR O JSON E RECUPERAR OS VALORES DAS ENTIDADES RECONHECIDAS
        public class ScoreIntent
        {
            public string intent { get; set; }
            public string score { get; set; }
        }

        public class Intents
        {
            public ScoreIntent topScoringIntent { get; set; }
        }

        public class EntidadeUnica
        {
            public string entity { get; set; }
            public string type { get; set; }
        }

        public class Entidades
        {
            public List<EntidadeUnica> entities { get; set; }
        }

        public static void Main(string []args)
        {
            //CONFIG LUIS
            configLuis = SpeechConfig.FromSubscription("b1ea9c02d3954e9f9cf90592f9062ef6", "westus2");
            configLuis.SpeechRecognitionLanguage = "pt-BR";

            //CONFIG SPEECH
            configSpeech = SpeechConfig.FromSubscription("9184b21db9b743bd83770061b376ee2c", "westus2");
            configSpeech.SpeechSynthesisVoiceName = "Microsoft Server Speech Text to Speech Voice (pt-BR, FranciscaNeural)";

            //ADICIONANDO AS INTENCOES CRIADAS NO LUIS
            recognizer = new IntentRecognizer(configLuis);
            model = LanguageUnderstandingModel.FromAppId("2940577d-15f2-42bb-a07f-a760cd651c0b");
            recognizer.AddIntent(model, "AvisarChegada", "avisar_chegada");
            recognizer.AddIntent(model, "Cumprimento", "cumprimento");
            recognizer.AddIntent(model, "Despedida", "despedida");
            recognizer.AddIntent(model, "Entregar", "entregar");
            recognizer.AddIntent(model, "Visita", "visitar");
            recognizer.AddIntent(model, "InformarDestino", "destino");
            recognizer.AddIntent(model, "AutorizarVisitante", "autorizado");
            recognizer.AddIntent(model, "NaoAutorizarVisitante", "nao_autorizado");

            synthesizer = new SpeechSynthesizer(configSpeech);

            //LÊ AS REGRAS DEFINIDAS NO ARQUIVO XML
            leitorXML = new Leitor();
            leitorXML.LeArquivoXML();

            //VALIDA AS VARIAVEIS E REGRAS LIDAS NO ARQUIVO ATRAVES DO PROCESSADOR LOGICO
            processador = new Processador();

            while (controle_interacao)
            {
                try
                {
                    if (estado == 2 || estado == 3 || estado == 4)
                    {
                        controle_interacao = false;
                        ligacao_morador();
                    }
                    else if (estado == 6)
                    {
                        despedida();
                    }
                    else
                        RecognizeIntentAsync().Wait();
                }
                catch
                {
                    if (timeout == 0)
                        Resposta("Olá, tem alguém me escutando?").Wait();

                    else if (timeout == 1)
                        Resposta("Olá, ainda tem alguém aí?").Wait();

                    else if (timeout == 2)
                    {
                        Resposta("Encerrando a chamada").Wait();
                        controle_interacao = false;
                    }

                    timeout++;
                }
            }
        }
    }
}
