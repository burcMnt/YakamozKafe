using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YakamozKafe.Data
{
    public class Siparis
    {
        public int MasaNo { get; set; }
        public SiparisDurum Durum { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; } = new List<SiparisDetay>();
        public decimal OdenenTutar { get; set; }
        public DateTime? AcilisZamani { get; set; } = DateTime.Now;
        public DateTime? KapanisZamani { get; set; }

        public string ToplamTutarTL => ToplamTutar().ToString("₺0.00");

        public decimal ToplamTutar()
        {
            //decimal toplam= 0;
            //foreach (SiparisDetay item in SiparisDetaylar)
            //{
            //    toplam += item.Tutar();
            //}
            //return toplam;

            return SiparisDetaylar.Sum(x => x.Tutar());
        }
    }
}
