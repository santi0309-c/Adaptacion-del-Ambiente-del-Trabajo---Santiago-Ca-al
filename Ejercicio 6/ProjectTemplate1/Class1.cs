using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTemplate1
{
    class Carta
    {
        public readonly string Palo;
        public readonly string Numero;

        public Carta(string palo, string numero)
        {
            Palo = palo;
            Numero = numero;
        }

        public override string ToString()
        {
            return $"{Numero} de {Palo}";
        }
    }

    class Mazo
    {
        private List<Carta> cartas;

        private static readonly string[] PALOS = { "Espadas", "Bastos", "Oros", "Copas" };
        private static readonly string[] NUMEROS = { "1", "2", "3", "4", "5", "6", "7", "10", "11", "12" };

        public Mazo()
        {
            cartas = new List<Carta>();

            foreach (string palo in PALOS)
            {
                foreach (string numero in NUMEROS)
                {
                    cartas.Add(new Carta(palo, numero));
                }
            }
        }

        public void Barajar()
        {
            Random rng = new Random();
            for (int i = cartas.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                Carta temp = cartas[i];
                cartas[i] = cartas[j];
                cartas[j] = temp;
            }
            Console.WriteLine("¡Mazo barajado!");
        }

        public Carta Robar()
        {
            if (cartas.Count == 0)
            {
                Console.WriteLine("No hay más cartas!");
                return null;
            }

            Carta carta = cartas[cartas.Count - 1];
            cartas.RemoveAt(cartas.Count - 1);
            return carta;
        }

        public int Cantidad()
        {
            return cartas.Count;
        }
    }

    class Mano
    {
        private List<Carta> cartas;
        private string nombre;

        public Mano(string jugador)
        {
            cartas = new List<Carta>();
            nombre = jugador;
        }

        public void Recibir(Carta carta)
        {
            if (carta != null)
                cartas.Add(carta);
        }

        public void Ver()
        {
            Console.WriteLine($"\nMano de {nombre} ({cartas.Count} cartas):");
            foreach (Carta carta in cartas)
            {
                Console.WriteLine($"  - {carta}");
            }
        }

        public int Cantidad()
        {
            return cartas.Count;
        }
    }

    class PruebaMazo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== JUEGO DE CARTAS ===\n");

            Mazo mazo = new Mazo();
            Console.WriteLine($"Cartas en el mazo: {mazo.Cantidad()}");

            mazo.Barajar();

            Mano j1 = new Mano("Juan");
            Mano j2 = new Mano("María");

            Console.WriteLine("\nRepartiendo 3 cartas a cada uno...");
            for (int i = 0; i < 3; i++)
            {
                j1.Recibir(mazo.Robar());
                j2.Recibir(mazo.Robar());
            }

            j1.Ver();
            j2.Ver();

            Console.WriteLine($"\nCartas que quedan en el mazo: {mazo.Cantidad()}");
        }
    }
}
