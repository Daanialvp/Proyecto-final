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
            return Console.ReadLine();
        }

        static void MedicionSeñalPPG() // Indicar cuando el paciente debe colocar su dedo sobre el sensor MAX30100 para llevar a cabo la medición de la señal PPG.
        {
            Console.WriteLine("Por favor, coloque su dedo sobre el sensor para comenzar la medición.");
            Thread.Sleep(5000);
        }

        static void Main(string[] args)
        {
            SerialPort puerto = new SerialPort
            {
                BaudRate = 250000,
                PortName = "COM3",
                ReadTimeout = 5000 // Timeout para evitar bloqueos en la lectura
            };

            try
            {
                puerto.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo abrir el puerto: " + ex.Message);
                return;
            }

            // Pedir nombre
            string nombrePaciente = PedirNombre();

            // Indicar al paciente que coloque su dedo
            MedicionSeñalPPG();

            // Variables para el cálculo de frecuencia cardíaca
            int pulsos = 0;
            Console.WriteLine("Midiendo frecuencia cardíaca...");

            // Leer datos del sensor
            while (pulsos < 10)
            {
                try
                {
                    string dato = puerto.ReadLine().Trim();
                    Console.WriteLine("Dato recibido: " + dato);

                    if (!double.TryParse(dato, out double valorIR))
                    {
                        Console.WriteLine("Dato no válido recibido: " + dato);
                        continue;
                    }

                    // Detectar un pulso
                    if (valorIR > 50000)
                    {
                        pulsos++;
                        Console.WriteLine("Pulso detectado: " + pulsos);
                    }

                    // Esperar 10 ms antes de la siguiente lectura
                    Thread.Sleep(10);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("No se recibieron datos a tiempo de la lectura del sensor");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al leer del sensor: " + ex.Message);
                }
                //Generar datos
                int LPM, i = 0, promedio;
                for (i = 0; i == 10; i++) ;
                {
                    LPM = pulsos / 10;
                    Console.ReadKey();
                }
                Console.WriteLine("LPM " + LPM);
                promedio = pulsos * 6;
                Console.WriteLine("promedio");
                    
            }

            puerto.Close();
            Console.WriteLine("Presione Enter para salir...");
            Console.ReadLine();
        }
    }
}
