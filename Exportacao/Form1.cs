using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FileProcessor
{
    public partial class MainForm : Form
    {
        private string folderPath = @"D:\csv"; // Padr�o para a pasta inicial
        private string exportPath = ""; // Vari�vel para armazenar o caminho de exporta��o

        // Componentes da interface
        private Button btnExportCSV;
        private ListBox listBoxFiles;
        private DataGridView dataGridViewCSV;

        public MainForm()
        {
            InitializeComponents();

            this.StartPosition = FormStartPosition.CenterScreen; // Centraliza o formul�rio na tela
            this.Size = new Size(1280, 720); // Define a largura e altura do formul�rio
            this.MinimumSize = new Size(600, 400); // Define o tamanho m�nimo do formul�rio

            // carrega os arquivos da pasta
            LoadCSVFiles();
        }

        private void InitializeComponents()
        {
            // ListBox para listar os arquivos CSV
            listBoxFiles = new ListBox();
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.Location = new System.Drawing.Point(20, 60);
            listBoxFiles.Size = new System.Drawing.Size(200, 200);
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged; // Evento ao selecionar um arquivo
            this.Controls.Add(listBoxFiles);

            // DataGridView para visualiza��o do CSV
            dataGridViewCSV = new DataGridView();
            dataGridViewCSV.Location = new System.Drawing.Point(240, 60);
            dataGridViewCSV.Size = new System.Drawing.Size(750, 200);
            dataGridViewCSV.ReadOnly = true;
            this.Controls.Add(dataGridViewCSV);

            // botao de exportar
            btnExportCSV = new Button();
            btnExportCSV.Text = "Exportar CSV Selecionado";
            btnExportCSV.Location = new System.Drawing.Point(20, 280);
            btnExportCSV.Click += btnExportCSV_Click;
            this.Controls.Add(btnExportCSV);
        }

        private void LoadCSVFiles()
        {
            // Verifica se o diret�rio padr�o existe
            if (Directory.Exists(folderPath))
            {
                // Lista os arquivos CSV na pasta selecionada
                List<string> csvFiles = Directory.GetFiles(folderPath, "*.csv")
                                                .Select(Path.GetFileName)
                                                .ToList();

                // Mostra os arquivos CSV na ListBox
                listBoxFiles.DataSource = csvFiles;
            }
            else
            {
                MessageBox.Show($"O diret�rio padr�o {folderPath} n�o existe.", "Diret�rio n�o encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Quando um arquivo � selecionado na ListBox, exibe seu conte�do no DataGridView
            if (listBoxFiles.SelectedItem != null)
            {
                string selectedFile = Path.Combine(folderPath, listBoxFiles.SelectedItem.ToString());

                // Carrega o conte�do do arquivo CSV no DataGridView
                LoadCSVIntoDataGridView(selectedFile);
            }
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            // Verifica se um arquivo foi selecionado na ListBox
            if (listBoxFiles.SelectedItem != null)
            {
                string selectedFile = Path.Combine(folderPath, listBoxFiles.SelectedItem.ToString());

                // Configura��o do SaveFileDialog para escolher onde salvar o arquivo exportado
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Arquivo CSV (*.csv)|*.csv";
                    saveDialog.FileName = "exportado.csv"; // Nome padr�o do arquivo exportado

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        exportPath = saveDialog.FileName;

                        // Exporta o arquivo CSV
                        ExportCSV(selectedFile);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um arquivo CSV para exportar.", "Nenhum arquivo selecionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadCSVIntoDataGridView(string filePath)
        {
            try
            {
                DataTable dataTable = ReadCSV(filePath);
                dataGridViewCSV.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o arquivo CSV: " + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCSV(string filePath)
        {
            try
            {
                // Leitura do arquivo CSV para obter os dados
                DataTable dataTable = ReadCSV(filePath);

                // Exporta o arquivo CSV
                using (StreamWriter sw = new StreamWriter(exportPath))
                {
                    // Escreve o cabe�alho
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        sw.Write(column.ColumnName + ",");
                    }
                    sw.WriteLine();

                    // Escreve os dados
                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            sw.Write(row[i].ToString() + ",");
                        }
                        sw.WriteLine();
                    }
                }

                MessageBox.Show("Arquivo CSV exportado com sucesso.", "Exporta��o conclu�da", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar o arquivo CSV: " + ex.Message, "Erro de exporta��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ReadCSV(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // L� a primeira linha para configurar as colunas
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // L� o restante das linhas
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dataTable.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ler o arquivo CSV: " + ex.Message, "Erro de leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }
    }
}
