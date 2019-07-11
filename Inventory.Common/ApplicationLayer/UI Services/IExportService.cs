using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.XtraPrinting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace Inventory.Common.ApplicationLayer.Services {
    public enum ExportFormat {Xlsx,Pdf,Csv}

    public interface IExportService {
        void Export(Stream stream, ExportFormat format,XlsxExportOptionsEx options=null);
    }
}
