using System;
using System.Collections.Generic;
$
if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;

namespace $rootnamespace$
{
    class Cronometro
    {
        private int segundos;
        private int minutos;

        public Cronometro()
        {
            segundos = 0;
            minutos = 0;
        }

        public void Resetear()
        {
            segundos = 0;
            minutos = 0;
        }

        public void Sumar()
        {
            segundos++;
            if (segundos > 59)
            {
                minutos++;
                segundos = 0;
            }
        }

        public string Ver()
        {
            return $"{minutos}m {segundos}s";
        }

        public void Mostrar()
        {
            Console.WriteLine($"Tiempo: {Ver()}");
        }
    }

    class PruebaCronometro
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PROBANDO CRONOMETRO ===\n");

            Cronometro crono = new Cronometro();
            crono.Mostrar();

            Console.WriteLine("Sumando 5000 segundos...");
            for (int i = 0; i < 5000; i++)
            {
                crono.Sumar();
            }
            crono.Mostrar();

            Console.WriteLine("\nResetando...");
            crono.Resetear();
            crono.Mostrar();
        }
    }
}
