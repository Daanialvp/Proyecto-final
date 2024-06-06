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

        static string PedirNombre() // función para obtener el nombre del paciente
        {
            Console.WriteLine("Por favor ingrese el nombre del (la) paciente: ");
            string nombre = Console.ReadLine();
            return nombre;
        }

        
        static string FechaHora() //función FechaHora() : de aqui pueden sacar la fecha y hora en fomato año:mes:dia:hora
        {
            DateTime verHora = DateTime.Now;
            string fechaHora = verHora.ToString("yyyy-MM-dd HH:mm");
            return fechaHora;
        }

        static void Main(string[] args)
        {
            // conexión con aduino (descomentar para probar con arduino)
            //SerialPort puerto = new SerialPort();
            //puerto.BaudRate = 9600;
            //puerto.PortName = "COM3";
            //puerto.Open();

            
            

            //Pedir nombre
            string nombrePaciente = PedirNombre();

            //Pedir dedo
            //Generar datos
            //Registrar datos

            Console.WriteLine("Texto para probar la consola");
            Console.ReadLine();

        }
    }
}
