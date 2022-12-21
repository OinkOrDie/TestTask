using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;

namespace TestTask
{
    public partial class Form1 : Form
    {

        private SqlConnection sqlConnection = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private SqlCommand sqlCommand = null;
        private DataSet dataSet = null;

        //Вывод таблицы в DataGridView с фильтрацией
        private void DataGridFill() 
        {
            using (sqlCommand = new SqlCommand("Pr_Select", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@PersonName", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@PersonStatus", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@PersonPost", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@PersonDep", SqlDbType.VarChar, 100);
                sqlCommand.Parameters["@PersonName"].Value = textBoxNameFilter.Text;
                sqlCommand.Parameters["@PersonStatus"].Value = comboBoxStatusFilter.Text;
                sqlCommand.Parameters["@PersonPost"].Value = comboBoxPostFilter.Text;
                sqlCommand.Parameters["@PersonDep"].Value = comboBoxDepFilter.Text;
                try
                {
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dataGridView1.DataSource = dataSet.Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        } 

        //Вывод таблицы в dataGridView со статистикой
        private void DataGridFillStats(string Proc_name)
        {
            using (sqlCommand = new SqlCommand(Proc_name, sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@Status", SqlDbType.VarChar, 100);
                sqlCommand.Parameters.Add("@Date1", SqlDbType.DateTime);
                sqlCommand.Parameters.Add("@Date2", SqlDbType.DateTime);
                sqlCommand.Parameters["@Status"].Value = comboBoxStatusStats.Text;
                sqlCommand.Parameters["@Date1"].Value = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                sqlCommand.Parameters["@Date2"].Value = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                try
                {
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);
                    dataGridView1.DataSource = dataSet.Tables[0];
                    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //Метод, заполняющий comboBox списком опций из БД
        private void ComboBoxFill(ComboBox comboBox, string param) 
        {
            using (sqlCommand = new SqlCommand(param, sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                try
                {
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    dataSet = new DataSet();
                    sqlDataAdapter.Fill(dataSet);

                    List<string> strDetailIDList = new List<string>();
                    foreach (DataRow cell in dataSet.Tables[0].Rows)
                    {
                        strDetailIDList.Add(cell[0].ToString());
                    }
                    comboBox.Items.AddRange(strDetailIDList.ToArray());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["EmployeeDB"].ConnectionString);
                sqlConnection.Open();

                DataGridFill();
                ComboBoxFill(comboBoxStatusFilter, "Pr_StatusNames");
                ComboBoxFill(comboBoxStatusStats, "Pr_StatusNames");
                ComboBoxFill(comboBoxPostFilter, "Pr_PostsNames");
                ComboBoxFill(comboBoxDepFilter, "Pr_DepsNames");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Проверьте, правильно ли введена строка подключения в App.config");
            }
        }

        //Кнопка показать статистику
        private void showStatsButton_Click(object sender, EventArgs e)
        {
            if (comboBoxHiredFiredStats.SelectedIndex < 0 || comboBoxStatusStats.SelectedIndex < 0)
            {
                MessageBox.Show("Заполните все поля для вывода статистики");
                return;
            }
            if (comboBoxHiredFiredStats.SelectedIndex == 0)
            {
                DataGridFillStats("Pr_EmployStats");
                textBox2.Text = "Количество работников нанятых с " + dateTimePicker1.Value.ToLongDateString() +
                    " по " + dateTimePicker2.Value.ToLongDateString() + " имеющих статус '" +
                    comboBoxStatusStats.Text + "': " + dataGridView1.RowCount.ToString();
            }
            else {
                DataGridFillStats("Pr_UneployStats");
                textBox2.Text = "Количество работников уволенных с " + dateTimePicker1.Value.ToLongDateString() +
                    " по " + dateTimePicker2.Value.ToLongDateString() + " имеющих статус '" +
                    comboBoxStatusStats.Text + "': " + dataGridView1.RowCount.ToString();
            };
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            DataGridFill();
        }

        //Очистка фильтров
        private void buttonClearFilter_Click(object sender, EventArgs e)
        {
            comboBoxPostFilter.SelectedIndex = -1;
            comboBoxDepFilter.SelectedIndex = -1;
            comboBoxStatusFilter.SelectedIndex = -1;
            textBoxNameFilter.Text = "";
            DataGridFill();
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowHelp(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void ConnectionState(object sender, EventArgs e)
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                MessageBox.Show("Подключение устанвленно");
            }
            else MessageBox.Show("Подклюяение не установленно"); 
        }
    }   
}
