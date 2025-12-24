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
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Globalization;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml etkileşim mantığı
    /// </summary>
    public partial class KullaniciPaneli : Window
    {
        public void masaComboBoxgoster()
        {
            SqlDataAdapter da3 = new SqlDataAdapter();
            da3.SelectCommand = new SqlCommand("SELECT MasaID FROM Masalar WHERE Durum='Bos'", baglanti);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            ComboBox_1.DisplayMemberPath = "MasaID";
            ComboBox_1.ItemsSource = dt3.DefaultView;
        }
        public KullaniciPaneli()
        {
            baglanti.Open();
            InitializeComponent();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SELECT *FROM Musteriler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;

            SqlDataAdapter da2 = new SqlDataAdapter();
            da2.SelectCommand = new SqlCommand("SELECT *FROM Masalar", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dg_2.ItemsSource = dt2.DefaultView;

            masaComboBoxgoster();

            SqlDataAdapter da4 = new SqlDataAdapter();
            da4.SelectCommand = new SqlCommand("SELECT MusteriID FROM Musteriler", baglanti);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);
            ComboBox_2.DisplayMemberPath = "MusteriID";
            ComboBox_2.ItemsSource = dt4.DefaultView;

            SqlDataAdapter da5 = new SqlDataAdapter();
            da5.SelectCommand = new SqlCommand("SELECT u.UrunID,u.UrunAdi FROM Urunler u WHERE u.UrunID IN(SELECT UrunID FROM UrunHareketleri GROUP BY UrunID HAVING SUM(Stok)>0)", baglanti);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);

            ComboBox_3.ItemsSource = dt5.DefaultView;
            ComboBox_3.DisplayMemberPath = "UrunAdi";
            ComboBox_3.SelectedValuePath = "UrunID";




        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        private void dg_3_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
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

        private void MusteriEkle_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_1.SelectedItem == null)
            {
                MessageBox.Show("Müşteri eklemek için bir masa seçiniz!");
            }
            else
            {
                int secilenMasaID = Convert.ToInt32(ComboBox_1.SelectedValue);
                SqlCommand musteriEkle = new SqlCommand("INSERT INTO Musteriler (OturduguMasaID) VALUES(@masaid)", baglanti);
                musteriEkle.Parameters.Add("@masaid", secilenMasaID);
                musteriEkle.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand("SELECT *FROM Musteriler", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dg_1.ItemsSource = dt.DefaultView;

                SqlDataAdapter da2 = new SqlDataAdapter();
                da2.SelectCommand = new SqlCommand("SELECT *FROM Masalar", baglanti);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                dg_2.ItemsSource = dt2.DefaultView;

                masaComboBoxgoster();

                SqlDataAdapter da4 = new SqlDataAdapter();
                da4.SelectCommand = new SqlCommand("SELECT MusteriID FROM Musteriler", baglanti);
                DataTable dt4 = new DataTable();
                da4.Fill(dt4);
                ComboBox_2.DisplayMemberPath = "MusteriID";
                ComboBox_2.ItemsSource = dt4.DefaultView;
            }
        }

        private void MusteriKapat_Button_Click(object sender, RoutedEventArgs e)
        {
            int secilenMusteriID = Convert.ToInt32(ComboBox_2.SelectedValue);
            SqlCommand musteriKapat = new SqlCommand("DELETE FROM Musteriler WHERE MusteriID=@musteriID", baglanti);
            musteriKapat.Parameters.Add("@musteriID", secilenMusteriID);
            musteriKapat.ExecuteNonQuery();

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SELECT *FROM Musteriler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;

            SqlDataAdapter da2 = new SqlDataAdapter();
            da2.SelectCommand = new SqlCommand("SELECT *FROM Masalar", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dg_2.ItemsSource = dt2.DefaultView;

            masaComboBoxgoster();

            SqlDataAdapter da4 = new SqlDataAdapter();
            da4.SelectCommand = new SqlCommand("SELECT MusteriID FROM Musteriler", baglanti);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);
            ComboBox_2.DisplayMemberPath = "MusteriID";
            ComboBox_2.ItemsSource = dt4.DefaultView;

        }

        private void dg_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int masaID;
            if (dg_2.SelectedItem == null)
            {
                
                masaID = -1;
            }
            else
            {
                DataRowView secilenmasa = (DataRowView)dg_2.SelectedItem;
                masaID = Convert.ToInt32(secilenmasa["MasaID"]);
            }
            
            SqlCommand adisyonid = new SqlCommand("SELECT TOP 1 AdisyonID FROM Adisyon WHERE MasaID = @masaID AND Durum = 'Acik' ORDER BY AdisyonID DESC",baglanti);
            adisyonid.Parameters.Add("@masaID", masaID);
            object adisyonIDob = adisyonid.ExecuteScalar();
            int adisyonID = Convert.ToInt32(adisyonIDob);
            SqlCommand adisyonHBul = new SqlCommand("SELECT ah.HareketID, u.UrunAdi, ah.Miktar, ah.Fiyat FROM AdisyonHareketleri ah INNER JOIN Urunler u ON ah.UrunID = u.UrunID WHERE ah.AdisyonID = @adisyonID",baglanti);
            adisyonHBul.Parameters.Add("@adisyonID", adisyonID);
            SqlDataAdapter da = new SqlDataAdapter(adisyonHBul);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_3.ItemsSource = dt.DefaultView;
            SqlCommand tutarhesapla = new SqlCommand("SELECT SUM(Fiyat) FROM AdisyonHareketleri WHERE AdisyonID=@adisyonid", baglanti);
            tutarhesapla.Parameters.AddWithValue("@adisyonid", adisyonID);
            decimal toplamtutar;
            string gosterilecekdeger;
            object tutar = tutarhesapla.ExecuteScalar();
            if (tutar != DBNull.Value)
            {
                toplamtutar = Convert.ToDecimal(tutar);
                gosterilecekdeger = toplamtutar.ToString("C");
                HesapTutari_Label.Content = "Toplam hesap tutarı :"+gosterilecekdeger;
                
            }
            



        }

        private void UrunEkle_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox_3.SelectedValue == null || UrunStok_ComboBox.SelectedValue==null)
            {
                MessageBox.Show("Lütfen ürün ve miktar giriniz.");
                return;
            }

            int urunID = Convert.ToInt32(ComboBox_3.SelectedValue);
            int miktar = Convert.ToInt32(UrunStok_ComboBox.SelectedValue);
            if (dg_2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen önce bir masa seçiniz.");
                return;
            }
            DataRowView secilenmasa = (DataRowView)dg_2.SelectedItem;
            object secilenmasaob = secilenmasa["MasaID"];
            int masaID = Convert.ToInt32(secilenmasaob);
            SqlCommand adisyonid = new SqlCommand("SELECT TOP 1 AdisyonID FROM Adisyon WHERE MasaID = @masaID AND Durum = 'Acik' ORDER BY AdisyonID DESC", baglanti);
            adisyonid.Parameters.AddWithValue("@masaID", masaID);

            object adisyonIDob = adisyonid.ExecuteScalar();
            int adisyonID = Convert.ToInt32(adisyonIDob);

            if (adisyonID == 0)
            {
                MessageBox.Show("Bu masa için açık bir adisyon bulunamadı.");
                return;
            }

            

            SqlCommand urunfiyati = new SqlCommand("SELECT Fiyat FROM Urunler WHERE UrunID=@urunID",baglanti);
            urunfiyati.Parameters.Add("@urunID", urunID);

            object fiyatob = urunfiyati.ExecuteScalar();
            decimal adetfiyatı = Convert.ToDecimal(fiyatob);
            decimal toplamfiyat = miktar * adetfiyatı;

            SqlCommand urunekle = new SqlCommand("INSERT INTO AdisyonHareketleri (AdisyonID,UrunID,Miktar,Fiyat) VALUES (@adisyonid,@urunid,@miktar,@fiyat)", baglanti);
            urunekle.Parameters.Add("@adisyonid", adisyonID);
            urunekle.Parameters.Add("@urunid", urunID);
            urunekle.Parameters.Add("@miktar", miktar);
            urunekle.Parameters.Add("@fiyat", toplamfiyat);

            int sonuc = urunekle.ExecuteNonQuery();

            if (sonuc > 0)
            {
                MessageBox.Show("Ürün hesaba eklendi");
            }
            else
            {
                MessageBox.Show("Ürün eklenemedi");
            }
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SELECT *FROM Musteriler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;

            SqlDataAdapter da2 = new SqlDataAdapter();
            da2.SelectCommand = new SqlCommand("SELECT *FROM Masalar", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dg_2.ItemsSource = dt2.DefaultView;
            // Ürünü stoktan düşücez !
            SqlCommand UrunStokDus = new SqlCommand("UPDATE UrunHareketleri SET Stok = Stok - @miktar WHERE UrunID=@urunid", baglanti);
            UrunStokDus.Parameters.AddWithValue("@miktar", miktar);
            UrunStokDus.Parameters.AddWithValue("@urunid", urunID);
            UrunStokDus.ExecuteNonQuery();
            // Ürünler combobox güncelle !
            SqlDataAdapter da5 = new SqlDataAdapter();
            da5.SelectCommand = new SqlCommand("SELECT u.UrunID,u.UrunAdi FROM Urunler u WHERE u.UrunID IN(SELECT UrunID FROM UrunHareketleri GROUP BY UrunID HAVING SUM(Stok)>0)", baglanti);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);

            ComboBox_3.ItemsSource = dt5.DefaultView;
            ComboBox_3.DisplayMemberPath = "UrunAdi";
            ComboBox_3.SelectedValuePath = "UrunID";

        }

        private void Kullanıcılar_Button_Click(object sender, RoutedEventArgs e)
        {
            Kullanıcılar kullanicilar = new Kullanıcılar();
            kullanicilar.Show();
      
        }

        private void MasaOlustur_Button_Click(object sender, RoutedEventArgs e)
        {
            MasalarEkrani masalar = new MasalarEkrani();
            masalar.Show();

        }

        private void HesapOdeme_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dg_2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen önce bir masa seçiniz.");
                return;
            }
            DataRowView secilenmasa = (DataRowView)dg_2.SelectedItem;
            object secilenmasaob = secilenmasa["MasaID"];
            int masaID = Convert.ToInt32(secilenmasaob);
            SqlCommand adisyonid = new SqlCommand("SELECT TOP 1 AdisyonID FROM Adisyon WHERE MasaID = @masaID AND Durum = 'Acik' ORDER BY AdisyonID DESC", baglanti);
            adisyonid.Parameters.AddWithValue("@masaID", masaID);

            object adisyonIDob = adisyonid.ExecuteScalar();
            if (adisyonIDob == null || adisyonIDob == DBNull.Value)
            {
                MessageBox.Show("Bu masa için açık bir adisyon bulunamadı.");
                return;
            }
            int adisyonID = Convert.ToInt32(adisyonIDob);

            if (adisyonID == 0)
            {
                MessageBox.Show("Bu masa için açık bir adisyon bulunamadı.");
                return;
            }
            else
            {
                OdemeEkrani hesapodeme = new OdemeEkrani(adisyonID);
                hesapodeme.Show();
                

            }
            

        }

        private void Urunler_Button_Click(object sender, RoutedEventArgs e)
        {
            Urunler urunform = new Urunler();
            urunform.Show();
        }

        private void ComboBox_3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UrunStok_ComboBox.Items.Clear();
            int urunid = Convert.ToInt32(ComboBox_3.SelectedValue);
            SqlCommand urunStokcomm = new SqlCommand("SELECT Stok FROM UrunHareketleri WHERE UrunID = @urunid", baglanti);
            urunStokcomm.Parameters.AddWithValue("@urunid", urunid);
            int urunstok = Convert.ToInt32(urunStokcomm.ExecuteScalar());
                for (int i = 1; i <= urunstok; i++)
                {
                    UrunStok_ComboBox.Items.Add(i);
                }
            
            
        }

        private void IrsaliyeGir_Button_Click(object sender, RoutedEventArgs e)
        {
            IrsaliyeHareket irsaliyegir = new IrsaliyeHareket();
            irsaliyegir.Show();
        }

        private void StokDuzenle_Button_Click(object sender, RoutedEventArgs e)
        {
            StokDuzenleme stokduzen = new StokDuzenleme();
            stokduzen.Show();
        }

        private void Raporlar_Button_Click(object sender, RoutedEventArgs e)
        {
            Raporlar raporgoster = new Raporlar();
            raporgoster.Show();
        }

        private void MasalariYenile_Button_Click(object sender, RoutedEventArgs e)
        {
            
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand("SELECT *FROM Musteriler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;

            SqlDataAdapter da2 = new SqlDataAdapter();
            da2.SelectCommand = new SqlCommand("SELECT *FROM Masalar", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dg_2.ItemsSource = dt2.DefaultView;

            masaComboBoxgoster();

            SqlDataAdapter da4 = new SqlDataAdapter();
            da4.SelectCommand = new SqlCommand("SELECT MusteriID FROM Musteriler", baglanti);
            DataTable dt4 = new DataTable();
            da4.Fill(dt4);
            ComboBox_2.DisplayMemberPath = "MusteriID";
            ComboBox_2.ItemsSource = dt4.DefaultView;

            SqlDataAdapter da5 = new SqlDataAdapter();
            da5.SelectCommand = new SqlCommand("SELECT u.UrunID,u.UrunAdi FROM Urunler u WHERE u.UrunID IN(SELECT UrunID FROM UrunHareketleri GROUP BY UrunID HAVING SUM(Stok)>0)", baglanti);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);

            ComboBox_3.ItemsSource = dt5.DefaultView;
            ComboBox_3.DisplayMemberPath = "UrunAdi";
            ComboBox_3.SelectedValuePath = "UrunID";
        }
    }
}
