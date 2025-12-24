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
    /// MasalarEkrani.xaml etkileşim mantığı
    /// </summary>
    public partial class MasalarEkrani : Window
    {
        SqlConnection baglanti = new SqlConnection(@"Data Source=RAPORZEN\MSSQLSERVERST;Initial Catalog=BERK;User ID=sa;Password=sapass_1");
        public void goster()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Masalar", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dg_1.ItemsSource = dt.DefaultView;
        }
        public MasalarEkrani()
        {
            baglanti.Open();
            InitializeComponent();
            goster();
        }

        private void Ekle_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(MasaAdi_TxtBox.Text) || !string.IsNullOrEmpty(MasaID_TxtBox.Text))
                {
                    SqlCommand masaekle = new SqlCommand("INSERT INTO Masalar (MasaID,MasaAdi) VALUES (@masaid,@masaadi)", baglanti);
                    masaekle.Parameters.AddWithValue("@masaid", Convert.ToInt32(MasaID_TxtBox.Text));
                    masaekle.Parameters.AddWithValue("@masaadi", MasaAdi_TxtBox.Text);
                    masaekle.ExecuteNonQuery();
                    goster();
                }
                else
                {
                    MessageBox.Show("Lütfen önce masa adı ve masa ID numarası giriniz!");
                }
            }catch(FormatException ex)
            {
                MessageBox.Show("Lütfen MasaID kısmını tamsayı değerleri ile doldurunuz!");
            }
            catch (OverflowException ex) {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }catch(SqlException ex)
            {
                MessageBox.Show("Girmek istediğiniz değer çok uzun!");
            }
         
        }

        private void Sil_Button_Click(object sender, RoutedEventArgs e)
        {
           
            if (dg_1.SelectedItem== null) {
                MessageBox.Show("Lütfen bir masa seçiniz !");
            }
            else {
                SqlCommand masasil = new SqlCommand("DELETE FROM Masalar WHERE MasaID=@masaid", baglanti);
                DataRowView secilenmasa = (DataRowView)dg_1.SelectedItem;
                object secilenmasaidob = secilenmasa["MasaID"];
                int secilenmasaid = Convert.ToInt32(secilenmasaidob);
                masasil.Parameters.AddWithValue("@masaid", secilenmasaid);
                masasil.ExecuteNonQuery();
                goster();

            }
            
            
        }
    }
}
