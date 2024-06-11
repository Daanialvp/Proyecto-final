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
        string archivoDatos = PedirNombre() + FechaHora() + ".txt";

        try
        {
            using (SerialPort puertoSerie = new SerialPort(puerto, baudios))
            {
                puertoSerie.Open();
                Console.WriteLine("Conexión establecida con Arduino.");

                MedicionSeñalPPG();

                Console.WriteLine("Recogiendo datos de la señal PPG...");
                RecogerDatosPPG(puertoSerie, archivoDatos);

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

    static string PedirNombre()
    {
        Console.WriteLine("Por favor ingrese el nombre del (la) paciente: ");
        return Console.ReadLine();
    }

    static void MedicionSeñalPPG()
    {
        Console.WriteLine("Por favor, coloque su dedo sobre el sensor para comenzar la medición.");
        Thread.Sleep(5000);
    }

    static string FechaHora()
    {
        DateTime verHora = DateTime.Now;
        return verHora.ToString("_yyyy_MM_dd_HH_mm");
    }

    private static void RecogerDatosPPG(SerialPort puertoSerie, string archivoDatos)
    {
        using (StreamWriter escritor = new StreamWriter(archivoDatos))
        {
            const int muestras = 1000;
            int muestrasRecogidas = 0;

            while (muestrasRecogidas < muestras)
            {
                Thread.Sleep(10);

                if (puertoSerie.BytesToRead > 0)
                {
                    string datosRecibidos = puertoSerie.ReadLine();
                    if (double.TryParse(datosRecibidos, out _))
                    {
                        escritor.WriteLine(datosRecibidos.Trim());
                        muestrasRecogidas++;
                    }
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

            Array.Resize(ref datosPPG, muestrasValidas);

            if (datosPPG.Length == 0)
            {
                Console.WriteLine("No hay datos válidos para detectar ciclos.");
                return;
            }

            const double umbralRelativo = 0.5;
            double maxValor = datosPPG[0];
            double minValor = datosPPG[0];

            foreach (var valor in datosPPG)
            {
                if (valor > maxValor) maxValor = valor;
                if (valor < minValor) minValor = valor;
            }

            double umbral = minValor + (maxValor - minValor) * umbralRelativo;

            bool enCiclo = false;
            int ciclosDetectados = 0;
            int inicioCiclo = 0;
            double duracionTotal = 0;

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
                    duracionTotal += duracionCiclo;
                    ciclosDetectados++;

                    if (ciclosDetectados == 10)
                        break;
                }
            }

            if (ciclosDetectados == 10)
            {
                double duracionPromedio = duracionTotal / 10;
                double frecuenciaCardiacaPromedio = 60000 / (duracionPromedio * 10);
                Console.WriteLine($"\nFrecuencia cardíaca promedio: {frecuenciaCardiacaPromedio} latidos por minuto");

                using (StreamWriter escritor = new StreamWriter("FrecuenciasCardiacas.txt", true))
                {
                    escritor.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {Path.GetFileName(archivoDatos)}: {frecuenciaCardiacaPromedio} latidos por minuto");
                }
            }
            else
            {
                Console.WriteLine("No se detectaron suficientes ciclos para calcular la frecuencia cardíaca.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al detectar los ciclos: " + ex.Message);
        }
    }
}
