using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace OstatnieZadanie
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OracleConnection conn;
        string checkedTable;

        public MainWindow()
        {
            InitializeComponent();
            SetConnection();
        }

        private void SetConnection()
        {
            //conn = new OracleConnection("User Id=RedactedID;Password=RedactedPassword;Data Source=RedactedIP/orcltp.iaii.local");
        }

        private void FillDataGrid(string selectedTable)
        {
            checkedTable = selectedTable;

            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = $"SELECT * FROM {selectedTable}";
            cmd.Connection = conn;
            OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(cmd);
            DataTable dt = new DataTable();
            oracleDataAdapter.Fill(dt);
            Tablica.ItemsSource = dt.DefaultView;

            Tablica.RowBackground = Brushes.White;

            MessageBoxResult result = MessageBox.Show("                     Zapisać ?", "Eksport XML", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveToXML();
                    break;
                case MessageBoxResult.No:
                    break;
            }

            conn.Close();
        }

        private void ShowKartoteka(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Kartoteka");
        }

        private void ShowLekarstwo(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Lekarstwo");
        }

        private void ShowPacjent(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Pacjent");
        }

        private void ShowPracownik(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Pracownik");
        }

        private void ShowRecepta(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Recepta");
        }

        private void ShowRecepta_Lekarstwo(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Recepta_Lekarstwo");
        }

        private void ShowSala(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Sala");
        }

        private void ShowStanowisko(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Stanowisko");
        }

        private void ShowWizyta(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Wizyta");
        }

        private void ShowZabieg(object sender, RoutedEventArgs e)
        {
            FillDataGrid("Zabieg");
        }

      
        private void SaveToXML()
        {
            DataTable dt = new DataTable();
            dt = ((DataView)Tablica.ItemsSource).ToTable();
            dt.TableName = checkedTable;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"{checkedTable}_XML";
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "Plik XML (*.xml)|*.xml";

            System.Nullable<bool> result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                dt.WriteXml(saveFileDialog.FileName, XmlWriteMode.WriteSchema);
            }

        }

        private void ReadFromXML(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "Plik XML (*.xml)|*.xml";

            System.Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                dt.ReadXml(openFileDialog.FileName);
                Tablica.ItemsSource = dt.DefaultView;
                Tablica.RowBackground = new SolidColorBrush(Color.FromRgb(153, 204, 255));

                MessageBoxResult messageBoxResult = MessageBox.Show("Zapisać?", "Eksport", MessageBoxButton.YesNo);
                switch (messageBoxResult)
                {
                    case MessageBoxResult.Yes:
                        SaveToDatabase(dt, dt.TableName);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void SaveToDatabase(DataTable dt, String tableToSave)
        {
            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;

            if (tableToSave == "kartoteka")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_kartoteka = dt.Rows[i].ItemArray[0];
                        var uczulenia = dt.Rows[i].ItemArray[1];
                        var brane_leki = dt.Rows[i].ItemArray[2];
                        var choroby_przeszłe = dt.Rows[i].ItemArray[3];
                        var choroby_przewlekłe = dt.Rows[i].ItemArray[4];


                        cmd.CommandText = $"INSERT INTO kartoteka VALUES({id_kartoteka}, '{uczulenia}', '{brane_leki}', '{choroby_przeszłe}', '{choroby_przewlekłe}')";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "lekarstwo")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_lekarstwo = dt.Rows[i].ItemArray[0];
                        var nazwa = dt.Rows[i].ItemArray[1];
                        var koszt = dt.Rows[i].ItemArray[2];
                        var zastosowanie = dt.Rows[i].ItemArray[3];
                        var przeciwwskazania = dt.Rows[i].ItemArray[4];
                        

                        cmd.CommandText = $"INSERT INTO lekarstwo VALUES({id_lekarstwo}, '{nazwa}', '{koszt}', '{zastosowanie}', '{przeciwwskazania}')";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Pacjent")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_pacjent = dt.Rows[i].ItemArray[0];
                        var imie = dt.Rows[i].ItemArray[1];
                        var nazwisko = dt.Rows[i].ItemArray[2];
                        var pesel = dt.Rows[i].ItemArray[3];
                        var data_urodzenia = dt.Rows[i].ItemArray[4];
                        var telefon = dt.Rows[i].ItemArray[5];
                        var email = dt.Rows[i].ItemArray[6];
                        var kartoteka_id_kartoteka = dt.Rows[i].ItemArray[7];


                        cmd.CommandText = $"INSERT INTO Pacjent VALUES({id_pacjent}, '{imie}', '{nazwisko}', '{pesel}', {data_urodzenia}, '{telefon}', '{email}', {kartoteka_id_kartoteka})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Pracownik")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_pracownik = dt.Rows[i].ItemArray[0];
                        var imie = dt.Rows[i].ItemArray[1];
                        var nazwisko = dt.Rows[i].ItemArray[2];
                        var telefon = dt.Rows[i].ItemArray[3];
                        var pesel = dt.Rows[i].ItemArray[4];
                        var data_zatrudnienia = dt.Rows[i].ItemArray[5];
                        var data_zwolnienia = dt.Rows[i].ItemArray[6];
                        var stanowisko_id_stanowisko = dt.Rows[i].ItemArray[7];


                        cmd.CommandText = $"INSERT INTO Pracownik VALUES({id_pracownik}, '{imie}', '{nazwisko}', '{telefon}', '{pesel}', {data_zatrudnienia}, {data_zwolnienia}, {stanowisko_id_stanowisko})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Recepta")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_recepta = dt.Rows[i].ItemArray[0];
                        var data_waznosci = dt.Rows[i].ItemArray[1];
                        

                        cmd.CommandText = $"INSERT INTO Recepta VALUES({id_recepta}, {data_waznosci})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Recepta_Lekarstwo")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var lekarstwo_id_lekarstwo = dt.Rows[i].ItemArray[0];
                        var recepta_id_recepta = dt.Rows[i].ItemArray[1];


                        cmd.CommandText = $"INSERT INTO Recepta_Lekarstwo VALUES({lekarstwo_id_lekarstwo}, {recepta_id_recepta})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Sala")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_sala = dt.Rows[i].ItemArray[0];
                        var numer_sala = dt.Rows[i].ItemArray[10];
                        var wyposazenie = dt.Rows[i].ItemArray[10];
                        
                        cmd.CommandText = $"INSERT INTO Sala VALUES({id_sala}, {numer_sala},  '{wyposazenie}')";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Stanowisko")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_stanowisko = dt.Rows[i].ItemArray[0];
                        var tytul = dt.Rows[i].ItemArray[1];
                        var pensja = dt.Rows[i].ItemArray[2];

                        cmd.CommandText = $"INSERT INTO Stanowisko VALUES({id_stanowisko}, '{tytul}',{pensja})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Wizyta")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_wizyta = dt.Rows[i].ItemArray[0];
                        var data = dt.Rows[i].ItemArray[1];
                        var pacjent_id_pacjent = dt.Rows[i].ItemArray[2];
                        var pracownik_id_pracownik = dt.Rows[i].ItemArray[3];
                        var zabieg_id_zabieg = dt.Rows[i].ItemArray[4];
                        var recepta_id_recepta = dt.Rows[i].ItemArray[5];
                        var sala_id_sala = dt.Rows[i].ItemArray[6];

                        cmd.CommandText = $"INSERT INTO Wizyta VALUES({id_wizyta}, {data}, {pacjent_id_pacjent}, {pracownik_id_pracownik}, {zabieg_id_zabieg}, {recepta_id_recepta}, {sala_id_sala})";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }

            if (tableToSave == "Zabieg")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        var id_zabieg = dt.Rows[i].ItemArray[0];
                        var nazwa = dt.Rows[i].ItemArray[1];
                        var koszt = dt.Rows[i].ItemArray[2];
                        var czas = dt.Rows[i].ItemArray[3];
                        var opis = dt.Rows[i].ItemArray[4];

                        cmd.CommandText = $"INSERT INTO Zabieg VALUES({id_zabieg}, '{nazwa}', {koszt}, {czas}, '{opis}')";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Uwaga błąd! " + ex.Message, "Coś poszło nie tak!", MessageBoxButton.OK);
                        break;
                    }
                }
            }
            conn.Close();
        }

        private void Tablica_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
