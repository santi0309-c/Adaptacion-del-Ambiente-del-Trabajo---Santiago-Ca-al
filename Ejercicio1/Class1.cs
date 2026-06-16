using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ejercicio1
{
    class Semaforo
    {
        private int segundosEnFase;
        private bool modoIntermitente;
        private int segundosIntermitente;
        private bool amarilloEncendido;

        private const int DURACION_ROJO = 30;
        private const int DURACION_ROJO_AMARILLO = 2;
        private const int DURACION_VERDE = 20;
        private const int DURACION_AMARILLO = 2;
        private const int CICLO_COMPLETO = DURACION_ROJO + DURACION_ROJO_AMARILLO + DURACION_VERDE + DURACION_AMARILLO;

        public Semaforo(string faseInicial)
        {
            modoIntermitente = false;
            segundosIntermitente = 0;
            amarilloEncendido = true;

            switch (faseInicial.ToLower())
            {
                case "rojo":
                    segundosEnFase = 0;
                    break;
                case "rojo+amarillo":
                case "rojo + amarillo":
                    segundosEnFase = DURACION_ROJO;
                    break;
                case "verde":
                    segundosEnFase = DURACION_ROJO + DURACION_ROJO_AMARILLO;
                    break;
                case "amarillo":
                    segundosEnFase = DURACION_ROJO + DURACION_ROJO_AMARILLO + DURACION_VERDE;
                    break;
                default:
                    Console.WriteLine("Fase inicial no reconocida, arrancando en Rojo.");
                    segundosEnFase = 0;
                    break;
            }
        }

        public void Avanzar(int segundos)
        {
            if (modoIntermitente)
            {
                segundosIntermitente += segundos;
                amarilloEncendido = segundosIntermitente % 2 == 0;
            }
            else
            {
                segundosEnFase = (segundosEnFase + segundos) % CICLO_COMPLETO;
            }
        }

        public void MostrarEstado()
        {
            if (modoIntermitente)
            {
                string estado = amarilloEncendido ? "Amarillo (intermitente)" : "Apagado (intermitente)";
                Console.WriteLine($"Estado actual: {estado}");
                return;
            }

            Console.WriteLine($"Estado actual: {ObtenerFaseActual()}");
        }

        private string ObtenerFaseActual()
        {
            if (segundosEnFase < DURACION_ROJO)
                return "Rojo";

            if (segundosEnFase < DURACION_ROJO + DURACION_ROJO_AMARILLO)
                return "Rojo + Amarillo";

            if (segundosEnFase < DURACION_ROJO + DURACION_ROJO_AMARILLO + DURACION_VERDE)
                return "Verde";

            return "Amarillo";
        }

        public void ActivarIntermitente()
        {
            modoIntermitente = true;
            segundosIntermitente = 0;
            amarilloEncendido = true;
            Console.WriteLine("Semáforo en modo intermitente.");
        }

        public void DesactivarIntermitente()
        {
            modoIntermitente = false;
            Console.WriteLine("Semáforo vuelve al modo normal.");
        }
    }

    class PruebaSemaforo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PRUEBA SEMÁFORO ===\n");

            Semaforo semaforo = new Semaforo("Verde");
            semaforo.MostrarEstado();

            semaforo.Avanzar(20);
            semaforo.MostrarEstado();

            semaforo.Avanzar(2);
            semaforo.MostrarEstado();

            semaforo.Avanzar(15);
            semaforo.MostrarEstado();

            Console.WriteLine("\n--- Probando intermitente ---");
            semaforo.ActivarIntermitente();
            semaforo.MostrarEstado();

            semaforo.Avanzar(1);
            semaforo.MostrarEstado();

            semaforo.Avanzar(1);
            semaforo.MostrarEstado();

            semaforo.DesactivarIntermitente();
            semaforo.MostrarEstado();
        }
    }
}
