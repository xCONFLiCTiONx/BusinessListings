using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BusinessListings
{
    public partial class Main : Form
    {
        private static BackgroundWorker BGWorker;

        private static SqlConnection sqlConnection;
        private static SqlDataAdapter adaptSponsorship;

        private static int RewardsCount = 0;
        private static int SponsorCount = 0;
        private static int ContactedCount = 0;

        public Main()
        {
            InitializeComponent();

            Shown += Main_Shown;

            WorkerThread();
        }

        #region Prepaint

        private void Main_Shown(object sender, EventArgs e)
        {
            lbl_Loading.Location = new Point((Width / 2) - (lbl_Loading.Width / 2), (Height / 2) - lbl_Loading.Height - 50);
            progressBar1.Location = new Point((Width / 2) - (progressBar1.Width / 2), (Height / 2) - progressBar1.Height - 25);
        }

        private void WorkerThread()
        {
            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += DoWork;
            BGWorker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();

                Properties.Settings.Default.UpgradeRequired = false;

                Properties.Settings.Default.Reload();

                Properties.Settings.Default.Save();
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "sqllocaldb",
                Arguments = "start MSSQLLocalDB",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }

            sqlConnection = new SqlConnection(Properties.Settings.Default.db_BusinessListConnectionString);

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 225, 225, 225);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView2.CellEndEdit += DataGridView2_CellEndEdit;

            dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            dataGridView2.CellContentClick += DataGridView2_CellContentClick;

            BeginInvoke(new MethodInvoker(DisplayData));
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            using (SqlCommand cmd = new SqlCommand("update tbl_BusinessList set Name=@Name,Email=@Email,Website=@Website,Address=@Address,Contacted=@Contacted,Rewards=@Rewards,Sponsor=@Sponsor,Representative=@Representative,Notes=@Notes,Date=@Date where Phone=@Phone", sqlConnection))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                cmd.Parameters.AddWithValue("@Name", dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                cmd.Parameters.AddWithValue("@Phone", dataGridView1.Rows[e.RowIndex].Cells[1].Value);
                cmd.Parameters.AddWithValue("@Email", dataGridView1.Rows[e.RowIndex].Cells[2].Value);
                cmd.Parameters.AddWithValue("@Website", dataGridView1.Rows[e.RowIndex].Cells[3].Value);
                cmd.Parameters.AddWithValue("@Address", dataGridView1.Rows[e.RowIndex].Cells[4].Value);
                cmd.Parameters.AddWithValue("@Contacted", dataGridView1.Rows[e.RowIndex].Cells[5].Value);
                cmd.Parameters.AddWithValue("@Rewards", dataGridView1.Rows[e.RowIndex].Cells[6].Value);
                cmd.Parameters.AddWithValue("@Sponsor", dataGridView1.Rows[e.RowIndex].Cells[7].Value);
                cmd.Parameters.AddWithValue("@Representative", dataGridView1.Rows[e.RowIndex].Cells[8].Value);
                cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[e.RowIndex].Cells[9].Value);
                cmd.Parameters.AddWithValue("@Date", dataGridView1.Rows[e.RowIndex].Cells[10].Value);

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }

            BeginInvoke(new MethodInvoker(DisplayData));
        }

        private void DataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            using (SqlCommand cmd = new SqlCommand("update tbl_ChurchList set Name=@Name,Email=@Email,Website=@Website,Address=@Address,Contacted=@Contacted,Representative=@Representative,Notes=@Notes,Date=@Date where Phone=@Phone", sqlConnection))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                cmd.Parameters.AddWithValue("@Name", dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                cmd.Parameters.AddWithValue("@Phone", dataGridView2.Rows[e.RowIndex].Cells[1].Value);
                cmd.Parameters.AddWithValue("@Email", dataGridView2.Rows[e.RowIndex].Cells[2].Value);
                cmd.Parameters.AddWithValue("@Website", dataGridView2.Rows[e.RowIndex].Cells[3].Value);
                cmd.Parameters.AddWithValue("@Address", dataGridView2.Rows[e.RowIndex].Cells[4].Value);
                cmd.Parameters.AddWithValue("@Contacted", dataGridView2.Rows[e.RowIndex].Cells[5].Value);
                cmd.Parameters.AddWithValue("@Representative", dataGridView2.Rows[e.RowIndex].Cells[6].Value);
                cmd.Parameters.AddWithValue("@Notes", dataGridView2.Rows[e.RowIndex].Cells[7].Value);
                cmd.Parameters.AddWithValue("@Date", dataGridView2.Rows[e.RowIndex].Cells[8].Value);

                cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }

            BeginInvoke(new MethodInvoker(DisplayData));
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[5].Value != null)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells[5].EditedFormattedValue))
                {
                    dataGridView1.Rows[e.RowIndex].Cells[10].Value = DateTime.Now;
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].Cells[10].Value = DBNull.Value;
                }
            }
        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows[e.RowIndex].Cells[5].Value != null)
            {
                if (Convert.ToBoolean(dataGridView2.Rows[e.RowIndex].Cells[5].EditedFormattedValue))
                {
                    dataGridView2.Rows[e.RowIndex].Cells[8].Value = DateTime.Now;
                }
                else
                {
                    dataGridView2.Rows[e.RowIndex].Cells[8].Value = DBNull.Value;
                }
            }
        }

        private void FinalizeLoad()
        {
            // dataGridView1
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.Automatic;

            // dataGridView2
            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;

            progressBar1.Dispose();
            lbl_Loading.Dispose();
            panel2.Dispose();
            menuStrip1.Visible = true;

            BGWorker.DoWork -= DoWork;

            BeginInvoke(new MethodInvoker(UpdateLabels));
        }

        #endregion Prepaint

        private void UpdateLabels()
        {
            RewardsCount = 0;
            SponsorCount = 0;
            ContactedCount = 0;
            var TotalCount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                TotalCount++;

                DataGridViewCheckBoxCell RewardsCell = row.Cells[5] as DataGridViewCheckBoxCell;

                if (RewardsCell.Value != null)
                {
                    if (Convert.ToBoolean(RewardsCell.EditedFormattedValue))
                    {
                        ContactedCount++;
                    }
                }

                DataGridViewCheckBoxCell SponsorCell = row.Cells[6] as DataGridViewCheckBoxCell;

                if (SponsorCell.Value != null)
                {
                    if (Convert.ToBoolean(SponsorCell.EditedFormattedValue))
                    {
                        RewardsCount++;
                    }
                }

                DataGridViewCheckBoxCell ContactedCell = row.Cells[7] as DataGridViewCheckBoxCell;

                if (ContactedCell.Value != null)
                {
                    if (Convert.ToBoolean(ContactedCell.EditedFormattedValue))
                    {
                        SponsorCount++;
                    }
                }
            }

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                TotalCount++;

                DataGridViewCheckBoxCell ChurchCell = row.Cells[5] as DataGridViewCheckBoxCell;

                if (ChurchCell.Value != null)
                {
                    if (Convert.ToBoolean(ChurchCell.EditedFormattedValue))
                    {
                        ContactedCount++;
                    }
                }
            }

            lbl_Contacted.Text = "Total Contacted: " + ContactedCount.ToString();
            lbl_Rewards.Text = "Total Rewards: " + RewardsCount.ToString();
            lbl_Sponsors.Text = "Total Sponsors: " + SponsorCount.ToString();
            lbl_Total.Text = "Total: " + TotalCount.ToString();

            lbl_Contacted.Location = new Point(lbl_Total.Right + 20, 7);
            lbl_Rewards.Location = new Point(lbl_Contacted.Right + 20, 7);
            lbl_Sponsors.Location = new Point(lbl_Rewards.Right + 20, 7);
        }

        private void DisplayData()
        {
            // Businesses
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            DataTable dta = new DataTable();
            adaptSponsorship = new SqlDataAdapter("select * from tbl_BusinessList", sqlConnection);
            adaptSponsorship.Fill(dta);
            dataGridView1.DataSource = dta;
            sqlConnection.Close();

            // Churces
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            DataTable dtb = new DataTable();
            adaptSponsorship = new SqlDataAdapter("select * from tbl_ChurchList", sqlConnection);
            adaptSponsorship.Fill(dtb);
            dataGridView2.DataSource = dtb;
            sqlConnection.Close();

            BeginInvoke(new MethodInvoker(FinalizeLoad));
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
