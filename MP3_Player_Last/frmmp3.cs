using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace MP3_Player_Last
{
    public partial class frmmp3 : DevExpress.XtraEditors.XtraForm
    {
        public frmmp3()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=TALHA-PC\SQLEXPRESS;Initial Catalog=PlayList;Integrated Security=True");

        Random rnd = new Random();

        bool playing;

        void listele()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select ad from tbl_muzik order by ad", baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                listBox1.Items.Add(dr[0]).ToString();
            }
            baglanti.Close();
        }

        void resimgetir()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select picture from tbl_resim where durum=1", baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                pictureBox1.Visible = true;
                pictureBox1.ImageLocation = dr[0].ToString();
            }
            baglanti.Close();
        }

        void favorimusicgoster()
        {
            textBox1.Clear();
            label6.Visible = false;
            listBox1.Items.Clear();
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from tbl_muzik where favori=1 order by ad", baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                listBox1.Items.Add(dr[0]).ToString();
            }
            baglanti.Close();
        }

        void play()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select yol from tbl_muzik where ad=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", Convert.ToString(listBox1.SelectedItem));
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                axWindowsMediaPlayer1.URL = dr[0].ToString();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            baglanti.Close();
            playing = true;
            btnplay.Visible = false;
            btnpause.Visible = true;
        }

        void deletesong()
        {
            baglanti.Open();

            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No music found.", "Warning - No music found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Select a song to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (MessageBox.Show("Are you sure you want to delete selected song?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("delete from tbl_muzik where ad=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", Convert.ToString(listBox1.SelectedItem));
                komut.ExecuteNonQuery();
                baglanti.Close();
            }
            baglanti.Close();
        }

        void readmusic()
        {
            baglanti.Close();
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from tbl_muzik where ad=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", label2.Text);
            SqlDataReader dr = komut.ExecuteReader();
            if (dr.Read() == false)
            {
                btnplay.Visible = true;
                btnpause.Visible = false;
                label2.Text = "";
                label3.Text = "";
                axWindowsMediaPlayer1.close();
            }
            baglanti.Close();
        }

        string sarkiad;

        string sarkiyol;

        void addsong()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Filter = "Music Files(*.mp3)|*.mp3";
            if (open.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i <= open.SafeFileNames.Length - 1; i++)
                {
                    sarkiad = Convert.ToString(open.SafeFileNames[i]);
                    sarkiyol = Convert.ToString(open.FileNames[i]);

                    if (listBox1.Items.Contains(sarkiad) == true)
                    {
                        baglanti.Open();
                        SqlCommand komut = new SqlCommand("update tbl_muzik set yol=@p1 where ad=@p2", baglanti);
                        komut.Parameters.AddWithValue("@p1", sarkiyol);
                        komut.Parameters.AddWithValue("@p2", sarkiad);
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                    }
                    else
                    {
                        baglanti.Open();
                        SqlCommand komut = new SqlCommand("insert into tbl_muzik (ad,yol) values (@p1,@p2)", baglanti);
                        komut.Parameters.AddWithValue("@p1", sarkiad);
                        komut.Parameters.AddWithValue("@p2", sarkiyol);
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                    }
                }
            }
            textBox1.Clear();
            listBox1.Items.Clear();
            listele();
        }

        private void frmmp3_Load(object sender, EventArgs e)
        {
            Thread.Sleep(6000);
            baglanti.Open();
            SqlCommand komut2 = new SqlCommand("select * from tbl_resim", baglanti);
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                this.BackColor = Color.FromArgb(Convert.ToInt16(dr2[3]), Convert.ToInt16(dr2[4]), Convert.ToInt16(dr2[5]));
            }
            baglanti.Close();

            listele();
            resimgetir();
            panel1.Size = new Size(286, 33);
            panel1.Location = new Point(0, 195);
            label3.Location = new Point(-300, 6);
            pictureBox1.Size = new Size(286, 162);
            pictureBox1.Location = new Point(0, 23);
            pictureBox2.Size = new Size(44, 32);
            buttonEdit1.Value = 100;
            axWindowsMediaPlayer1.settings.volume = 100;
            pictureBox3.Size = new Size(44, 32);
            pictureBox4.Size = new Size(44, 32);
            pictureBox5.Size = new Size(44, 32);
            pictureBox5.Visible = true;
            this.KeyPreview = true;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null && axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused && label2.Text == Convert.ToString(listBox1.SelectedItem))
            {
                playing = true;
                btnplay.Visible = false;
                btnpause.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else if (checkBox1.Checked)
            {
                listBox2.Items.Remove(listBox1.SelectedItems[0]);
                playing = true;
                btnplay.Visible = false;
                btnpause.Visible = true;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else if (listBox1.SelectedItem != null)
            {
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                if (textBox1.Text != "" && listBox1.Items.Count == 0)
                {
                    axWindowsMediaPlayer1.close();
                    btnplay.Visible = true;
                    btnpause.Visible = false;
                }
                else if (checkBox4.Checked && listBox1.Items.Count == 0)
                {
                    axWindowsMediaPlayer1.close();
                    btnplay.Visible = true;
                    btnpause.Visible = false;
                }
                else if (checkBox1.Checked)
                {
                    BeginInvoke(new Action(() =>
                    {
                        if (listBox2.Items.Count == 0)
                        {
                            for (int i = 0; i < listBox1.Items.Count; i++)
                            {
                                listBox2.Items.Add(listBox1.Items[i].ToString());
                            }
                        }
                        int sayi2 = rnd.Next(0, listBox2.Items.Count);
                        listBox2.SelectedIndex = sayi2;
                        listBox1.SelectedItem = listBox2.SelectedItem;
                        play();
                        while (listBox2.SelectedItems.Count > 0)
                        {
                            listBox2.Items.Remove(listBox2.SelectedItems[0]);
                        }
                        label2.Text = Convert.ToString(listBox1.SelectedItem);
                        label3.Text = label2.Text;
                        label3.Location = new Point(-300, 3);
                    }));
                }
                else if (checkBox2.Checked)
                {
                    BeginInvoke(new Action(() =>
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex;
                        label2.Text = Convert.ToString(listBox1.SelectedItem);
                        label3.Text = label2.Text;
                        label3.Location = new Point(-300, 3);
                        play();
                    }));
                }
                else if (listBox1.SelectedIndex != listBox1.Items.Count - 1)
                {
                    BeginInvoke(new Action(() =>
                    {
                        listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                        label2.Text = Convert.ToString(listBox1.SelectedItem);
                        label3.Text = label2.Text;
                        label3.Location = new Point(-300, 3);
                        play();
                    }));
                }
                else if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                {
                    BeginInvoke(new Action(() =>
                    {
                        listBox1.SelectedIndex = 0;
                        label2.Text = Convert.ToString(listBox1.SelectedItem);
                        label3.Text = label2.Text;
                        label3.Location = new Point(-300, 3);
                        play();
                    }));
                }
            }
        }

        private void btnpause_Click(object sender, EventArgs e)
        {
            playing = false;
            btnplay.Visible = true;
            btnpause.Visible = false;
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            playing = false;
            btnpause.Visible = false;
            btnplay.Visible = true;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void btnplay_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                playing = true;
                btnplay.Visible = false;
                btnpause.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else if (label6.Visible == true)
            {
                MessageBox.Show("A song by that name could not be found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (checkBox4.Checked && listBox1.Items.Count == 0)
            {
                MessageBox.Show("No favorite musics found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (listBox1.Items.Count == 0 && MessageBox.Show("Add song(s) to play.", "Warning - No music found", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                addsong();
            }
            else if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Select a song to play.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No music found on the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (checkBox1.Checked)
            {
                if (listBox2.Items.Count == 0)
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        listBox2.Items.Add(listBox1.Items[i].ToString());
                    }
                }
                int sayi2 = rnd.Next(0, listBox2.Items.Count);
                listBox2.SelectedIndex = sayi2;
                listBox1.SelectedItem = listBox2.SelectedItem;
                play();
                while (listBox2.SelectedItems.Count > 0)
                {
                    listBox2.Items.Remove(listBox2.SelectedItems[0]);
                }
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
            }
            else if (checkBox2.Checked)
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
            {
                listBox1.SelectedIndex = 0;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
        }

        private void btnprevious_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No music found on the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (listBox1.SelectedItem == null)
            {
                listBox1.SelectedIndex = 0;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else if (checkBox1.Checked)
            {
                if (listBox2.Items.Count == 0)
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        listBox2.Items.Add(listBox1.Items[i].ToString());
                    }
                }
                int sayi2 = rnd.Next(0, listBox2.Items.Count);
                listBox2.SelectedIndex = sayi2;
                listBox1.SelectedItem = listBox2.SelectedItem;
                play();
                while (listBox2.SelectedItems.Count > 0)
                {
                    listBox2.Items.Remove(listBox2.SelectedItems[0]);
                }
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
            }
            else if (checkBox2.Checked)
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else if (listBox1.SelectedIndex == 0)
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
            else
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                label2.Text = Convert.ToString(listBox1.SelectedItem);
                label3.Text = label2.Text;
                label3.Location = new Point(-300, 3);
                play();
            }
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked == false)
            {
                addsong();
                listBox1.SelectedItem = label2.Text;
            }
            else
            {
                MessageBox.Show("You can not add song(s) while viewing your favorite song(s).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                textBox1.Enabled = true;
                listBox1.Enabled = true;
                label1.Visible = false;
                label6.Visible = false;
                readmusic();
            }
            else if (textBox1.Text != "" && listBox1.Items.Count == 0)
            {
                readmusic();
                label6.Visible = true;
            }
            else if (checkBox4.Checked && listBox1.Items.Count == 0)
            {
                listBox1.Enabled = false;
                label1.Visible = true;
            }
            else if (textBox1.Text == "" && listBox1.Items.Count == 0)
            {
                axWindowsMediaPlayer1.close();
                btnplay.Visible = true;
                btnpause.Visible = false;
                listBox1.Enabled = false;
                label1.Visible = true;
                label3.Text = "";
            }
            else if (listBox1.Items.Count == 0 && axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                listBox1.SelectedItem = label3.Text;
                listBox1.Enabled = false;
                label1.Show();
            }
            else if (checkBox4.Checked && listBox1.Items.Count == 0 && axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                label2.Text = "hi";
                label2.Visible = true;
            }
            else if (listBox1.Items.Count == 0)
            {
                axWindowsMediaPlayer1.close();
                btnplay.Visible = true;
                btnpause.Visible = false;
                listBox1.Enabled = false;
                label1.Visible = true;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (label3.Text != "")
            {
                if (label3.Left < 300)
                {
                    label3.Left = label3.Left + 1;
                }
                else
                {
                    label3.Left = -label3.Width;
                }
            }
            else
            {
                label3.Location = new Point(-300, 3);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listBox2.Items.Add(listBox1.Items[i].ToString());
                }
                checkBox2.Enabled = false;
            }
            else
            {
                listBox2.Items.Clear();
                checkBox2.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked == false)
            {
                if (string.IsNullOrEmpty(textBox1.Text.Trim()))
                {
                    deletesong();
                    listBox1.Items.Clear();
                    listele();
                    listBox1.SelectedItem = label2.Text;
                    textBox1.Clear();
                }
                else
                {
                    deletesong();
                    listBox1.Items.Clear();

                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("select ad from tbl_muzik where ad like '%" + textBox1.Text + "%' order by ad", baglanti);
                    SqlDataReader dr = komut.ExecuteReader();
                    while (dr.Read())
                    {
                        listBox1.Items.Add(dr[0]).ToString();
                    }
                    baglanti.Close();

                    listBox1.SelectedItem = label2.Text;
                }
            }
            else
            {
                MessageBox.Show("You can not delete song while viewing your favorite song(s).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (checkBox4.Checked)
            {
                MessageBox.Show("You can not delete song(s) while viewing your favorite song(s).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (textBox1.Text != "")
            {
                MessageBox.Show("Please clear the text before deleting all the song(s).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No music found.", "Warning - No music found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (MessageBox.Show("This process will delete ALL songs. Do you accept?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("delete from tbl_muzik", baglanti);
                komut.ExecuteNonQuery();
                baglanti.Close();
                listBox1.Items.Clear();
                listele();
                label2.Text = "";
                label3.Text = "";
                textBox1.Clear();
                textBox1.Enabled = false;
                label6.Visible = false;
                label1.Visible = true;
                label3.Location = new Point(-300, 3);
                axWindowsMediaPlayer1.close();
                MessageBox.Show("Song(s) successfully deleted.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true && checkBox4.Checked == true)
            {
                listBox2.Items.Clear();
                listBox1.Items.Clear();
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select ad from tbl_muzik where ad like '%" + textBox1.Text + "%'" + " and favori=1 order by ad", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0]).ToString();
                    listBox2.Items.Add(dr[0]).ToString();
                }
                baglanti.Close();
                listBox1.SelectedItem = label3.Text;
            }
            else if (checkBox4.Checked)
            {
                listBox2.Items.Clear();
                listBox1.Items.Clear();
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select ad from tbl_muzik where ad like '%" + textBox1.Text + "%'" + " and favori=1 order by ad", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0]).ToString();
                    listBox2.Items.Add(dr[0]).ToString();
                }
                baglanti.Close();
                listBox1.SelectedItem = label3.Text;
            }
            else if (checkBox1.Checked)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select ad from tbl_muzik where ad like '%" + textBox1.Text + "%' order by ad", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0]).ToString();
                    listBox2.Items.Add(dr[0]).ToString();
                }
                baglanti.Close();
                listBox1.SelectedItem = label3.Text;
            }
            else
            {
                listBox1.Items.Clear();
                baglanti.Open();
                SqlCommand komut = new SqlCommand("select ad from tbl_muzik where ad like '%" + textBox1.Text + "%' order by ad", baglanti);
                SqlDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0]).ToString();
                }
                baglanti.Close();
                listBox1.SelectedItem = label3.Text;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox1.Checked = false;
                favorimusicgoster();
                listBox1.SelectedItem = label2.Text;
            }
            else
            {
                textBox1.Clear();
                listBox1.Items.Clear();
                listele();
                listBox1.SelectedItem = label2.Text;
                checkBox1.Checked = false;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && listBox1.Items.Count == 0)
            {
                textBox1.Enabled = false;
            }
        }

        private void buttonEdit1_EditValueChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = buttonEdit1.Value;
            switch (buttonEdit1.Value)
            {
                case 10: axWindowsMediaPlayer1.settings.volume = 100; break;
                case 9: axWindowsMediaPlayer1.settings.volume = 90; break;
                case 8: axWindowsMediaPlayer1.settings.volume = 80; break;
                case 7: axWindowsMediaPlayer1.settings.volume = 70; break;
                case 6: axWindowsMediaPlayer1.settings.volume = 60; break;
                case 5: axWindowsMediaPlayer1.settings.volume = 50; break;
                case 4: axWindowsMediaPlayer1.settings.volume = 40; break;
                case 3: axWindowsMediaPlayer1.settings.volume = 30; break;
                case 2: axWindowsMediaPlayer1.settings.volume = 20; break;
                case 1: axWindowsMediaPlayer1.settings.volume = 10; break;
                case 0: axWindowsMediaPlayer1.settings.volume = 0; break;
            }
            if (axWindowsMediaPlayer1.settings.volume >= 50)
            {
                pictureBox5.Visible = true;
                pictureBox4.Visible = false;
                pictureBox3.Visible = false;
                pictureBox2.Visible = false;
            }
            else if (axWindowsMediaPlayer1.settings.volume <= 50 && axWindowsMediaPlayer1.settings.volume != 0)
            {
                pictureBox5.Visible = false;
                pictureBox4.Visible = true;
                pictureBox3.Visible = false;
                pictureBox2.Visible = false;
            }
            else if (axWindowsMediaPlayer1.settings.volume == 0)
            {
                pictureBox5.Visible = false;
                pictureBox4.Visible = false;
                pictureBox3.Visible = true;
                pictureBox2.Visible = false;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            buttonEdit1.Value = 0;
            pictureBox5.Visible = false;
            pictureBox4.Visible = false;
            pictureBox3.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            buttonEdit1.Value = 0;
            pictureBox5.Visible = false;
            pictureBox4.Visible = false;
            pictureBox3.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            buttonEdit1.Value = 0;
            pictureBox5.Visible = false;
            pictureBox4.Visible = false;
            pictureBox3.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            buttonEdit1.Value = 5;
            pictureBox5.Visible = true;
            pictureBox4.Visible = false;
            pictureBox3.Visible = false;
            pictureBox2.Visible = false;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a song to add favorites.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("update tbl_muzik set favori=1 where ad=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", Convert.ToString(listBox1.SelectedItem));
                komut.ExecuteNonQuery();
                baglanti.Close();
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a song to remove from favorites.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (checkBox4.Checked)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("update tbl_muzik set favori=0 where ad=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", Convert.ToString(listBox1.SelectedItem));
                komut.ExecuteNonQuery();
                baglanti.Close();

                favorimusicgoster();
                listBox1.SelectedItem = label2.Text;
            }
            else
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("update tbl_muzik set favori=0 where ad=@p1", baglanti);
                komut.Parameters.AddWithValue("@p1", Convert.ToString(listBox1.SelectedItem));
                komut.ExecuteNonQuery();
                baglanti.Close();
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (label2.Text == "")
            {
                MessageBox.Show("No songs are currently playing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                listBox1.ClearSelected();
                listBox1.SelectedItem = label2.Text;
            }
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "235");
            komut.Parameters.AddWithValue("@p2", "236");
            komut.Parameters.AddWithValue("@p3", "239");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(235, 236, 239);
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "230");
            komut.Parameters.AddWithValue("@p2", "59");
            komut.Parameters.AddWithValue("@p3", "59");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(230, 59, 59);
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "255");
            komut.Parameters.AddWithValue("@p2", "254");
            komut.Parameters.AddWithValue("@p3", "225");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(255, 254, 225);
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "144");
            komut.Parameters.AddWithValue("@p2", "238");
            komut.Parameters.AddWithValue("@p3", "144");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(144, 238, 144);
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "148");
            komut.Parameters.AddWithValue("@p2", "112");
            komut.Parameters.AddWithValue("@p3", "220");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(148, 112, 220);
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "255");
            komut.Parameters.AddWithValue("@p2", "232");
            komut.Parameters.AddWithValue("@p3", "232");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(255, 232, 232);
        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "255");
            komut.Parameters.AddWithValue("@p2", "165");
            komut.Parameters.AddWithValue("@p3", "0");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(255, 165, 0);
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "210");
            komut.Parameters.AddWithValue("@p2", "135");
            komut.Parameters.AddWithValue("@p3", "49");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(210, 135, 49);
        }

        private void barButtonItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "211");
            komut.Parameters.AddWithValue("@p2", "211");
            komut.Parameters.AddWithValue("@p3", "211");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Filter = "Image Files(*.jpg;*.jpeg;*.PNG;*.GIF;*.BMP)|*.jpg;*.jpeg;*.png;*.GIF;*.BMP";
            if (open.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i <= open.SafeFileNames.Length - 1; i++)
                {
                    baglanti.Open();
                    SqlCommand komut2 = new SqlCommand("update tbl_resim set picture=@p1", baglanti);
                    komut2.Parameters.AddWithValue("@p1", Convert.ToString(open.FileNames[i]));
                    komut2.ExecuteNonQuery();
                    baglanti.Close();

                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("update tbl_resim set durum=1", baglanti);
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    resimgetir();
                }
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("Copyright © Black Knights 2018 ~ All Rights Reserved" + "\n" + "To give feedback, to report bugs, or to contact us: Black_Knights@zero.com", "About Us", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set durum=0", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();
            pictureBox1.Hide();
        }

        private void barButtonItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set r=@p1,g=@p2,b=@p3", baglanti);
            komut.Parameters.AddWithValue("@p1", "192");
            komut.Parameters.AddWithValue("@p2", "255");
            komut.Parameters.AddWithValue("@p3", "255");
            komut.ExecuteNonQuery();
            baglanti.Close();
            this.BackColor = Color.FromArgb(192, 255, 255);
        }

        private void frmmp3_Shown(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut0 = new SqlCommand("select * from tbl_resim where programgiris='True'", baglanti);
            SqlDataReader dr = komut0.ExecuteReader();
            if (dr.Read())
            {
                MessageBox.Show("Hello! We are Black Knights!" + "\n" + "Thanks for downloading the Mp3 Player." + "\n" + " If you have any problems, use the help button above." + "\n" + "To give feedback, to report bugs, or to contact us: Black_Knights@zero.com" + "\n" + "Copyright © All Rights reserved", "Thanks For Downloading!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            baglanti.Close();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("update tbl_resim set programgiris='False'", baglanti);
            komut.ExecuteNonQuery();
            baglanti.Close();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copyright © Black Knights 2018 ~ All Rights Reserved" + "\n" + "To give feedback, to report bugs, or to contact us: Black_Knights@zero.com", "About Us", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("You can play the songs by double clicking on them. If you cant play the song you selected, you have probably changed name or location of the song. Try re-adding the song you want to play.\n\nYou can add songs to your favorite list. To do this, select a song, right click and click on ''Add to Favorites''. You can also remove them from your favorite list.\n\nYou can change backround color. Right click on the form and select a color.\n\nIf you cant view your picture, you have probably changed location or name of your picture. Try to re-add the picture. To set picture into default,  right click and select ''Show Default Image''.\n\nYou can add as many song as you want.\n\n''Delete Song'' button. Select a song to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnstop_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.MediaStop)
            {
                btnpause.Visible = false;
                btnplay.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
        }

        private void frmmp3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.MediaPlayPause)
            {
                if (playing == false)
                {
                    if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
                    {
                        playing = true;
                        btnplay.Visible = false;
                        btnpause.Visible = true;
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    }
                    else if (label6.Visible == true)
                    {
                        MessageBox.Show("A song by that name could not be found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (checkBox4.Checked && listBox1.Items.Count == 0)
                    {
                        MessageBox.Show("No favorite musics found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (listBox1.Items.Count == 0 && MessageBox.Show("Add song(s) to play.", "Warning - No music found", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        addsong();
                    }
                    else if (listBox1.SelectedItem == null)
                    {
                        MessageBox.Show("Select a song to play.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        label2.Text = Convert.ToString(listBox1.SelectedItem);
                        label3.Text = label2.Text;
                        label3.Location = new Point(-300, 3);
                        play();
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    }
                }
                else
                {
                    playing = false;
                    btnplay.Visible = true;
                    btnpause.Visible = false;
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                }
            }
            if (e.KeyCode == Keys.MediaStop)
            {
                playing = false;
                btnpause.Visible = false;
                btnplay.Visible = true;
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
            if (e.KeyCode == Keys.MediaNextTrack)
            {
                if (listBox1.Items.Count == 0)
                {
                    MessageBox.Show("No music found on the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (checkBox1.Checked)
                {
                    if (listBox2.Items.Count == 0)
                    {
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            listBox2.Items.Add(listBox1.Items[i].ToString());
                        }
                    }
                    int sayi2 = rnd.Next(0, listBox2.Items.Count);
                    listBox2.SelectedIndex = sayi2;
                    listBox1.SelectedItem = listBox2.SelectedItem;
                    play();
                    while (listBox2.SelectedItems.Count > 0)
                    {
                        listBox2.Items.Remove(listBox2.SelectedItems[0]);
                    }
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                }
                else if (checkBox2.Checked)
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
                else if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                {
                    listBox1.SelectedIndex = 0;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
                else
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
            }
            if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                if (listBox1.Items.Count == 0)
                {
                    MessageBox.Show("No music found on the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (listBox1.SelectedItem == null)
                {
                    listBox1.SelectedIndex = 0;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
                else if (checkBox1.Checked)
                {
                    if (listBox2.Items.Count == 0)
                    {
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            listBox2.Items.Add(listBox1.Items[i].ToString());
                        }
                    }
                    int sayi2 = rnd.Next(0, listBox2.Items.Count);
                    listBox2.SelectedIndex = sayi2;
                    listBox1.SelectedItem = listBox2.SelectedItem;
                    play();
                    while (listBox2.SelectedItems.Count > 0)
                    {
                        listBox2.Items.Remove(listBox2.SelectedItems[0]);
                    }
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                }
                else if (checkBox2.Checked)
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
                else if (listBox1.SelectedIndex == 0)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
                else
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                    label2.Text = Convert.ToString(listBox1.SelectedItem);
                    label3.Text = label2.Text;
                    label3.Location = new Point(-300, 3);
                    play();
                }
            }
        }

        private void asToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You can play the songs by double clicking on them. If you cant play the song you selected, you have probably changed name or location of the song. Try re-adding the song you want to play.\n\nYou can add songs to your favorite list. To do this, select a song, right click and click on ''Add to Favorites''. You can also remove them from your favorite list.\n\nYou can change backround color. Right click on the form and select a color.\n\nIf you cant view your picture, you have probably changed location or name of your picture. Try to re-add the picture. To set picture into default,  right click and select ''Show Default Image''.\n\nYou can add as many song as you want.\n\n''Delete Song'' button. Select a song to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
