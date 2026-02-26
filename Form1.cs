using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using WinFormsApp1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1 
{
    public partial class Form1 : Form
    {
        // НИЖЕ СТРОКА ДЛЯ ПОДКЛЮЧЕНИЯ БД, КОТОРУЮ МЫ СОЗДАЛИ РАНЕЕ, НАЗВАНИЕ СЕРВЕРА ПОСМОТРИТЕ В СВОЙСТВАХ В SSMS 22(У МЕНЯ ТАКОЕ - WIN-PUURG92IVC5\SQLEXPRESS У ВАС МОЖЕТ НАЗЫВАТЬСЯ ИНАЧЕ!!!!)
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Maxon_Mechanic;Integrated Security=True;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
            // ТУТ ШРИФТ ПО ЗАДАНИЮ, МОЖЕТЕ УБРАТЬ ЭТУ СТРОЧКУ КОДА И НАЗНАЧИТЬ В СВОЙСТВАЗ ФОРМЫ И Т.П.
            // Принудительно подписываем кнопки на события

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // НИЖЕ КОД ДЛЯ Кнопки ВОЙТИ
        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Запрос - получаем роль и ФИО пользователя с БД
                    string query = "SELECT roleid, surname, name, patronmic FROM [User] WHERE login = @log AND password = @pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@log", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pass", textBox2.Text);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int roleId = reader.GetInt32(0);
                        // Формируем ФИО
                        string fio = $"{reader.GetString(1)} {reader.GetString(2)} {reader.GetString(3)}";

                        MessageBox.Show("Авторизация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Переход на вторую форму, к ней приступим ниже в данной методичке
                        Form2 f = new Form2(roleId, fio);
                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(3, "Гость");
            f.Show();
            this.Hide(); // Скрываем окно авторизации
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
