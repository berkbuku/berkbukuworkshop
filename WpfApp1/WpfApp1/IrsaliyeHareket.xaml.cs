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
    /// IrsaliyeHareket.xaml etkileşim mantığı
    /// </summary>
    public partial class IrsaliyeHareket : Window
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");

        public void IrsaliyeGoster()
        {
            SqlCommand IrsaliyeGostermeComm = new SqlCommand("SELECT *FROM IrsaliyeHareket",baglanti);
            SqlDataAdapter IrsaliyeGosterDA = new SqlDataAdapter(IrsaliyeGostermeComm);
            DataTable IrsaliyeDT = new DataTable();
            IrsaliyeGosterDA.Fill(IrsaliyeDT);
            Irsaliye_dg.ItemsSource = IrsaliyeDT.DefaultView;
        }
        public void UrunStokGoster()
        {
            SqlCommand UrunStokGostermeComm = new SqlCommand("SELECT *FROM UrunHareketleri", baglanti);
            SqlDataAdapter UrunStokGosterDA = new SqlDataAdapter(UrunStokGostermeComm);
            DataTable UrunStokDT = new DataTable();
            UrunStokGosterDA.Fill(UrunStokDT);
            UrunHareketleri_dg.ItemsSource = UrunStokDT.DefaultView;
        }
        private void Irsaliye_dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "BirimFiyat") // kolon adı veritabanından gelen property adıyla aynı olmalı
            {
                if (e.Column is DataGridTextColumn textColumn)
                {
                    // Decimal değeri sadece tam sayı olarak göstermek için:
                    (textColumn.Binding as Binding).StringFormat = "C";

                    // Alternatifler:
                    // "N2" → 2 basamak ondalıklı: 30,00
                    // "C" → ₺30,00 (yerel ayara bağlı olarak TL simgeli para formatı)
                }
            }
        }
        public IrsaliyeHareket()
        {
            InitializeComponent();
            baglanti.Open();
            IrsaliyeGoster();
            UrunStokGoster();


        }

        private void StokGiris_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(UrunID_TxtBox.Text) | string.IsNullOrEmpty(IrsaliyeNo_TxtBox.Text) || string.IsNullOrEmpty(CariAdi_TxtBox.Text) || string.IsNullOrEmpty(CariKodu_TxtBox.Text) || string.IsNullOrEmpty(Miktar_TxtBox.Text) || string.IsNullOrEmpty(BirimFiyat_TxtBox.Text))
                {
                    MessageBox.Show("Boş alan bırakamazsınız!");
                }
                else
                {
                    int urunid = Convert.ToInt32(UrunID_TxtBox.Text);
                    string irsaliyeno = IrsaliyeNo_TxtBox.Text;
                    int miktar = Convert.ToInt32(Miktar_TxtBox.Text);
                    string carikodu = CariKodu_TxtBox.Text;
                    string cariadi = CariAdi_TxtBox.Text;
                    decimal birimfiyat = Convert.ToDecimal(BirimFiyat_TxtBox.Text);
                    string teslimalankullanici = Application.Current.Properties["k_Adi"] as string;
                    SqlCommand irsaliyeKaydetcomm = new SqlCommand("INSERT INTO IrsaliyeHareket (IrsaliyeNO,UrunID,TeslimAlanKullanici,Miktar,CariKodu,CariAdi,BirimFiyat) VALUES (@irsaliyeno,@urunid,@kullanici,@miktar,@carikodu,@cariadi,@birimfiyat)", baglanti);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@irsaliyeno", irsaliyeno);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@urunid", urunid);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@miktar", miktar);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@kullanici", teslimalankullanici);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@carikodu", carikodu);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@cariadi", cariadi);
                    irsaliyeKaydetcomm.Parameters.AddWithValue("@birimfiyat", birimfiyat);
                    irsaliyeKaydetcomm.ExecuteNonQuery();
                    IrsaliyeGoster();
                    UrunStokGoster();
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Hatalı değer girdiniz!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }
            catch(OverflowException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }
        }
        }
    }


