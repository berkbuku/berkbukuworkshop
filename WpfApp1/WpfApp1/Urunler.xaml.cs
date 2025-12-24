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
    /// Urunler.xaml etkileşim mantığı
    /// </summary>
    public partial class Urunler : Window
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        public void goster()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Urunler",baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Urunlerdg.ItemsSource = dt.DefaultView;
        }
        public Urunler()
        {
            
            InitializeComponent();
            baglanti.Open();
            goster();
            
        }
        private void Masalar_Grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Fiyat") // kolon adı veritabanından gelen property adıyla aynı olmalı
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

            private void UrunEkle_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal girilenfiyat;
                if (string.IsNullOrEmpty(UrunAdi_TxtBox.Text) || string.IsNullOrEmpty(Fiyat_TxtBox.Text))
                {
                    MessageBox.Show("Lütfen ürün adı ve fiyatı giriniz!");
                }
                else
                {


                    girilenfiyat = Convert.ToDecimal(Fiyat_TxtBox.Text);
                    SqlCommand urunEkle = new SqlCommand("INSERT INTO Urunler (UrunAdi,Fiyat) VALUES (@urunadi,@fiyat)", baglanti);
                    urunEkle.Parameters.AddWithValue("@urunadi", UrunAdi_TxtBox.Text);
                    urunEkle.Parameters.AddWithValue("@fiyat", girilenfiyat);
                    urunEkle.ExecuteNonQuery();
                    goster();
                }
            }catch(OverflowException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }catch(FormatException ex)
            {
                MessageBox.Show("Hatalı değer girdiniz!");
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }
            
        }

        private void UrunSil_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Urunlerdg.SelectedItem != null)
            {
                DataRowView secilenUrun = (DataRowView)Urunlerdg.SelectedItem;
                object secilenurunidob = secilenUrun["UrunID"];
                int secilenurunid = Convert.ToInt32(secilenurunidob);
                SqlCommand urunSil = new SqlCommand("DELETE FROM Urunler WHERE UrunID=@urunid", baglanti);
                SqlCommand urunHareketSil = new SqlCommand("DELETE FROM UrunHareketleri WHERE UrunID = @urunid", baglanti);
                urunHareketSil.Parameters.AddWithValue("@urunid", secilenurunid);
                urunSil.Parameters.AddWithValue("@urunid", secilenurunid);
                urunHareketSil.ExecuteNonQuery();
                urunSil.ExecuteNonQuery();
                goster();
            }
            else
            {
                MessageBox.Show("Lütfen önce silinecek ürünü seçiniz!");
            }

        }
    }
}
