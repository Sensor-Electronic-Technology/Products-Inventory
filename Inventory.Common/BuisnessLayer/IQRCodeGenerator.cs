using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;

namespace Inventory.Common.BuisnessLayer {
    public interface IQRCodeGenerator {
        string GenerateQRCode(ProductInstance product);
        
    }
}
