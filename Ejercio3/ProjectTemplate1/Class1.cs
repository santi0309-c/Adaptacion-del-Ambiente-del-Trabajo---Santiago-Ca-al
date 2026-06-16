using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTemplate1
{
    interface IJugador
    {
        bool Correr(int min);
        bool Cansado();
        void Descansar(int min);
    }

    class JugadorAmateur : IJugador
    {
        private int minCorridos;
        private const int MAX = 20;

        public JugadorAmateur()
        {
            minCorridos = 0;
        }

        public bool Correr(int min)
        {
            if (Cansado())
            {
                Console.WriteLine("El amateur está muy cansado, no aguanta más!");
                return false;
            }

            int minDisponibles = MAX - minCorridos;

            if (min <= minDisponibles)
            {
                minCorridos += min;
                Console.WriteLine($"Amateur corrió {min} min sin problema.");
                return true;
            }
            else
            {
                minCorridos = MAX;
                Console.WriteLine($"Amateur se cansó después de {minDisponibles} min.");
                return false;
            }
        }

        public bool Cansado()
        {
            return minCorridos >= MAX;
        }

        public void Descansar(int min)
        {
            minCorridos -= min;
            if (minCorridos < 0)
                minCorridos = 0;
            Console.WriteLine($"Amateur descansó {min} min, se recuperó.");
        }
    }

    class JugadorProfesional : IJugador
    {
        private int minCorridos;
        private const int MAX = 40;

        public JugadorProfesional()
        {
            minCorridos = 0;
        }

        public bool Correr(int min)
        {
            if (Cansado())
            {
                Console.WriteLine("El profesional está cansado, necesita parar!");
                return false;
            }

            int minDisponibles = MAX - minCorridos;

            if (min <= minDisponibles)
            {
                minCorridos += min;
                Console.WriteLine($"Profesional corrió {min} min fácil.");
                return true;
            }
            else
            {
                minCorridos = MAX;
                Console.WriteLine($"Profesional se cansó después de {minDisponibles} min.");
                return false;
            }
        }

        public bool Cansado()
        {
            return minCorridos >= MAX;
        }

        public void Descansar(int min)
        {
            minCorridos -= min;
            if (minCorridos < 0)
                minCorridos = 0;
            Console.WriteLine($"Profesional descansó {min} min.");
        }
    }

    class PruebaJugadores
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PROBANDO JUGADORES ===\n");

            JugadorAmateur pepe = new JugadorAmateur();
            JugadorProfesional messi = new JugadorProfesional();

            Console.WriteLine("--- Amateur (Pepe) ---");
            pepe.Correr(15);
            Console.WriteLine($"¿Cansado? {pepe.Cansado()}");

            pepe.Correr(10);
            Console.WriteLine($"¿Cansado? {pepe.Cansado()}");

            pepe.Correr(5);
            pepe.Descansar(20);
            Console.WriteLine($"¿Cansado después de descansar? {pepe.Cansado()}\n");

            Console.WriteLine("--- Profesional (Messi) ---");
            messi.Correr(30);
            Console.WriteLine($"¿Cansado? {messi.Cansado()}");

            messi.Correr(15);
            Console.WriteLine($"¿Cansado? {messi.Cansado()}");

            messi.Descansar(10);
            messi.Correr(5);
        }
    }
}
