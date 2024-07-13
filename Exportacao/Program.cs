using System;
using System.Windows.Forms;

namespace FileProcessor
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cria e inicia o formulário principal
            Application.Run(new MainForm());
        }
    }
}
