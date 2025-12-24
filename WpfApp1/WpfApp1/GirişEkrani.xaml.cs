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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class GirisEkrani : Window
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

        public void kullaniciLog(string k_adi)
        {
            SqlCommand kullaniciIDBulcomm = new SqlCommand("SELECT KullaniciID FROM Kullanicilar WHERE KullaniciAdi=@k_adi", baglanti);
            kullaniciIDBulcomm.Parameters.AddWithValue("@k_adi", k_adi);
            int kullaniciID = Convert.ToInt32(kullaniciIDBulcomm.ExecuteScalar());
            SqlCommand kullaniciRolBulcomm = new SqlCommand("SELECT Rol FROM Kullanicilar WHERE KullaniciAdi=@k_adi",baglanti);
            kullaniciRolBulcomm.Parameters.AddWithValue("@k_adi", k_adi);
            string kullanicirol = Convert.ToString(kullaniciRolBulcomm.ExecuteScalar());
            SqlCommand kullaniciLogcomm = new SqlCommand("INSERT INTO KullaniciGiris (KullaniciID,KullaniciAdi,Rol) VALUES (@kullaniciid,@kullaniciadi,@rol)", baglanti);
            kullaniciLogcomm.Parameters.AddWithValue("@kullaniciid", kullaniciID);
            kullaniciLogcomm.Parameters.AddWithValue("@kullaniciadi", k_adi);
            kullaniciLogcomm.Parameters.AddWithValue("@rol",kullanicirol);
            kullaniciLogcomm.ExecuteNonQuery();
                

        }


        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        public GirisEkrani()
        {
            InitializeComponent();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string k_Adi = K_Adi_TxtBox.Text.Trim();
            string sifre = Sifre_TxtBoxpass.Password.Trim();
            string sifremd5 = HashMD5(sifre);
            baglanti.Open();
            string query = "SELECT Rol FROM Kullanicilar  WHERE KullaniciAdi=@k_Adi AND Sifre = @sifre";
            SqlCommand komut = new SqlCommand(query,baglanti);
            komut.Parameters.AddWithValue("@k_Adi", k_Adi);
            komut.Parameters.AddWithValue("@sifre", sifremd5);
            object rolObj = komut.ExecuteScalar();
            if (rolObj != null)
            {
                string rol = rolObj.ToString();

                if (rol == "Yonetici") { 
                
                    MessageBox.Show("Yönetici Girişi Başarılı");
                    KullaniciPaneli yonetici = new KullaniciPaneli();
                    yonetici.Show();
                    kullaniciLog(k_Adi);
                    this.Close();
                }

                else 
                {
                    MessageBox.Show("Garson Girişi Başarılı");
                    KullaniciPaneli garson = new KullaniciPaneli();
                    garson.Show();
                    garson.Kullanıcılar_Button.Visibility = Visibility.Hidden;
                    garson.MasaOlustur_Button.Visibility = Visibility.Hidden;
                    garson.Urunler_Button.Visibility = Visibility.Hidden;
                    garson.IrsaliyeGir_Button.Visibility = Visibility.Hidden;
                    garson.StokDuzenle_Button.Visibility = Visibility.Hidden;
                    garson.Raporlar_Button.Visibility = Visibility.Hidden;
                    kullaniciLog(k_Adi);
                    this.Close();

                }
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
            }
            Application.Current.Properties["k_Adi"] = k_Adi;

            
        baglanti.Close();
        }

       
    }
}
