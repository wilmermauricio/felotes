namespace DGII.FE.XML.Models.Interno
{
    public class RetencionInterno
    {
        private decimal? _montoITBISRetenido;
        private decimal? _montoISRRetenido;

        public int? IndicadorAgenteRetencionoPercepcion { get; set; }
        public decimal? MontoITBISRetenido
        {
            get
            {
                return _montoITBISRetenido == null ? null :
                  _montoITBISRetenido.GetValueOrDefault();
            }
            set
            {
                _montoITBISRetenido = value;
            }
        }
        public decimal? MontoISRRetenido
        {
            get
            {
                return _montoISRRetenido == null ? null :
                   _montoISRRetenido.GetValueOrDefault();
            }
            set
            {
                _montoISRRetenido = value;
            }
        }
    }
}
