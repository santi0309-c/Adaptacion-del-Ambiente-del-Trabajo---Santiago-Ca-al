using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTemplate1
{
    interface IVehiculo
    {
        void Mover(int tiempo);
        double Pos();
        void Resetear();
    }

    class Bicicleta : IVehiculo
    {
        private double pos;
        private const double VEL = 10;

        public Bicicleta()
        {
            pos = 0;
        }

        public void Mover(int tiempo)
        {
            pos += VEL * tiempo;
        }

        public double Pos()
        {
            return pos;
        }

        public void Resetear()
        {
            pos = 0;
        }

        public override string ToString()
        {
            return "Bici";
        }
    }

    class Camion : IVehiculo
    {
        private double pos;
        private const double VEL = 30;

        public Camion()
        {
            pos = 0;
        }

        public void Mover(int tiempo)
        {
            pos += VEL * tiempo;
        }

        public double Pos()
        {
            return pos;
        }

        public void Resetear()
        {
            pos = 0;
        }

        public override string ToString()
        {
            return "Camión";
        }
    }

    class Auto : IVehiculo
    {
        private double pos;
        private double vel;
        private const double VEL_DEFAULT = 40;

        public Auto()
        {
            pos = 0;
            vel = VEL_DEFAULT;
        }

        public Auto(double velCustom)
        {
            pos = 0;
            vel = velCustom;
        }

        public void Mover(int tiempo)
        {
            pos += vel * tiempo;
        }

        public double Pos()
        {
            return pos;
        }

        public void Resetear()
        {
            pos = 0;
        }

        public override string ToString()
        {
            return $"Auto({vel}m/s)";
        }
    }

    class Carrera
    {
        private IVehiculo v1;
        private IVehiculo v2;
        private string nombre1;
        private string nombre2;

        public Carrera(IVehiculo auto1, string nom1, IVehiculo auto2, string nom2)
        {
            v1 = auto1;
            nombre1 = nom1;
            v2 = auto2;
            nombre2 = nom2;
        }

        public void Correr(int segundos)
        {
            Console.WriteLine($"\n==== CARRERA: {nombre1} vs {nombre2} ({segundos}s) ====");

            v1.Resetear();
            v2.Resetear();

            v1.Mover(segundos);
            v2.Mover(segundos);

            double pos1 = v1.Pos();
            double pos2 = v2.Pos();

            Console.WriteLine($"{nombre1}: {pos1}m");
            Console.WriteLine($"{nombre2}: {pos2}m");

            if (pos1 > pos2)
                Console.WriteLine($"¡Ganó {nombre1}!");
            else if (pos2 > pos1)
                Console.WriteLine($"¡Ganó {nombre2}!");
            else
                Console.WriteLine("¡Empate!");
        }
    }

    class PruebaVehiculos
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CARRERAS DE VEHICULOS ===\n");

            Auto fiat = new Auto(45);
            Bicicleta bici = new Bicicleta();
            Camion camion = new Camion();

            Console.WriteLine("--- Test individual ---");
            bici.Mover(20);
            Console.WriteLine($"Bici después de 20s: {bici.Pos()}m");

            bici.Mover(10);
            Console.WriteLine($"Bici después de 10s más: {bici.Pos()}m");

            Console.WriteLine("\n--- Las carreras ---");

            Carrera carrera1 = new Carrera(fiat, "Fiat 45m/s", bici, "Bicicleta");
            carrera1.Correr(60);

            Carrera carrera2 = new Carrera(bici, "Bicicleta", camion, "Camión");
            carrera2.Correr(30);
        }
    }

}
