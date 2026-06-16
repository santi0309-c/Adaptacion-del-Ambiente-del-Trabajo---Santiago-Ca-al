using System;
using System.Data.SQLite;
using System.IO;

namespace ClinicaMedicaTurnos
{
    class Program
    {
        static readonly string cadenaConexion = ObtenerCadenaConexion();

        static string ObtenerCadenaConexion()
        {
            string rutaEnv = Environment.GetEnvironmentVariable("CLINICA_DB_PATH");
            string ruta = string.IsNullOrWhiteSpace(rutaEnv)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClinicaMedica.db")
                : rutaEnv;
            return $"Data Source={ruta};Version=3;";
        }

        static string LeerLinea(string mensaje)
        {
            Console.Write(mensaje);
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        static int LeerEntero(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                MostrarError("No es número, reintentá.");
            }
        }

        static DateTime LeerFecha(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (DateTime.TryParse(Console.ReadLine(), out DateTime d)) return d;
                MostrarError("Fecha inválida.");
            }
        }

        static TimeSpan LeerHora(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan t)) return t;
                MostrarError("Hora inválida.");
            }
        }

        static void MostrarError(string m)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n! " + m);
            Console.ForegroundColor = prev;
        }

        static void MostrarExito(string m)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n> " + m);
            Console.ForegroundColor = prev;
        }

        static void Pausa()
        {
            Console.WriteLine("\n(enter para seguir)");
            Console.ReadLine();
        }

        static int EjecutarComando(string sql, Action<SQLiteCommand> parametros)
        {
            using (var conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SQLiteCommand(sql, conexion))
                {
                    parametros?.Invoke(cmd);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Ejecuta una consulta escalar y devuelve el resultado casteado a T.</summary>
        static T EjecutarEscalar<T>(string sql, Action<SQLiteCommand> parametros)
        {
            using (var conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SQLiteCommand(sql, conexion))
                {
                    parametros?.Invoke(cmd);
                    object resultado = cmd.ExecuteScalar();
                    return resultado == null || resultado == DBNull.Value ? default(T) : (T)Convert.ChangeType(resultado, typeof(T));
                }
            }
        }

        /// <summary>Ejecuta un SELECT y llama a onFila por cada fila leída.</summary>
        static void EjecutarLectura(string sql, Action<SQLiteCommand> parametros, Action<SQLiteDataReader> onFila, Action onHeader = null)
        {
            using (var conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SQLiteCommand(sql, conexion))
                {
                    parametros?.Invoke(cmd);
                    using (var reader = cmd.ExecuteReader())
                    {
                        bool primero = true;
                        while (reader.Read())
                        {
                            if (primero) { onHeader?.Invoke(); primero = false; }
                            onFila(reader);
                        }
                    }
                }
            }
        }

        // ---------------------------
        // Lógica de la aplicación
        // ---------------------------
        static void Main()
        {
            CargarDatosIniciales();

            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("Clínica - menú (versión simple)");
                Console.WriteLine("1) Nuevo paciente");
                Console.WriteLine("2) Ver médicos/especialidades");
                Console.WriteLine("3) Pedir turno");
                Console.WriteLine("4) Historial paciente");
                Console.WriteLine("5) Cancelar turno");
                Console.WriteLine("6) Marcar atendido");
                Console.WriteLine("7) Salir");
                string op = LeerLinea("-> ");
                switch (op)
                {
                    case "1": NuevoPaciente(); break;
                    case "2": VerMedicos(); break;
                    case "3": PedirTurno(); break;
                    case "4": HistorialTurnosPaciente(); break;
                    case "5": CancelarTurno(); break;
                    case "6": MarcarTurnoAtendido(); break;
                    case "7": salir = true; break;
                    default: MostrarError("Opción inválida."); Pausa(); break;
                }
            }
        }

        static void NuevoPaciente()
        {
            Console.Clear();
            Console.WriteLine("--- Nuevo paciente ---\n");
            string dni = LeerLinea("DNI: ");
            if (string.IsNullOrWhiteSpace(dni)) { MostrarError("DNI vacío"); Pausa(); return; }
            if (ValidarExistenciaPaciente(dni)) { MostrarError("Ese DNI ya existe"); Pausa(); return; }
            string nombre = LeerLinea("Nombre: ");
            string telefono = LeerLinea("Tel: ");
            string email = LeerLinea("Email: ");
            DateTime fn = LeerFecha("Fnac (YYYY-MM-DD): ");

            EjecutarComando(
                @"INSERT INTO Pacientes (dni, nombre_completo, telefono, email, fecha_nacimiento)
                  VALUES (@dni,@nom,@tel,@mail,@fn)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@tel", telefono);
                    cmd.Parameters.AddWithValue("@mail", email);
                    cmd.Parameters.AddWithValue("@fn", fn.ToString("yyyy-MM-dd"));
                });

            MostrarExito("Paciente guardado :)");
            Pausa();
        }

        static bool ValidarExistenciaPaciente(string dni)
        {
            return EjecutarEscalar<long>("SELECT COUNT(*) FROM Pacientes WHERE dni=@dni", cmd => cmd.Parameters.AddWithValue("@dni", dni)) > 0;
        }

        static void VerMedicos()
        {
            Console.Clear();
            Console.WriteLine("--- Médicos y especialidades ---\n");
            EjecutarLectura(
                @"SELECT m.id_medico, m.nombre_completo, m.matricula,
                         e.id_especialidad, e.nombre AS esp, e.duracion_turno
                  FROM Medicos m
                  JOIN Medicos_Especialidades me ON m.id_medico = me.id_medico
                  JOIN Especialidades e ON me.id_especialidad = e.id_especialidad
                  WHERE m.activo = 1
                  ORDER BY m.nombre_completo, e.nombre",
                _ => { },
                r => Console.WriteLine($"{r["id_medico"],-3} | {r["nombre_completo"],-22} | {r["matricula"],-10} | [{r["id_especialidad"]}] {r["esp"],-15} | {r["duracion_turno"]}m"),
                () => Console.WriteLine("ID | Médico | Matrícula | Especialidad | Dur"));

            Pausa();
        }

        static void PedirTurno()
        {
            Console.Clear();
            Console.WriteLine("--- Pedir turno ---\n");
            string dni = LeerLinea("DNI: ");
            if (!ValidarExistenciaPaciente(dni)) { MostrarError("No está registrado"); Pausa(); return; }

            int idMed = LeerEntero("ID médico: ");
            int idEsp = LeerEntero("ID especialidad: ");
            DateTime fecha = LeerFecha("Fecha (YYYY-MM-DD): ");
            TimeSpan hora = LeerHora("Hora (HH:MM): ");

            string fechaStr = fecha.ToString("yyyy-MM-dd");
            string horaStr = hora.ToString(@"hh\:mm");

            if (!ValidarDisponibilidad(idMed, idEsp, fecha.DayOfWeek, hora)) { MostrarError("El doc no atiende ahí"); Pausa(); return; }
            if (ValidarSuperposicionPaciente(dni, idMed, fechaStr)) { MostrarError("Ya tenés turno con ese doc ese día"); Pausa(); return; }
            if (ValidarSuperposicionMedico(idMed, fechaStr, horaStr)) { MostrarError("El doc ya tiene turno a esa hora"); Pausa(); return; }

            EjecutarComando(
                @"INSERT INTO Turnos (dni_paciente, id_medico, id_especialidad, fecha, hora, estado)
                  VALUES (@dni,@idm,@ide,@f,@h,'reservado')",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@idm", idMed);
                    cmd.Parameters.AddWithValue("@ide", idEsp);
                    cmd.Parameters.AddWithValue("@f", fechaStr);
                    cmd.Parameters.AddWithValue("@h", horaStr);
                });

            MostrarExito($"Turno reservado {fecha:dd/MM/yyyy} {horaStr}");
            Pausa();
        }

        // RN-03: valida en tabla Disponibilidad (dia_semana 0=Dom..6=Sáb como DayOfWeek)
        static bool ValidarDisponibilidad(int idMedico, int idEspecialidad, DayOfWeek dia, TimeSpan hora)
        {
            long tablaExiste = EjecutarEscalar<long>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Disponibilidad'", _ => { });
            if (tablaExiste == 0) return true;

            return EjecutarEscalar<long>(
                @"SELECT COUNT(*) FROM Disponibilidad
                  WHERE id_medico       = @idMed
                    AND id_especialidad = @idEsp
                    AND dia_semana      = @dia
                    AND hora_inicio    <= @hora
                    AND hora_fin        > @hora",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@idMed", idMedico);
                    cmd.Parameters.AddWithValue("@idEsp", idEspecialidad);
                    cmd.Parameters.AddWithValue("@dia", (int)dia);
                    cmd.Parameters.AddWithValue("@hora", hora.ToString(@"hh\:mm"));
                }) > 0;
        }

        static bool ValidarSuperposicionPaciente(string dni, int idMedico, string fecha)
        {
            return EjecutarEscalar<long>(
                "SELECT COUNT(*) FROM Turnos WHERE dni_paciente=@dni AND id_medico=@idMed AND fecha=@fecha AND estado='reservado'",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@idMed", idMedico);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                }) > 0;
        }

        static bool ValidarSuperposicionMedico(int idMedico, string fecha, string hora)
        {
            return EjecutarEscalar<long>(
                "SELECT COUNT(*) FROM Turnos WHERE id_medico=@idMed AND fecha=@fecha AND hora=@hora AND estado='reservado'",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@idMed", idMedico);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@hora", hora);
                }) > 0;
        }

        // HISTORIAL
        static void HistorialTurnosPaciente()
        {
            Console.Clear();
            Console.WriteLine("─── HISTORIAL DE TURNOS POR PACIENTE ───\n");
            string dni = LeerLinea("DNI del paciente: ");

            bool hayRegistros = false;
            EjecutarLectura(
                @"SELECT t.id_turno, t.fecha, t.hora,
                         m.nombre_completo          AS Medico,
                         e.nombre                   AS Especialidad,
                         t.estado,
                         t.motivo_cancelacion,
                         t.responsable_cancelacion
                  FROM Turnos t
                  JOIN Medicos       m ON t.id_medico        = m.id_medico
                  JOIN Especialidades e ON t.id_especialidad = e.id_especialidad
                  WHERE t.dni_paciente = @dni
                  ORDER BY t.fecha DESC, t.hora DESC",
                cmd => cmd.Parameters.AddWithValue("@dni", dni),
                reader =>
                {
                    hayRegistros = true;
                    string estado = reader["estado"].ToString();
                    string infoExtra = "";

                    if (estado == "cancelado")
                    {
                        string motivo = reader["motivo_cancelacion"] != DBNull.Value
                            ? reader["motivo_cancelacion"].ToString()
                            : "Sin motivo";
                        string responsable = reader["responsable_cancelacion"] != DBNull.Value
                            ? reader["responsable_cancelacion"].ToString()
                            : "No registrado";
                        infoExtra = $"Motivo: {motivo} | Responsable: {responsable}";
                    }

                    Console.WriteLine($"  {reader["id_turno"],-6} | {reader["fecha"],-10} | {reader["hora"],-5} | " +
                                      $"{reader["Medico"],-20} | {reader["Especialidad"],-16} | {estado,-10} | {infoExtra}");
                },
                () =>
                {
                    Console.WriteLine($"  {"ID",-6} | {"Fecha",-10} | {"Hora",-5} | {"Médico",-20} | {"Especialidad",-16} | {"Estado",-10} | Info Extra");
                    Console.WriteLine($"  {new string('─', 110)}");
                });

            if (!hayRegistros)
                Console.WriteLine("  No se encontraron turnos para ese DNI.");

            Pausa();
        }

        // CANCELAR CON MOTIVO Y RESPONSABLE
        static void CancelarTurno()
        {
            Console.Clear();
            Console.WriteLine("─── CANCELACIÓN LÓGICA DE TURNO (RN-07) ───\n");

            int idTurno = LeerEntero("ID del turno a cancelar: ");
            string estadoActual = ObtenerEstadoTurno(idTurno);
            if (estadoActual == null)
            {
                MostrarError("No se encontró ningún turno con ese ID.");
                Pausa(); return;
            }
            if (estadoActual != "reservado")
            {
                MostrarError($"El turno ya se encuentra en estado '{estadoActual}' y no puede cancelarse. (RN-06)");
                Pausa(); return;
            }

            string motivo = LeerLinea("Motivo de cancelación: ");
            if (string.IsNullOrWhiteSpace(motivo)) motivo = "Sin motivo especificado";

            string responsable = LeerLinea("Responsable que registra la cancelación (nombre/usuario): ");
            if (string.IsNullOrWhiteSpace(responsable)) responsable = "No registrado";

            int filas = EjecutarComando(
                @"UPDATE Turnos
                  SET estado                   = 'cancelado',
                      motivo_cancelacion        = @motivo,
                      responsable_cancelacion   = @responsable
                  WHERE id_turno = @id",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@motivo", motivo);
                    cmd.Parameters.AddWithValue("@responsable", responsable);
                    cmd.Parameters.AddWithValue("@id", idTurno);
                });

            if (filas > 0)
            {
                MostrarExito("Turno cancelado. El registro permanece en el historial (RN-07).");
                Console.WriteLine($"  Motivo: {motivo}");
                Console.WriteLine($"  Responsable: {responsable}");
            }
            else
            {
                MostrarError("No se pudo cancelar el turno.");
            }

            Pausa();
        }

        // MARCAR ATENDIDO
        static void MarcarTurnoAtendido()
        {
            Console.Clear();
            Console.WriteLine("─── MARCAR TURNO COMO ATENDIDO (RN-06) ───\n");

            int idTurno = LeerEntero("ID del turno: ");
            string estadoActual = ObtenerEstadoTurno(idTurno);
            if (estadoActual == null)
            {
                MostrarError("No se encontró ningún turno con ese ID.");
                Pausa(); return;
            }
            if (estadoActual != "reservado")
            {
                MostrarError($"Solo se pueden marcar como atendidos los turnos 'reservados'. Estado actual: '{estadoActual}'. (RN-06)");
                Pausa(); return;
            }

            int filas = EjecutarComando("UPDATE Turnos SET estado='atendido' WHERE id_turno=@id", cmd => cmd.Parameters.AddWithValue("@id", idTurno));
            if (filas > 0)
                MostrarExito("Turno marcado como 'atendido'.");
            else
                MostrarError("No se pudo actualizar el turno.");

            Pausa();
        }

        static string ObtenerEstadoTurno(int idTurno)
        {
            using (var conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();
                using (var cmd = new SQLiteCommand("SELECT estado FROM Turnos WHERE id_turno=@id", conexion))
                {
                    cmd.Parameters.AddWithValue("@id", idTurno);
                    object resultado = cmd.ExecuteScalar();
                    return resultado == null || resultado == DBNull.Value ? null : resultado.ToString();
                }
            }
        }

        // Datos iniciales
        static void CargarDatosIniciales()
        {
            long count = EjecutarEscalar<long>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Especialidades'", _ => { });
            // Si no existe la tabla, asumimos DB vacía: intentamos crear tablas mínimas (evitar excepción si DB está bien formada).
            if (count == 0)
            {
                // No se crean esquemas completos aquí; se asume que el proyecto ya dispone del esquema.
                // Si se desea crear tablas automáticamente, agregar DDL aquí.
            }

            // Si ya hay filas en Especialidades, no insertamos datos de ejemplo.
            long c2 = EjecutarEscalar<long>("SELECT COUNT(*) FROM Especialidades", _ => { });
            if (c2 > 0) return;

            EjecutarComando(
                @"INSERT INTO Especialidades (nombre, duracion_turno) VALUES
                  ('Clínica médica', 20),
                  ('Cardiología',    30),
                  ('Psicología',     50),
                  ('Pediatría',      20),
                  ('Traumatología',  30);",
                _ => { });

            EjecutarComando(
                @"INSERT INTO Medicos (matricula, nombre_completo, activo) VALUES
                  ('MAT-5543', 'Dr. Carlos Gómez',  1),
                  ('MAT-9921', 'Dra. Ana Restrepo', 1);",
                _ => { });

            EjecutarComando(
                @"INSERT INTO Medicos_Especialidades (id_medico, id_especialidad) VALUES
                  (1, 1), (1, 2), (2, 3);",
                _ => { });
        }
    }
}
