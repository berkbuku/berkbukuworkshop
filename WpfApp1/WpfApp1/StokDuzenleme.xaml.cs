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
using System.Data;

namespace WpfApp1
{
    /// <summary>
    /// StokDuzenleme.xaml etkileşim mantığı
    /// </summary>
    public partial class StokDuzenleme : Window
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        public int urunid;
        public string urunadi;
        public int oncekimiktar;
        public void UrunStokGoster()
        {
            SqlCommand UrunStokGostermeComm = new SqlCommand("SELECT *FROM UrunHareketleri", baglanti);
            SqlDataAdapter UrunStokGosterDA = new SqlDataAdapter(UrunStokGostermeComm);
            DataTable UrunStokDT = new DataTable();
            UrunStokGosterDA.Fill(UrunStokDT);
            UrunHareketleri_dg.ItemsSource = UrunStokDT.DefaultView;
        }

        public void StokHareketleriGoster()
        {
            SqlCommand stokhareketleriGostercomm = new SqlCommand("SELECT TOP 50 *FROM StokHareketleri", baglanti);
            SqlDataAdapter StokHareketleriDA = new SqlDataAdapter(stokhareketleriGostercomm);
            DataTable dt = new DataTable();
            StokHareketleriDA.Fill(dt);
            StokHareketleri_dg.ItemsSource = dt.DefaultView;
        }
        public StokDuzenleme()
        {
            InitializeComponent();
            baglanti.Open();
            UrunStokGoster();
            StokHareketleriGoster();
        }

        private void UrunHareketleri_dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object secilenurunidob;
            object secilenurunmiktarob;
            object secilenurunadiob;
            if (UrunHareketleri_dg.SelectedItem != null)
            {

                DataRowView secilenurun = (DataRowView)UrunHareketleri_dg.SelectedItem;
                secilenurunidob = secilenurun["UrunID"];
                secilenurunmiktarob = secilenurun["Stok"];
                secilenurunadiob = secilenurun["UrunAdi"];
                string secilenurunadi = Convert.ToString(secilenurunadiob);
                int secilenurunmiktar = Convert.ToInt32(secilenurunmiktarob);
                int secilenurunid = Convert.ToInt32(secilenurunidob);
                miktardegistir_se.Value = secilenurunmiktar;
                urunid = secilenurunid;
                oncekimiktar = secilenurunmiktar;
                urunadi = secilenurunadi;

            }
            else
            {
               
                
            }

            
             
            
           
        }
        
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UrunHareketleri_dg.SelectedItem == null || string.IsNullOrEmpty(SebepTextBox.Text))
                {
                    MessageBox.Show("Lütfen miktar ve stok güncelleme sebebi giriniz!");
                }
                else
                {
                    int miktar = Convert.ToInt32(miktardegistir_se.Value);
                    SqlCommand stokguncelle = new SqlCommand("INSERT INTO StokHareketleri (UrunID,UrunAdi,OncekiStok,GuncelStok,GuncelleyenKullanici,GuncellemeSebebi) VALUES (@urunid,@urunadi,@oncekistok,@guncelstok,@guncelleyenkullanici,@sebep)", baglanti);
                    stokguncelle.Parameters.AddWithValue("@urunid", urunid);
                    stokguncelle.Parameters.AddWithValue("@urunadi", urunadi);
                    stokguncelle.Parameters.AddWithValue("@oncekistok", oncekimiktar);
                    stokguncelle.Parameters.AddWithValue("@guncelstok", miktar);
                    string degistirenkullanici = Application.Current.Properties["k_Adi"] as string;
                    stokguncelle.Parameters.AddWithValue("@guncelleyenkullanici", degistirenkullanici);
                    string sebep = SebepTextBox.Text;
                    stokguncelle.Parameters.AddWithValue("@sebep", sebep);
                    stokguncelle.ExecuteNonQuery();
                    UrunStokGoster();
                    StokHareketleriGoster();
                }
            }catch(OverflowException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }catch(FormatException ex)
            {
                MessageBox.Show("Hatalı değer girdiniz!");
            }catch(SqlException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }
        }
    }
}
