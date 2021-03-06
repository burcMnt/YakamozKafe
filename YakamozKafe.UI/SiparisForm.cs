using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YakamozKafe.Data;

namespace YakamozKafe.UI
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi;
        private readonly KafeVeri _db;
        private readonly Siparis _siparis;
        private readonly BindingList<SiparisDetay> _blSiparisDetaylar;
        public SiparisForm(KafeVeri kafeveri, Siparis siparis)
        {
            _db = kafeveri;
            _siparis = siparis;
            _blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            dgvSiparisDetay.AutoGenerateColumns = false;
            UrunleriGoster();
            EkleFormSifirla();
            MasaNoGuncelle();
            FiyatGuncelle();
            DetaylariListele();
            MasaNolariDoldur();
            _blSiparisDetaylar.ListChanged += _blSiparisDetaylar_ListChanged;
        }

        private void MasaNolariDoldur()
        {
            //List<int> bosMasaNolar = new List<int>();
            //for (int i = 1; i <= _db.MasaAdet; i++)
            //{
            //    if (!_db.AktifSiparisler.Any(x=> x.MasaNo==i))
            //    {
            //        bosMasaNolar.Add(i);
            //    }
            //}
            //cboMasaNo.DataSource = bosMasaNolar;

            cboMasaNo.DataSource = Enumerable
                .Range(1, 20)
                .Where(i => !_db.AktifSiparisler.Any(x => x.MasaNo == i))
                .ToList();


        }

        private void _blSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            FiyatGuncelle();
        }

        private void DetaylariListele()
        {
            dgvSiparisDetay.DataSource = _blSiparisDetaylar;
        }

        private void UrunleriGoster()
        {
            cboUrun.DataSource = _db.Urunler;
        }

        private void FiyatGuncelle()
        {
            lblOdemeTutar.Text = _siparis.ToplamTutarTL;
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa{_siparis.MasaNo} Sipariş Bilgileri";
            lblMasaNo.Text = _siparis.MasaNo.ToString("00");
        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedIndex == -1 || nudAdet.Value < 1) return;
            Urun urun = (Urun)cboUrun.SelectedItem;
            SiparisDetay siparisDetay = new SiparisDetay()
            {
                UrunAd = urun.UrunAd,
                BirimFiyat = urun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };
            _blSiparisDetaylar.Add(siparisDetay);

            EkleFormSifirla();
        }

        private void EkleFormSifirla()
        {
            cboUrun.SelectedIndex = -1;
            nudAdet.Value = 1;
        }

        private void btnİptal_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Iptal, 0);
        }

        private void SiparisKapat(SiparisDurum siparisDurum, decimal odenenTutar)
        {
            _siparis.OdenenTutar = odenenTutar;
            _siparis.Durum = siparisDurum;
            _siparis.KapanisZamani = DateTime.Now;
            _db.AktifSiparisler.Remove(_siparis);
            _db.GecmisSiparisler.Add(_siparis);
            Close();
        }

        private void btnOde_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Odendi, _siparis.ToplamTutar());
        }

        private void btnTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedIndex == -1) return;
            {
                int eskiMasaNo = _siparis.MasaNo;
                int yeniMasaNo = (int)cboMasaNo.SelectedItem;
                _siparis.MasaNo = yeniMasaNo;
                MasaNolariDoldur(); //dolu masalar değişti.

                if (MasaTasindi != null)
                {
                    MasaTasindi(this, new MasaTasindiEventArgs(eskiMasaNo, yeniMasaNo));
                }
                MasaNoGuncelle();
            }
        }

        private void dgvSiparisDetay_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Seçili sipariş detayları silinecektir.Emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
            e.Cancel = dr == DialogResult.No;
        }
    }
}
