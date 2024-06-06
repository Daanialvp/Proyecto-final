using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;


namespace Proyecto
{
    internal class Program
    {

        static string PedirNombre()
        {
            Console.WriteLine("Por favor ingrese el nombre del (la) paciente: ");
            string nombre = Console.ReadLine();
            return nombre;
        }

        static void Main(string[] args)
        {
            // conexión con aduino
            SerialPort puerto = new SerialPort();
            puerto.BaudRate = 9600;
            puerto.PortName = "COM3";
            puerto.Open();

            //Pedir nombre
            string nombrePaciente = PedirNombre();
            //Pedir dedo
            //Generar datos
            //Registrar datos

            Console.WriteLine(puerto.ReadLine());
            Console.ReadLine();

        }
    }
}
