using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        string puerto = "COM6";
        int baudios = 250000;
        string archivoDatos = PedirNombre() + FechaHora() + ".txt"; // Nombre del archivo para guardar los datos PPG

        try
        {
            using (SerialPort puertoSerie = new SerialPort(puerto, baudios))
            {
                puertoSerie.Open();
                Console.WriteLine("Conexión establecida con Arduino.");

                // Pedir al paciente que coloque el dedo en el sensor
                MedicionSeñalPPG();

                // Recoger datos de PPG y guardarlos en un archivo
                Console.WriteLine("Recogiendo datos de la señal PPG...");
                RecogerDatosPPG(puertoSerie, archivoDatos);

                // Leer datos desde el archivo y detectar ciclos
                Console.WriteLine("Detectando ciclos cardíacos...");
                DetectarCiclos(archivoDatos);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: Puerto COM en uso o no disponible.");
        }
        catch (IOException ex)
        {
            Console.WriteLine("Error de E/S: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.WriteLine("Presiona cualquier tecla para salir...");
        Console.ReadKey();
    }

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

    static string FechaHora() // función FechaHora() : de aqui pueden sacar la fecha y hora en formato año:mes:dia:hora
    {
        DateTime verHora = DateTime.Now;
        string fechaHora = verHora.ToString("_yyyy_MM_dd_HH_mm"); // formato ya listo para guardar así el archivo de texto
        return fechaHora;
    }

    private static void RecogerDatosPPG(SerialPort puertoSerie, string archivoDatos)
    {
        using (StreamWriter escritor = new StreamWriter(archivoDatos))
        {
            const int muestras = 1000; // Número total de muestras a recoger
            int muestrasRecogidas = 0;

            while (muestrasRecogidas < muestras)
            {
                Thread.Sleep(10);

                if (puertoSerie.BytesToRead > 0)
                {
                    string datosRecibidos = puertoSerie.ReadLine();
                    escritor.WriteLine(datosRecibidos.Trim());
                    muestrasRecogidas++;
                }
            }
        }
        Console.WriteLine("Datos de la señal PPG guardados en '{0}'.", archivoDatos);
    }

    private static void DetectarCiclos(string archivoDatos)
    {
        try
        {
            string[] lineas = File.ReadAllLines(archivoDatos);
            if (lineas.Length == 0)
            {
                Console.WriteLine("Error: No se encontraron datos en el archivo.");
                return;
            }

            double[] datosPPG = new double[lineas.Length];
            int muestrasValidas = 0;

            for (int i = 0; i < lineas.Length; i++)
            {
                if (double.TryParse(lineas[i], out double valor))
                {
                    datosPPG[muestrasValidas++] = valor;
                }
                else
                {
                    Console.WriteLine("Advertencia: Se encontró un dato no válido en la línea {0}: '{1}'", i + 1, lineas[i]);
                }
            }

            // Redimensionar el arreglo para contener solo las muestras válidas
            Array.Resize(ref datosPPG, muestrasValidas);

            // Continuar con la detección de ciclos solo si hay muestras válidas
            if (datosPPG.Length == 0)
            {
                Console.WriteLine("No hay datos válidos para detectar ciclos.");
                return;
            }

            // Detección de ciclos cardíacos
            const double umbral = 0.5;
            bool enCiclo = false;
            int ciclosDetectados = 0;
            int inicioCiclo = 0;

            for (int i = 0; i < datosPPG.Length; i++)
            {
                if (datosPPG[i] > umbral && !enCiclo)
                {
                    enCiclo = true;
                    inicioCiclo = i;
                }
                else if (datosPPG[i] < umbral && enCiclo)
                {
                    enCiclo = false;
                    int duracionCiclo = i - inicioCiclo;
                    Console.WriteLine($"Ciclo {++ciclosDetectados}: {duracionCiclo} muestras ({duracionCiclo * 10} ms)");
                }
            }

            // Calcular la frecuencia cardíaca promedio
            double duracionTotal = datosPPG.Length * 10; // Duración total en milisegundos
            double frecuenciaCardiacaPromedio = (ciclosDetectados / (duracionTotal / 60000.0)); // En latidos por minuto
            Console.WriteLine($"\nFrecuencia cardíaca promedio: {frecuenciaCardiacaPromedio} latidos por minuto");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al detectar los ciclos: " + ex.Message);
        }
    }
}
