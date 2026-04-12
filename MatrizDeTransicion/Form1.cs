using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace MatrizDeTransicion
{
    public partial class Form1 : Form
    {
        // --- 1. CONFIGURACIÓN DE CONEXIÓN ---

        private string connectionString = @"Server=DESKTOP-3G6AMVL\SQLEXPRESS; Database=NovaNyx; Integrated Security=True; TrustServerCertificate=True;";

        // --- 2. VARIABLES DE LÓGICA ---

        private Dictionary<string, string> mapaColumnas = new Dictionary<string, string>(); 
        private List<string> alfabeto = new List<string>();
        private int[,] matriz;
        private Dictionary<int, string> categoriasEstado = new Dictionary<int, string>();
        private int maxEstado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Se usan los metdos combinados para detectar la estructura y cargar los datos
                CargarEstructuraYDatosDesdeSQL();
                CargarMatrizEnGrid();
                lblEstado.Text = "Estructura detectada y matriz cargada desde SQL Server.";
                lblEstado.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar o procesar la tabla:\n\n" + ex.Message);
                lblEstado.Text = "Error de conexión.";
                lblEstado.ForeColor = Color.Red;
            }
        }

        //Se combinan los pasos de detección de estructura y carga de datos en un solo método para mayor eficiencia y simplicidad
        private void CargarEstructuraYDatosDesdeSQL()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Obtenemos todos los datos de la tabla MatrizTransicion para analizar su estructura y contenido
                string query = "SELECT * FROM MatrizTransicion ORDER BY 1";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable tablaDB = new DataTable();
                adapter.Fill(tablaDB);

                // Validación básica para asegurar que la tabla no esté vacía
                if (tablaDB.Rows.Count == 0)
                    throw new Exception("La tabla MatrizTransicion está vacía.");

                // 2. DETECCIÓN AUTOMÁTICA DEL ALFABETO
                alfabeto.Clear();
                foreach (DataColumn col in tablaDB.Columns)
                {
                    // El nombre real de la columna en la base de datos en la iteración actual
                    string nombreReal = col.ColumnName;

                    // Saltamos la columna ID (índice 0) y ACEPTA
                    if (nombreReal != tablaDB.Columns[0].ColumnName && nombreReal.ToUpper() != "ACEPTA")
                    {
                        //Si es 'a1' y mide 2 caracteres, lo limpiamos a 'a'
                        string nombreLimpio = nombreReal;
                        if (nombreReal.Length == 2 && nombreReal.EndsWith("1"))
                        {
                            nombreLimpio = nombreReal.Substring(0, 1);
                        }

                        alfabeto.Add(nombreLimpio);// Agregamos el símbolo limpio al alfabeto
                        mapaColumnas[nombreLimpio] = nombreReal; // Guardamos la relación para cuando el usuario ejemplo ingrese 'a' y nosotros busquemos 'a1' en la tabla
                    }
                }

                // 3. DIMENSIONAR MATRIZ
                // Buscamos la fila con el ID más alto para determinar el número de estados (filas) que necesitamos en la matriz
                int realMaxId = 0;
                foreach (DataRow row in tablaDB.Rows)
                {
                    if (int.TryParse(row[0].ToString(), out int id) && id > realMaxId)
                        realMaxId = id;
                }
                // Guardamos el número máximo de estados detectado para usarlo en la lógica de análisis
                maxEstado = realMaxId;
                // Dimensionamos la matriz con base en el número máximo de estados detectado y el tamaño del alfabeto
                matriz = new int[maxEstado + 2, alfabeto.Count];

                // 4. LLENAR LA MATRIZ CON LOS VALORES DE LAS CELDAS
                foreach (DataRow row in tablaDB.Rows)
                {
                    // Obtenemos el ID del estado actual desde la primera columna de la fila
                    if (!int.TryParse(row[0].ToString(), out int idEstadoActual)) continue;


                    for (int i = 0; i < alfabeto.Count; i++)
                    {
                        string simbolo = alfabeto[i];
                        object valorCelda = row[simbolo];

                        if (valorCelda == DBNull.Value ||
                            string.IsNullOrWhiteSpace(valorCelda.ToString()) ||
                            valorCelda.ToString().ToLower() == "error")
                        {
                            matriz[idEstadoActual, i] = -1; // -1 significa transición a ERROR
                        }
                        else
                        {
                            if (int.TryParse(valorCelda.ToString(), out int destino))
                                matriz[idEstadoActual, i] = destino;
                            else
                                matriz[idEstadoActual, i] = -1;
                        }
                    }

                    // Guardar categoría de aceptación
                    string acepta = row["ACEPTA"]?.ToString() ?? "No valido";
                    categoriasEstado[idEstadoActual] = acepta;
                }
            }
        }

        private void CargarMatrizEnGrid()
        {
            if (dgvMatriz == null) return;

            dgvMatriz.Columns.Clear();
            dgvMatriz.Rows.Clear();

            // Configurar columnas visuales
            dgvMatriz.Columns.Add("Estado", "No.");
            foreach (var simbolo in alfabeto)
            {
                dgvMatriz.Columns.Add(simbolo, simbolo);
            }
            dgvMatriz.Columns.Add("ACEPTA", "Categoría");

            // Llenar filas visuales
            var estadosOrdenados = categoriasEstado.Keys.OrderBy(k => k);
            foreach (var estadoId in estadosOrdenados)
            {
                string[] fila = new string[alfabeto.Count + 2];
                fila[0] = estadoId.ToString();
                for (int j = 0; j < alfabeto.Count; j++)
                {
                    int val = matriz[estadoId, j];
                    fila[j + 1] = (val == -1) ? "Error" : val.ToString();
                }
                fila[alfabeto.Count + 1] = categoriasEstado[estadoId];
                dgvMatriz.Rows.Add(fila);
            }
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            string entrada = txtEntrada.Text;
            if (lstLog != null) lstLog.Items.Clear();
            if (string.IsNullOrEmpty(entrada)) return;

            int estadoActual = 1; // Punto de entrada estándar
            bool errorDetectado = false;

            // Procesamos cada carácter de la cadena de entrada
            foreach (char c in entrada)
            {
                string s = c.ToString();
                int col = alfabeto.IndexOf(s);

                // Si el carácter no está en las columnas de la tabla
                if (col == -1)
                {
                    lstLog?.Items.Add($"Error: El símbolo '{c}' no existe en la estructura de la tabla.");
                    errorDetectado = true;
                    break;
                }

                // Validación de límites de matriz
                if (estadoActual < 0 || estadoActual > maxEstado)
                {
                    errorDetectado = true;
                    break;
                }

                // Obtenemos el siguiente estado desde la matriz de transición
                int siguiente = matriz[estadoActual, col];
                // Si la transición no está definida (valor -1), se considera un error
                if (siguiente == -1)
                {
                    lstLog?.Items.Add($"Q{estadoActual} --('{c}')--> ERROR (Transición no definida)");
                    estadoActual = -1;
                    errorDetectado = true;
                    break;
                }
                // Si la transición es válida, se registra el movimiento y se actualiza el estado actual
                lstLog?.Items.Add($"Q{estadoActual} --('{c}')--> Q{siguiente}");
                estadoActual = siguiente;
            }

            // Procesamiento de fin de cadena (EOC)
            if (!errorDetectado)
            {
                int colEOC = alfabeto.FindIndex(x => x.ToUpper() == "EOC");
                if (colEOC != -1)
                {
                    int estadoFinal = matriz[estadoActual, colEOC];
                    if (estadoFinal != -1)
                    {
                        lstLog?.Items.Add($"EOC: Q{estadoActual} --> Q{estadoFinal}");
                        estadoActual = estadoFinal;
                    }
                }
            }

            // Resultado final según la columna ACEPTA de la fila alcanzada
            if (estadoActual != -1 && categoriasEstado.ContainsKey(estadoActual))
            {
                string cat = categoriasEstado[estadoActual];
                lblEstado.Text = "RESULTADO: " + cat;

                bool esValido = !cat.ToLower().Contains("no valido") && !cat.ToLower().Contains("error");
                lblEstado.ForeColor = esValido ? Color.Green : Color.Red;
            }
            else
            {
                lblEstado.Text = "RESULTADO: Cadena Inválida";
                lblEstado.ForeColor = Color.Red;
            }
        }
    }
}
