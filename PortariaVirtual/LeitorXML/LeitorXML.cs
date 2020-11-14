using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeitorXML
{
    [XmlRoot(ElementName = "regras")]
    public class Regras
    {
        public Estado_0 estado_0 { get; set; }
        public Estado estado_1 { get; set; }
        public Estado estado_5 { get; set; }
        public EstadosDestino estado_7 { get; set; }
        public EstadosInformar estado_8 { get; set; }
        public EstadosInformar estado_9 { get; set; }
        public EstadosDestino estado_10 { get; set; }
    }

    //ESTRUTURA COMPLETA DO ESTADO 0
    public class Estado_0
    {
        public Case_Cumprimento_Estado_0 case_cumprimento { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_entrega { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_visita { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_chegada { get; set; }

        public Case_Default case_default { get; set; }
    }

    //ESTRUTURA COMPLETA DOS ESTADOS 1, 5
    public class Estado
    {
        public Case_Cumprimento case_cumprimento { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_entrega { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_visita { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_chegada { get; set; }

        public Case_Despedida case_despedida { get; set; }

        public Case_Default case_default { get; set; }
    }

    //ESTRUTURA COMPLETA DOS ESTADOS 7, 10
    public class EstadosDestino
    {
        public Case_Entregar_Visitar_Chegada_Destino case_destino { get; set; }

        public Case_Entregar_Visitar_Chegada_Destino case_solicitacao { get; set; }

        public Case_Default case_default { get; set; }
    }

    //ESTRUTURA COMPLETA DOS ESTADOS 8, 9
    public class EstadosInformar
    {
        public Se se { get; set; }

        public string frase { get; set; }

        public int prox_estado { get; set; }
    }

    //ESTRUTURA DO CASE CUMPRIMENTO UTILIZADO NO ESTADO 0
    public class Case_Cumprimento_Estado_0
    {
        public Se se { get; set; }

        public string frase { get; set; }

        public int prox_estado { get; set; }
    }

    //ESTRUTURA DO CASE CUMPRIMENTO UTILIZADO NOS ESTADOS 1, 5
    public class Case_Cumprimento
    {
        public string frase { get; set; }

        public int prox_estado { get; set; }
    }

    public class Case_Entregar_Visitar_Chegada_Destino
    {
        public Se se { get; set; }

        public Senao senao { get; set; }
    }

    public class Case_Despedida
    {
        public int prox_estado { get; set; }
    }

    public class Case_Default
    {
        public string frase { get; set; }
        public int prox_estado { get; set; }
    }

    public class Se
    {
        public string regra_se { get; set; }

        public string frase { get; set; }

        public int prox_estado { get; set; }
    }

    public class Senao
    {
        public string regra_se { get; set; }

        public string frase_se { get; set; }

        public int prox_estado_se { get; set; }

        public string regra_senao { get; set; }

        public string frase_senao { get; set; }

        public int prox_estado_senao { get; set; }
    }

    public class Leitor
    {
        public Regras regras;
        public void LeArquivoXML()
        {
            var serializer = new XmlSerializer(typeof(Regras));
            var configuracao = new Regras();
            var localArquivo = @"regras.xml";

            StreamReader textReader = new StreamReader(localArquivo);
            regras = (Regras)serializer.Deserialize(textReader);
            textReader.Close();
        }
    }
}
