using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data;


namespace WpfApp1
{
    /// <summary>
    /// Kullanıcılar.xaml etkileşim mantığı
    /// </summary>
    public partial class Kullanıcılar : Window
    {
        public string HashMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public void goster() {
            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Kullanicilar", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;
        }

        public void logGoster()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT TOP 100 *FROM KullaniciGiris ORDER BY HareketID DESC", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_2.ItemsSource = dt.DefaultView;
        }
        public Kullanıcılar()
        {
            baglanti.Open();
            InitializeComponent();
            goster();
            logGoster();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");

        private void KullanıcıEkle_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(K_Adi_TxtBox.Text) || string.IsNullOrEmpty(Sifre_TxtBox.Text) || string.IsNullOrEmpty(Rol_TxtBox.Text))
                {
                    MessageBox.Show("Lütfen eklenecek kullanıcının bilgilerini eksiksiz giriniz !");
                }
                else if(Rol_TxtBox.Text!="Garson" && Rol_TxtBox.Text!="Yonetici")
                {
                    MessageBox.Show("Hatalı rol girdiniz!");
                }
                else
                {
                    string k_Adi = K_Adi_TxtBox.Text;
                    string sifre = Sifre_TxtBox.Text;
                    string rol = Rol_TxtBox.Text;
                    string sifremd5 = HashMD5(sifre);
                    SqlCommand kullaniciKaydet = new SqlCommand("INSERT INTO Kullanicilar (KullaniciAdi,Sifre,Rol) VALUES (@k_Adi,@sifre,@rol)", baglanti);
                    kullaniciKaydet.Parameters.AddWithValue("@k_Adi", k_Adi);
                    kullaniciKaydet.Parameters.AddWithValue("@sifre", sifremd5);
                    kullaniciKaydet.Parameters.AddWithValue("@rol", rol);
                    kullaniciKaydet.ExecuteNonQuery();
                    goster();
                }
            }catch(OverflowException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");

            }catch(FormatException ex)
            {
                MessageBox.Show("Hatalı değer girdiniz!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun");
            }
        }

        private void KullanıcıSil_Button_Click(object sender,RoutedEventArgs e)
        {

            if (dg_1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen önce bir kullanıcı seçiniz.");
                return;
            }
            else
            {
                DataRowView secilenkullanici = (DataRowView)dg_1.SelectedItem;
                object secilenkullaniciob = secilenkullanici["KullaniciID"];
                int secilenkullaniciID = Convert.ToInt32(secilenkullaniciob);
                SqlCommand kullanicisil = new SqlCommand("DELETE FROM Kullanicilar WHERE KullaniciID=@k_ID", baglanti);
                kullanicisil.Parameters.AddWithValue("@k_ID", secilenkullaniciID);
                kullanicisil.ExecuteNonQuery();
                goster();
            }


        }
    }
}
