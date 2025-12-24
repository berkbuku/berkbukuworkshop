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
using System.Windows.Data;

namespace WpfApp1
{
    /// <summary>
    /// Raporlar.xaml etkileşim mantığı
    /// </summary>
    public partial class Raporlar : Window
    {
        
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");

        public void sonuclariGoster()
        {
            DateTime aybasi = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime aysonu = aybasi.AddMonths(1);
            decimal gidertutar = 0;
            decimal gelirtutar = 0;
            decimal sonuc;
            SqlCommand giderHesaplacomm = new SqlCommand("SELECT SUM(ToplamTutar) FROM IrsaliyeHareket WHERE Zaman>=@ayinbasi AND Zaman<@ayinsonu", baglanti);
            giderHesaplacomm.Parameters.AddWithValue("@ayinbasi", SqlDbType.DateTime).Value = aybasi;
            giderHesaplacomm.Parameters.AddWithValue("@ayinsonu", SqlDbType.DateTime).Value = aysonu;
            object gidertutarob = giderHesaplacomm.ExecuteScalar();
            if (gidertutarob != DBNull.Value)
            {
                gidertutar = Convert.ToDecimal(gidertutarob);
                Gider_TextBlock.Text = string.Format(new CultureInfo("tr-TR"), "{0:C2}", gidertutar);
            }
            else
            {
                MessageBox.Show("İrsaliye kaydı bulunamadı!");
            }
            SqlCommand gelirHesaplacomm = new SqlCommand("SELECT SUM(Miktar) FROM Odemeler WHERE Tarih>=@aybasi AND Tarih<@aysonu", baglanti);
            gelirHesaplacomm.Parameters.AddWithValue("@aybasi", SqlDbType.DateTime).Value = aybasi;
            gelirHesaplacomm.Parameters.AddWithValue("@aysonu", SqlDbType.DateTime).Value = aysonu;
            object gelirtutarob = gelirHesaplacomm.ExecuteScalar();
            if (gelirtutarob != DBNull.Value)
            {
                gelirtutar = Convert.ToDecimal(gelirtutarob);
                Gelir_TextBlock.Text = string.Format(new CultureInfo("tr-TR"), "{0:C2}", gelirtutar);


            }
            else
            {
                MessageBox.Show("Alınan ödeme kaydı bulunamadı!");
            }
            sonuc = gelirtutar - gidertutar;
            Sonuc_TextBlock.Text = string.Format(new CultureInfo("tr-TR"), "{0:C2}", sonuc);



        }

        public void irsaliyeGoster()
        {
            DateTime aybasi = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime aysonu = aybasi.AddMonths(1);
            SqlCommand irsaliyeComm = new SqlCommand("SELECT *FROM IrsaliyeHareket WHERE Zaman>=@ayinbasi AND Zaman<@ayinsonu", baglanti);
            irsaliyeComm.Parameters.AddWithValue("@ayinbasi", SqlDbType.DateTime).Value = aybasi;
            irsaliyeComm.Parameters.AddWithValue("@ayinsonu", SqlDbType.DateTime).Value = aysonu;
            SqlDataAdapter da = new SqlDataAdapter(irsaliyeComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Irsaliye_dg.ItemsSource = dt.DefaultView;

        }

        public void odemelerGoster()
        {
            SqlCommand odemelerComm = new SqlCommand("SELECT *FROM Odemeler", baglanti);
            SqlDataAdapter da = new SqlDataAdapter(odemelerComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Odemeler_dg.ItemsSource = dt.DefaultView;
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

        private void Odemeler_dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Miktar") // kolon adı veritabanından gelen property adıyla aynı olmalı
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
        public Raporlar()
        {
            InitializeComponent();
            baglanti.Open();
            irsaliyeGoster();
            odemelerGoster();
            sonuclariGoster();
            

        }
    }
}
