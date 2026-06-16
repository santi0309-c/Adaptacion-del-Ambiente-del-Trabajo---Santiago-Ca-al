using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTemplate1
{
    class CuentaBancaria
    {
        protected double saldo;

        public CuentaBancaria()
        {
            saldo = 0;
        }

        public virtual void Depositar(double monto)
        {
            if (monto <= 0)
            {
                Console.WriteLine("Error: el monto debe ser mayor a 0.");
                return;
            }
            saldo += monto;
            Console.WriteLine($"Depositaste ${monto}. Saldo: ${saldo}");
        }

        public virtual bool Extraer(double monto)
        {
            if (monto <= 0)
            {
                Console.WriteLine("Error: el monto debe ser mayor a 0.");
                return false;
            }
            saldo -= monto;
            return true;
        }

        public void Ver()
        {
            Console.WriteLine($"Saldo: ${saldo}");
        }
    }

    class CajaDeAhorro : CuentaBancaria
    {
        public CajaDeAhorro() : base() { }

        public override bool Extraer(double monto)
        {
            if (saldo < monto)
            {
                Console.WriteLine($"No hay suficiente plata. Saldo: ${saldo}");
                return false;
            }

            saldo -= monto;
            Console.WriteLine($"Extrajiste ${monto}.");
            return true;
        }
    }

    class CuentaCorriente : CuentaBancaria
    {
        private double limiteDesc;

        public CuentaCorriente(double desc) : base()
        {
            limiteDesc = desc;
        }

        public override bool Extraer(double monto)
        {
            if (saldo - monto < -limiteDesc)
            {
                Console.WriteLine($"Superarías el límite de ${limiteDesc}.");
                return false;
            }

            saldo -= monto;
            Console.WriteLine($"Extrajiste ${monto}. Saldo: ${saldo}");
            return true;
        }
    }

    class Banco
    {
        private List<CuentaBancaria> cuentas;

        public Banco()
        {
            cuentas = new List<CuentaBancaria>();
        }

        public void Agregar(CuentaBancaria cuenta)
        {
            cuentas.Add(cuenta);
            Console.WriteLine("Cuenta agregada.");
        }

        public void Transferir(CuentaBancaria origen, CuentaBancaria destino, double monto)
        {
            Console.WriteLine($"\n--- Transferencia de ${monto} ---");

            if (!cuentas.Contains(origen))
            {
                Console.WriteLine("La cuenta origen no existe.");
                return;
            }
            if (!cuentas.Contains(destino))
            {
                Console.WriteLine("La cuenta destino no existe.");
                return;
            }

            if (monto <= 0)
            {
                Console.WriteLine("El monto debe ser mayor a 0.");
                return;
            }

            if (!origen.Extraer(monto))
            {
                Console.WriteLine("No se pudo extraer.");
                return;
            }

            destino.Depositar(monto);
            Console.WriteLine("¡Transferencia hecha!");
        }
    }

    class PruebaCajero
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CAJERO AUTOMATICO ===\n");

            Console.WriteLine("--- Caja de Ahorro ---");
            CajaDeAhorro ahorro = new CajaDeAhorro();
            ahorro.Depositar(1000);
            ahorro.Extraer(400);
            ahorro.Extraer(800);
            ahorro.Ver();

            Console.WriteLine("\n--- Cuenta Corriente ---");
            CuentaCorriente corriente = new CuentaCorriente(500);
            corriente.Depositar(200);
            corriente.Extraer(600);
            corriente.Extraer(200);
            corriente.Ver();

            Console.WriteLine("\n--- Banco ---");
            Banco banco = new Banco();
            CajaDeAhorro ahorro2 = new CajaDeAhorro();
            CuentaCorriente corriente2 = new CuentaCorriente(500);

            banco.Agregar(ahorro2);
            banco.Agregar(corriente2);

            ahorro2.Depositar(1000);
            banco.Transferir(ahorro2, corriente2, 300);
            banco.Transferir(ahorro2, corriente2, 900);

            ahorro2.Ver();
            corriente2.Ver();
        }
    }
}
