using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;


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

        static void MedicionSeñalPPG() // Indicar cuando el paciente debe colocar su dedo sobre el sensor MAX30100 para llevar a cabo la medición de la señal PPG.
        {
            Console.WriteLine("Por favor, coloque su dedo sobre el sensor para comenzar la medición.");
            Thread.Sleep(5000);
        }

        static void Main(string[] args)
        {
            SerialPort puerto = new SerialPort();
            puerto.BaudRate = 250000;
            puerto.PortName = "COM3";
            puerto.Open();

            //Pedir nombre
            string nombrePaciente = PedirNombre();

            //Pedir dedo
            MedicionSeñalPPG();

            // Variables para el cálculo de frecuencia cardíaca
            int pulsos = 0;
            DateTime inicio = DateTime.Now;
            string datosPPG = "";
            Console.WriteLine("Midiendo frecuencia cardíaca...");

            // Leer datos del sensor
            while (pulsos < 10)
            {
                try
                {
                    string dato = puerto.ReadLine();
                    int valorIR = int.Parse(dato.Trim());

                    // Detectar un pulso (umbral simple para ejemplo)
                    if (valorIR > 50000)
                    {
                        pulsos++;
                        Console.WriteLine("Pulso detectado: " + pulsos);
                    }

                    // Almacenar datos PPG
                    datosPPG += valorIR + "\n";

                    // Esperar 10 ms antes de la siguiente lectura
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al leer del sensor: " + ex.Message);
                }
            }
            //Generar datos


            //Registrar datos

            Console.WriteLine("Texto para probar la consola");
            Console.ReadLine();

        }
    }
}
