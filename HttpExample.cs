using System.Collections.Generic;
using System.Net;
using System.Xml;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class HttpExample
    {
        private string _NombreAreaActual;
        private long _CantNodosArea;
        private long _CantEmpleadosNodoActual;
        private long _CantNodosMasDe2Empleados;
        private long _SumaSalarios;
        private string _SalariosPorArea;


        public HttpExample()
        {
            _NombreAreaActual = "";
            _SalariosPorArea = "";
            _CantNodosArea = 0;
            _CantNodosMasDe2Empleados = 0;
            _CantEmpleadosNodoActual = 0;
        }

        [Function("HttpExample")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //Leer desde request
            string requestBody = new StreamReader(req.Body).ReadToEnd();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestBody);
            XmlNode node = doc as XmlNode;
            ProcesarNodo(node);


            response.WriteString(String.Format("Se encontraron {0} nodos del tipo <area>" + Environment.NewLine , _CantNodosArea));
            response.WriteString(String.Format("Se encontraron {0} nodos del tipo <area> con más de 2 empleados" + Environment.NewLine, _CantNodosMasDe2Empleados));
            response.WriteString(_SalariosPorArea);

            return response;
        }

        public void ProcesarNodo(XmlNode nodo)
        {
            //Establecer valores según nodo actual
            if (nodo.Name == "area")
            {               
                _CantNodosArea++;
                _CantEmpleadosNodoActual = 0;
            };

            if (nodo.Name == "name")
            {
                _NombreAreaActual = nodo.InnerText;
                _SumaSalarios = 0;
            };

            if (nodo.Name == "employee")
            {
                _CantEmpleadosNodoActual++;
                _SumaSalarios += int.Parse(nodo.Attributes["salary"].Value.Replace(".00", ""));
            };

            //Procesar nodos hijos
            if (nodo.HasChildNodes)
            {
                foreach (XmlNode nodoHijo in nodo.ChildNodes)
                {
                    ProcesarNodo(nodoHijo);
                }
            }

            //Establecer valores según resultados procesamiento de nodos hijos
            if (nodo.Name == "area")
            {
                if (_CantEmpleadosNodoActual > 2)
                {
                    _CantNodosMasDe2Empleados++;
                };

                _SalariosPorArea += (String.Format("{0} | {1}" + Environment.NewLine, _NombreAreaActual, _SumaSalarios));
            };


        }

    }
}
