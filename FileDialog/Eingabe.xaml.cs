using System;
using System.IO;
using System.Windows;


namespace FileDialog
{
    /// <summary>
    /// Interaktionslogik für Eingabe.xaml
    /// </summary>
    public partial class Eingabe : Window
    {
        private string ausgabe;
        public string Ausgabe { get; set; }
        public Eingabe()
        {
            InitializeComponent();
            txtBox.Text = Path.GetFullPath(@"#");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ausgabe = txtBox.Text;
                File.Create(Ausgabe).Close();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
