using System;
using ExtentionProjekt;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace FileDialog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        FolderBrowserDialog FolderBrowserDialog = new FolderBrowserDialog();
        /// <summary>
        /// Represent the last pressed Key.
        /// </summary>
        Key lastkey;
        bool fileExist = false;
        /// <summary>
        /// Property FileExist:
        /// Represent a bool-value that say if "myList" contains a file-path already. Sets btnfavortie Content - true: Remove, false: Add 
        /// </summary>
        public bool FileExist
        {
            get { return fileExist; }
            set
            {
                fileExist = value;
                btnfavortie.Content = value ? "Remove" : "Add";
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|C# files (*.cs)|*.cs|All files (*.*)|*.*"; //Selectable file filter - OpenFileDialog
            if (!(File.Exists(@"Starttxt.txt"))) File.Create(@"Starttxt.txt").Close(); //if "Starttxt.txt" doesn't exist: create file "Starttxt.txt

            string[] sort = File.ReadAllText(@"Starttxt.txt").Split(',');
            foreach (string s in sort)
                _ = File.Exists(s) ? myList.Items.Add(s) : 0; //Add every Existing path from textfile "Starttxt.txt" in "myList"

            MenuNew.Click += (s, e) => New(); //Click EventHandler - Menupoint "New". C.: Start method "New();", No parameteres
            MenuOpen.Click += (s, e) => _ = openFileDialog.ShowDialog() == true ? NewFileSelected(true, false, openFileDialog.FileName) : false;
            //Click EventHandler - Menupoint "Open". C.: Select File with "OpenFileDialog". If file selected: start method "NewFileSelected
            MenuDelete.Click += (s, e) => Delete();
            MenuSave.Click += (s, e) => myList.Items.Save(); //Click EventHandler - Menupoint "Save".
            MenuExit.Click += (s, e) => Close(); //Click EventHandler - Menupoint "Exit.
            btnClear.Click += (s, e) => myList.Items.Clear();  //Click EventHandler - Button "Clear".
        }
        /// <summary>
        /// Display costum dialog "Eingabe". Add file with the string-path "Ausgabe" from Eingabe in myList.
        /// </summary>
        private void New()
        {
            Eingabe eingabe = new Eingabe();
            eingabe.ShowDialog();
            if (eingabe.Ausgabe != null)
            {
                myList.Items.Add(myList.SelectedItem = eingabe.Ausgabe);
            }
        }
        private void Delete()
        {
            if (path.Text.FileExists() == true)
                myList.Items.Remove(path.Text);
            File.Delete(path.Text);
        }

        /// <summary>
        /// Add new path in myList or write content of pathText(.txt) in txtEditor.Text.
        /// </summary>
        /// <param name="write">boolean-value write content of pathText(.txt) in txtEditor.Text</param>
        /// <param name="list">boolean-value add path in myList</param>
        /// <param name="pathText">string-value representing the file-path</param>
        /// <returns></returns>
        private bool NewFileSelected(bool write, bool list, string pathText)
        {
            try
            {
                if (list)
                    myList.Items.Add(myList.SelectedItem = pathText);
                txtEditor.Text = write ? File.ReadAllText(path.Text = pathText) : txtEditor.Text;
            }
            catch (Exception exe)
            {
                exe.ErrorMessage();
            }
            return true;
        }
        /// <summary>
        /// Write content of path.Text in txtEditor when file path.Text exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void path_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEditor.Text = path.Text.FileExists() ? File.ReadAllText(path.Text) : String.Empty;
            MenuDelete.IsEnabled = File.Exists(path.Text);
            FileExist = myList.Items.Contains(path.Text);
        }
        /// <summary>
        /// Overwrite content from path.Text(.txt) by change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (path.Text.FileExists())
                File.WriteAllText(path.Text, txtEditor.Text);
        }
        /// <summary>
        /// Write content from the currently selected path in myList. Set value of FileExist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myList.SelectedItem != null)
                NewFileSelected(true, false, myList.SelectedItem.ToString());
        }
        /// <summary>
        /// Add selected file from OpenFileDialog in myList, when myList doesn't contain item already.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true && !(myList.Items.Contains(openFileDialog.FileName)))
                NewFileSelected(false, true, openFileDialog.FileName);
        }
        /// <summary>
        /// Add/Remove current path written in path.Text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnfavortie_Click(object sender, RoutedEventArgs e)
        {
            if (FileExist) myList.Items.Remove(myList.SelectedItem.ToString());
            else if (File.Exists(path.Text))
                NewFileSelected(false, true, path.Text);
        }
        /// <summary>
        /// Method for shortkeys in the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (lastkey == Key.LeftCtrl && e.Key == Key.S)
                myList.Items.Save();
            lastkey = e.Key;
        }
    }
}
namespace ExtentionProjekt
{
    static class Extentions
    {
        public static void ErrorMessage(this Exception exe) => MessageBox.Show(exe.Message); //Show exception-description by exception
        public static bool FileExists(this string FilePath)
        {
            return File.Exists(FilePath);
        }
        /// <summary>
        /// Method which write all current items from myList in textfile "Starttxt.txt".
        /// </summary>
        public static void Save(this ItemCollection myList)
        {
            string word = "";
            foreach (var item in myList)
                word += item.ToString() + ",";
            File.WriteAllText(@"Starttxt.txt", word);
        }
    }
}
