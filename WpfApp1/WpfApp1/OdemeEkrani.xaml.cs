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
using System.Globalization;

namespace WpfApp1
{
    /// <summary>
    /// OdemeEkrani.xaml etkileşim mantığı
    /// </summary>
    /// 

    class SepetItem
    {
        public string UrunAdi { get; set; }
        public int Miktar { get; set; }
        public string OdemeTuru { get; set; }
    }
    public partial class OdemeEkrani : Window
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        int adisyonID;
        List<SepetItem> sepetlistesi = new List<SepetItem>();
        decimal toplammiktar;
        decimal odenenmiktar;
        int adisyonidd;
        public OdemeEkrani(int gelenadisyonID)
        {
            InitializeComponent();
            baglanti.Open();
            adisyonID = gelenadisyonID;
            odenenmiktar = 0;
            goster();
            OdemeSecenekleri_ComboBox_Copy.Items.Add("Nakit");
            OdemeSecenekleri_ComboBox_Copy.Items.Add("Kredi Karti");
            adisyonidd = gelenadisyonID;
            SqlCommand urunleriyukle = new SqlCommand("SELECT DISTINCT Urunler.UrunAdi FROM AdisyonHareketleri INNER JOIN Urunler ON AdisyonHareketleri.UrunID=Urunler.UrunID WHERE AdisyonHareketleri.AdisyonID = @id",baglanti);
            urunleriyukle.Parameters.AddWithValue("@id", adisyonID);
            SqlDataReader dr = urunleriyukle.ExecuteReader();
            while (dr.Read())
            {
                UrunAdi_ComboBox.Items.Add(dr["UrunAdi"].ToString());
            }
            dr.Close();

            OdemeSecenekleri_ComboBox.Items.Add("Nakit");
            OdemeSecenekleri_ComboBox.Items.Add("Kredi Karti");

            
            SqlCommand toplamBulcomm = new SqlCommand("SELECT SUM(Fiyat) FROM AdisyonHareketleri WHERE AdisyonID=@adisyonid", baglanti);
            toplamBulcomm.Parameters.AddWithValue("@adisyonid", gelenadisyonID);
            toplammiktar = Convert.ToDecimal(toplamBulcomm.ExecuteScalar());
            Toplam_TextBlock.Text = "Toplam Miktar:"+string.Format(new CultureInfo("tr-TR"), "{0:C2}", toplammiktar);
            sepetlistesi.Clear();
            Sepet_ListBox.Items.Clear();
            

                }
        public void goster()
        {
            SqlCommand adisyonHBul = new SqlCommand("SELECT ah.HareketID, u.UrunAdi, ah.Miktar, ah.Fiyat FROM AdisyonHareketleri ah INNER JOIN Urunler u ON ah.UrunID = u.UrunID WHERE ah.AdisyonID = @adisyonID", baglanti);
            adisyonHBul.Parameters.AddWithValue("@adisyonID", adisyonID);
            SqlDataAdapter da = new SqlDataAdapter(adisyonHBul);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Masalar_Grid.ItemsSource = dt.DefaultView;
        }
        
        private decimal FiyatGetir(string urunAdi)
        {
            SqlCommand cmd = new SqlCommand(@"SELECT Fiyat FROM Urunler WHERE UrunAdi=@urunAdi", baglanti);
            cmd.Parameters.AddWithValue("@urunAdi", urunAdi);

            object sonuc = cmd.ExecuteScalar();

            if (sonuc != null && sonuc != DBNull.Value)
                return Convert.ToDecimal(sonuc);
            else
                return 0;
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

        private void UrunAdi_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UrunMiktari_ComboBox.Items.Clear();
            string secilenurun = UrunAdi_ComboBox.SelectedItem.ToString();
            SqlCommand miktar = new SqlCommand("SELECT SUM(Miktar) FROM AdisyonHareketleri INNER JOIN Urunler ON AdisyonHareketleri.UrunID=Urunler.UrunID WHERE AdisyonHareketleri.AdisyonID = @id AND Urunler.UrunAdi=@urun", baglanti);
            miktar.Parameters.AddWithValue("@id", adisyonID);
            miktar.Parameters.AddWithValue("@urun", secilenurun);

            int maxmiktar = Convert.ToInt32(miktar.ExecuteScalar());
            for(int i=1;i<=maxmiktar;i++)
            {
                UrunMiktari_ComboBox.Items.Add(i);
            }
        }

        private void UrunOnayla_Button_Click(object sender, RoutedEventArgs e)
        {
            if (UrunAdi_ComboBox.SelectedItem == null || UrunMiktari_ComboBox.SelectedItem == null || OdemeSecenekleri_ComboBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen ürün, miktar ve ödeme türünü seçiniz.");
                return;
            }
            string urun = UrunAdi_ComboBox.SelectedItem.ToString();
            int miktar = Convert.ToInt32(UrunMiktari_ComboBox.SelectedItem);
            string odemeTuru = OdemeSecenekleri_ComboBox.SelectedItem.ToString();
            var mevcut = sepetlistesi.FirstOrDefault(x => x.UrunAdi == urun);
            int toplamMiktar = miktar + (mevcut != null ? mevcut.Miktar : 0);
            SqlCommand miktarbul = new SqlCommand("SELECT SUM(Miktar) FROM AdisyonHareketleri INNER JOIN Urunler ON AdisyonHareketleri.UrunID=Urunler.UrunID WHERE AdisyonHareketleri.AdisyonID =@id AND Urunler.UrunAdi = @urun", baglanti);
            miktarbul.Parameters.AddWithValue("@id", adisyonID);
            miktarbul.Parameters.AddWithValue("@urun", urun);
            int adisyondakimiktar = Convert.ToInt32(miktarbul.ExecuteScalar());
            if (toplamMiktar > adisyondakimiktar)
            {
                MessageBox.Show($"Bu üründen adisyonda en fazla {adisyondakimiktar} adet var. Eklemek istediğiniz miktar fazla.");
                return;
            }
            if (mevcut != null)
            {
                mevcut.Miktar += miktar;

                
                int index = Sepet_ListBox.Items.IndexOf($"{mevcut.Miktar - miktar} x {urun} ({odemeTuru})");
                Sepet_ListBox.Items[index] = $"{mevcut.Miktar} x {urun} ({odemeTuru})";
            }
            else
            {
                sepetlistesi.Add(new SepetItem
                {
                    UrunAdi = urun,
                    Miktar = miktar,
                    OdemeTuru = odemeTuru
                });

                Sepet_ListBox.Items.Add($"{miktar} x {urun} ({odemeTuru})");
            }
            decimal toplam = sepetlistesi.Sum(x =>
            {
                decimal fiyat = FiyatGetir(x.UrunAdi); 
                return fiyat * x.Miktar;
            });
            labelToplamTutar.Content = toplam.ToString("C", new CultureInfo("tr-TR"));


        }

        private void OdemeAl_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sepetlistesi.Count == 0)
            {
                MessageBox.Show("Sepet boş. Önce ürün ve ödeme türü ekleyin.");
                return;
            }



            foreach (var item in sepetlistesi)
            {

                SqlCommand urunIDBul = new SqlCommand("SELECT UrunID FROM Urunler WHERE UrunAdi = @urun", baglanti);
                urunIDBul.Parameters.AddWithValue("@urun", item.UrunAdi);
                int urunID = Convert.ToInt32(urunIDBul.ExecuteScalar());


                SqlCommand fiyatBul = new SqlCommand("SELECT Fiyat FROM Urunler WHERE UrunID = @id", baglanti);
                fiyatBul.Parameters.AddWithValue("@id", urunID);
                decimal birimFiyat = Convert.ToDecimal(fiyatBul.ExecuteScalar());


                SqlCommand odemeEkle = new SqlCommand("INSERT INTO Odemeler (AdisyonID ,Miktar, OdemeTuru) VALUES (@adisyonID, @miktar, @odemeTuru)", baglanti);
                odemeEkle.Parameters.AddWithValue("@adisyonID", adisyonID);
                odemeEkle.Parameters.AddWithValue("@miktar", birimFiyat * item.Miktar);
                odemeEkle.Parameters.AddWithValue("@odemeTuru", item.OdemeTuru);
                odemeEkle.ExecuteNonQuery();

                int odenecekMiktar = item.Miktar;
                SqlCommand hareketlerCmd = new SqlCommand(@"SELECT HareketID, Miktar FROM AdisyonHareketleri WHERE AdisyonID = @adisyonID AND UrunID = @urunID ORDER BY HareketID", baglanti);
                hareketlerCmd.Parameters.AddWithValue("@adisyonID", adisyonID);
                hareketlerCmd.Parameters.AddWithValue("@urunID", urunID);

                var reader = hareketlerCmd.ExecuteReader();
                List<(int HareketID, int Miktar)> hareketler = new List<(int HareketID, int Miktar)>();


                while (reader.Read())
                {
                    hareketler.Add((reader.GetInt32(0), reader.GetInt32(1)));
                }
                reader.Close();

                // FIFO düşüm işlemi
                int kalan = odenecekMiktar;
                foreach (var hareket in hareketler)
                {
                    if (kalan <= 0)
                        break;

                    int dusulecek = Math.Min(kalan, hareket.Miktar);

                    SqlCommand guncelle = new SqlCommand(@"UPDATE AdisyonHareketleri SET Miktar = Miktar - @miktar WHERE HareketID = @hareketID", baglanti);
                    guncelle.Parameters.AddWithValue("@miktar", dusulecek);
                    guncelle.Parameters.AddWithValue("@hareketID", hareket.HareketID);
                    guncelle.ExecuteNonQuery();

                    // Eğer miktar 0'a düşerse satırı sil
                    SqlCommand sil = new SqlCommand(@"DELETE FROM AdisyonHareketleri WHERE HareketID = @hareketID AND Miktar <= 0", baglanti);
                    sil.Parameters.AddWithValue("@hareketID", hareket.HareketID);
                    sil.ExecuteNonQuery();

                    kalan -= dusulecek;
                }
            }
                     Sepet_ListBox.Items.Clear();
                     sepetlistesi.Clear();
                     MessageBox.Show("Ödeme başarıyla alındı.");


            SqlCommand hesapkontrol = new SqlCommand("SELECT COUNT(*) FROM AdisyonHareketleri WHERE AdisyonID = @adisyonID", baglanti);
                    hesapkontrol.Parameters.AddWithValue("@adisyonID", adisyonID);
                    int hesapkontrolson = Convert.ToInt32(hesapkontrol.ExecuteScalar());
                    if (hesapkontrolson == 1)
                    {
                        int musteriID;
                        SqlCommand musteribul = new SqlCommand("SELECT MusteriID FROM Adisyon WHERE AdisyonID=@adisyonid", baglanti);
                        musteribul.Parameters.AddWithValue("@adisyonid", adisyonID);
                        object musteriidob = musteribul.ExecuteScalar();
                        musteriID = Convert.ToInt32(musteriidob);
                        SqlCommand musterikapat = new SqlCommand("DELETE FROM Musteriler WHERE MusteriID=@musteriid", baglanti);
                        musterikapat.Parameters.AddWithValue("@musteriid", musteriID);
                        musterikapat.ExecuteNonQuery();
                        MessageBox.Show("Hesap kapanmıştır!");
                        this.Close();


                    }
                
            goster();
            


        }

        private void MiktarOdemeAl_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Miktar_TxtBox.Text != null && OdemeSecenekleri_ComboBox_Copy.SelectedItem != null)
                {
                    decimal odememiktari = Convert.ToDecimal(Miktar_TxtBox.Text);

                    odenenmiktar += odememiktari;
                    decimal kalanmiktar = toplammiktar - odenenmiktar;
                    Odenen_TextBlock.Text = "Ödenen Miktar:" + string.Format(new CultureInfo("tr-TR"), "{0:C2}", odenenmiktar);
                    Kalan_TextBlock.Text = "Kalan Miktar:" + string.Format(new CultureInfo("tr-TR"), "{0:C2}", kalanmiktar);
                    SqlCommand odemeEklecomm = new SqlCommand("INSERT INTO Odemeler(AdisyonID,Miktar,OdemeTuru) VALUES (@adisyonid,@miktar,@odemeturu)", baglanti);
                    odemeEklecomm.Parameters.AddWithValue("@adisyonid", adisyonID);
                    odemeEklecomm.Parameters.AddWithValue("@miktar", odememiktari);
                    odemeEklecomm.Parameters.AddWithValue("@odemeturu", OdemeSecenekleri_ComboBox_Copy.SelectedValue.ToString());
                    odemeEklecomm.ExecuteNonQuery();
                    if (kalanmiktar == 0)
                    {
                        MessageBox.Show("Hesap kapanmıştır.");
                        int musteriID;
                        SqlCommand musteribul = new SqlCommand("SELECT MusteriID FROM Adisyon WHERE AdisyonID=@adisyonid", baglanti);
                        musteribul.Parameters.AddWithValue("@adisyonid", adisyonID);
                        object musteriidob = musteribul.ExecuteScalar();
                        musteriID = Convert.ToInt32(musteriidob);
                        SqlCommand musterikapat = new SqlCommand("DELETE FROM Musteriler WHERE MusteriID=@musteriid", baglanti);
                        musterikapat.Parameters.AddWithValue("@musteriid", musteriID);
                        musterikapat.ExecuteNonQuery();
                        this.Close();

                    }
                    else if (kalanmiktar < 0)
                    {
                        int paraustu = Convert.ToInt32(kalanmiktar);
                        decimal paraustudec = -kalanmiktar;
                        MessageBox.Show($"Para üstü {-paraustu} TL'dir! Hesap kapanmıştır.");
                        SqlCommand paraustuekle = new SqlCommand("UPDATE Odemeler SET ParaUstu=@paraustu WHERE Id=(SELECT MAX(Id) FROM Odemeler)",baglanti);
                        paraustuekle.Parameters.AddWithValue("@paraustu", paraustudec);
                        paraustuekle.ExecuteNonQuery();
                        int musteriID;
                        SqlCommand musteribul = new SqlCommand("SELECT MusteriID FROM Adisyon WHERE AdisyonID=@adisyonid", baglanti);
                        musteribul.Parameters.AddWithValue("@adisyonid", adisyonID);
                        object musteriidob = musteribul.ExecuteScalar();
                        musteriID = Convert.ToInt32(musteriidob);
                        SqlCommand musterikapat = new SqlCommand("DELETE FROM Musteriler WHERE MusteriID=@musteriid", baglanti);
                        musterikapat.Parameters.AddWithValue("@musteriid", musteriID);
                        musterikapat.ExecuteNonQuery();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Ödeme yöntemi seçilmedi veya ödenecek miktar girilmedi!");
                }
            }catch(OverflowException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }catch(FormatException ex)
            {
                MessageBox.Show("Hatalı değer girdiniz!");
            }catch (SqlException ex)
{
    MessageBox.Show("Girmek istediğiniz değer çok uzun!");
}

        }


    }

    }


