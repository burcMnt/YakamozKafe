using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using YakamozKafe.Data;

namespace YakamozKafe.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db = new KafeVeri();
        public AnaForm()
        {
           // OrnekUrunleriEkle();
            VerileriOku();
            InitializeComponent();
            Icon = Resources.starfruit_96816;
            masalarImageList.Images.Add("bos", Resources.dining_room);
            masalarImageList.Images.Add("dolu", Resources.meeting);
            MasalariOlustur();

        }

        private void VerileriOku()
        {
            //veri oku ve deserilize et
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonSerializer.Deserialize<KafeVeri>(json);
            }
            catch (Exception)
            {

                db = new KafeVeri();
                OrnekUrunleriEkle();
            }
           
        }

        private void OrnekUrunleriEkle()
        {
            db.Urunler.Add(new Urun() { UrunAd = "Çay", BirimFiyat = 4.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Açma", BirimFiyat = 5.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Poğaça", BirimFiyat = 5.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Kahve", BirimFiyat = 17.00m });
        }

        private void MasalariOlustur()
        {
            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem();
                lvi.Tag = i;
                lvi.Text = "Masa" + i;
                lvi.ImageKey = MasaDoluMu(i) ? "dolu" : "bos";
                lvwMasalar.Items.Add(lvi);
            }
        }

        private bool MasaDoluMu(int masaNo)
        {
            // return db.AktifSiparisler.Any(x => x.MasaNo == masaNo);

            foreach (var item in db.AktifSiparisler)
            {
                if (item.MasaNo == masaNo)
                {
                    return true;
                }
            }
            return false;
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int)lvi.Tag;
            lvi.ImageKey = "dolu";

            //siprariş oluştur

            Siparis siparis = SiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }

            //bu siparişi başka formda aç

            SiparisForm siparisForm = new SiparisForm(db, siparis);
            siparisForm.MasaTasindi += SiparisForm_MasaTasindi;
            siparisForm.ShowDialog();

            //Sipariş durum kontrolu

            if (siparis.Durum != SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
            }


        }

        private void SiparisForm_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                int masaNo = (int)lvi.Tag;
                if (masaNo==e.EskiMasaNo)
                {
                    lvi.ImageKey = "bos";
                }
                else if (masaNo==e.YeniMasaNo)
                {
                    lvi.ImageKey = "dolu";
                }
            }
        }

        private Siparis SiparisBul(int masaNo)
        {
            //foreach (Siparis item in db.AktifSiparisler)
            //{
            //    if (item.MasaNo==masaNo)
            //    {
            //        return item;
            //    }
            //}
            //return null;
            return db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == tsmiUrunler)
            {
                new UrunlerForm(db).ShowDialog();
            }
            else if (e.ClickedItem == tsmiGecmisSiparisler)
            {
                new GecmisSiparislerForm(db).ShowDialog();
            }

        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }

        private void VerileriKaydet()
        {
            var options = new JsonSerializerOptions() { WriteIndented = true }; 
            string json = JsonSerializer.Serialize(db,options);
            File.WriteAllText("veri.json", json);
        }
    }
}
