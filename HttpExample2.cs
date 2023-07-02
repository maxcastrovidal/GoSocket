using System;
using System.Net;
using Azure;
using System.Xml;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class HttpExample2
    {
        private string _NombreAreaActual;
        private long _CantNodosArea;
        private long _CantEmpleadosNodoActual;
        private long _CantNodosMasDe2Empleados;
        private long _SumaSalarios;
        private string _SalariosPorArea;

        public HttpExample2()
        {
            _NombreAreaActual = "";
            _SalariosPorArea = "";
            _CantNodosArea = 0;
            _CantNodosMasDe2Empleados = 0;
            _CantEmpleadosNodoActual = 0;
        }

        [Function("HttpExample2")]
        public void Run([TimerTrigger("0 * * * * *")] MyInfo myTimer)
        {
            //Leer desde variable
            String requestBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<gosocket>\r\n  <area>\r\n    <name>Producto</name>\r\n\t<description>Descripción del área de producto</description>\r\n    <employees>\r\n\t\t<employee name=\"Empleado 1\" salary=\"1000.00\" jobTitle=\"Ingeniero Producto\" />\r\n\t\t<employee name=\"Empleado 2\" salary=\"1300.00\" jobTitle=\"Ingeniero Producto\" />\r\n\t</employees>\r\n  </area>\r\n  <area>\r\n    <name>Tecnología</name>\r\n\t<description>Descripción del área de tecnología</description>\r\n    <employees>\r\n\t\t<employee name=\"Empleado 3\" salary=\"1100.00\" jobTitle=\"Desarrollador Jr\" />\r\n\t\t<employee name=\"Empleado 4\" salary=\"1200.00\" jobTitle=\"Desarrollador Semi-Senior\" />\r\n\t\t<employee name=\"Empleado 5\" salary=\"1300.00\" jobTitle=\"Desarrollador Sr\" />\r\n\t</employees>\r\n  </area>\r\n  <area>\r\n    <name>Consultoría</name>\r\n\t<description>Descripción del área de consultoría</description>\r\n    <employees>\r\n\t\t<employee name=\"Empleado 6\" salary=\"1000.00\" jobTitle=\"Consultor Jr\" />\r\n\t\t<employee name=\"Empleado 7\" salary=\"1150.00\" jobTitle=\"Consultor Semi-Senior\" />\r\n\t\t<employee name=\"Empleado 8\" salary=\"1250.00\" jobTitle=\"Consultor Sr\" />\r\n\t\t<employee name=\"Empleado 9\" salary=\"1250.00\" jobTitle=\"Consultor Sr\" />\r\n\t</employees>\r\n  </area>\r\n  <area>\r\n    <name>Soporte</name>\r\n\t<description>Descripción del área de soporte</description>\r\n    <employees>\r\n\t\t<employee name=\"Empleado 10\" salary=\"700.00\" jobTitle=\"Ingeniero Soporte Jr\" />\r\n\t\t<employee name=\"Empleado 11\" salary=\"750.00\" jobTitle=\"Ingeniero Soporte Jr\" />\r\n\t\t<employee name=\"Empleado 12\" salary=\"800.00\" jobTitle=\"Ingeniero Soporte Sr\" />\r\n\t\t<employee name=\"Empleado 13\" salary=\"850.00\" jobTitle=\"Ingeniero Soporte Sr\" />\r\n\t</employees>\r\n  </area>\r\n</gosocket>\r\n";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestBody);
            XmlNode node = doc as XmlNode;
            ProcesarNodo(node);

            string respuesta = "";
            respuesta += String.Format("Se encontraron {0} nodos del tipo <area>" + Environment.NewLine, _CantNodosArea);
            respuesta += String.Format("Se encontraron {0} nodos del tipo <area> con más de 2 empleados" + Environment.NewLine, _CantNodosMasDe2Empleados);
            respuesta += _SalariosPorArea;

            Console.WriteLine(respuesta);            
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

    

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
