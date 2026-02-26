using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Maxon_Mechanic;Integrated Security=True;TrustServerCertificate=True";

        public Form2(int role, string fio)
        {
            InitializeComponent();

            label1.Text = fio;

            btnAdd.Visible = btnEdit.Visible = btnDelete.Visible = (role == 1);

            txtSearch.TextChanged += (s, e) => LoadProducts();
            cmbSort.SelectedIndexChanged += (s, e) => LoadProducts();

            LoadProducts();
        }

        public void LoadProducts()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Tag = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string sql = @"
SELECT p.*, 
       pr.name as prN, 
       prov.name as pvN
FROM Product p
LEFT JOIN Producer pr ON p.producerid = pr.id
LEFT JOIN Provider prov ON p.providerid = prov.id
WHERE p.name LIKE @s
";

                    if (cmbSort.SelectedIndex == 0)
                        sql += " ORDER BY price ASC";

                    if (cmbSort.SelectedIndex == 1)
                        sql += " ORDER BY price DESC";


                    SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@s", "%" + txtSearch.Text + "%");

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            Producties card = new Producties();

                            card.FillData(
                                r["name"]?.ToString(),
                                r["description"]?.ToString(),
                                r["prN"]?.ToString(),
                                r["pvN"]?.ToString(),
                                r["price"] != DBNull.Value ? Convert.ToDecimal(r["price"]) : 0,
                                r["discount"] != DBNull.Value ? Convert.ToInt32(r["discount"]) : 0,
                                "шт.",
                                r["amountidstock"] != DBNull.Value ? Convert.ToInt32(r["amountidstock"]) : 0,
                                r["photo"]?.ToString()
                            );

                            card.Tag = Convert.ToInt32(r["id"]);

                            // ВАЖНО — обработчик клика
                            card.Click += Card_Click;

                            foreach (Control c in card.Controls)
                                c.Click += Card_Click;

                            flowLayoutPanel1.Controls.Add(card);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message);
                }
            }
        }


        private void Card_Click(object sender, EventArgs e)
        {
            Control clicked = sender as Control;

            while (clicked != null && !(clicked is Producties))
                clicked = clicked.Parent;

            if (clicked != null)
            {
                foreach (Control ct in flowLayoutPanel1.Controls)
                    ct.BackColor = Color.White;

                clicked.BackColor = Color.LightGray;

                flowLayoutPanel1.Tag = clicked.Tag;
            }
        }


        private void OpenEdit(int? id)
        {
            Form3 f = new Form3(id);

            if (f.ShowDialog() == DialogResult.OK)
                LoadProducts();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenEdit(null);
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Tag == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            OpenEdit((int)flowLayoutPanel1.Tag);
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Tag == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            int id = (int)flowLayoutPanel1.Tag;

            if (MessageBox.Show("Удалить товар?", "Удаление",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    SqlCommand cmd =
                        new SqlCommand("DELETE FROM Product WHERE id=@id", con);

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();

                    LoadProducts();
                }
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int role = 3; // По умолчанию гость

            if (label1.Text.Contains("Администратор")) role = 1;
            else if (label1.Text.Contains("Менеджер")) role = 2;

            // Открываем форму со списком заказов (Form4)
            Form4 f4 = new Form4(role);
            f4.Show();
        }
    }
}
