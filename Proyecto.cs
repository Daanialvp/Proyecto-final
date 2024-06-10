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

        static string FechaHora() //función FechaHora() : de aqui pueden sacar la fecha y hora en fomato año:mes:dia:hora
        {
            DateTime verHora = DateTime.Now;
            string fechaHora = verHora.ToString("_yyyy_MM_dd_HH_mm"); // formato ya listo para guardar así el archivo de texto
            return fechaHora;
        }

        static void Main(string[] args)
        {
            SerialPort puerto = new SerialPort
            {
                BaudRate = 250000,
                PortName = "COM3",
                ReadTimeout = 5000 // Timeout para evitar bloqueos en la lectura
            };


            // Apertura del puerto
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

            // Armar nombre del archivo de texto
            string NombreArchivo = nombrePaciente+FechaHora()+".txt";

            //Inicializar el escritor
            StreamWriter escritor = new StreamWriter(NombreArchivo);

            // Indicar al paciente que coloque su dedo
            MedicionSeñalPPG();

            // Variables para el cálculo de frecuencia cardíaca
            int pulsos = 0;
            double IRAnterior = -1;
            Console.WriteLine("Midiendo frecuencia cardíaca, por favor espere...");

            //ciclo de espera
            string basura;
            for (int index = 0; index < 1500; index++)
            {
                basura = puerto.ReadLine();
                Thread.Sleep(1);
            }

            // Leer datos del sensor
            while (pulsos < 10)
            {
                try
                {
                    


                    string dato = puerto.ReadLine().Trim();
                    Console.WriteLine("Dato recibido: " + dato);
                    escritor.WriteLine(dato);

                    if (!double.TryParse(dato, out double valorIR))
                    {
                        Console.WriteLine("Dato no válido recibido: " + dato);
                        continue;
                    }

                    // Detectar un pulso
                    if ( (valorIR <= 0) && (IRAnterior > 0))
                    {
                        pulsos++;
                        Console.WriteLine("Pulso detectado: " + pulsos);
                        Console.Beep(); // beep cada que detecte un pulso Es más que nada para que se vea bonito jaja
                    }
                    IRAnterior = valorIR;
                    // Esperar 10 ms antes de la siguiente lectura
                    Thread.Sleep(10);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("No se recibieron datos a tiempo de la lectura del sensor");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al leer el sensor: " + ex.Message);
                }
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

            escritor.Close();
            puerto.Close();
            Console.WriteLine("Presione Enter para salir...");
            Console.ReadLine();
        }
    }
}
