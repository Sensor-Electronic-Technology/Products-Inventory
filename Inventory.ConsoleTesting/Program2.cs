using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.DataLayer;
using Inventory.Common.DataLayer.Providers;
using Console_Table;
using System.Reflection;
using Inventory.Common.DataLayer.Services;
using System.Dynamic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Cryptography;
using System.IO;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;
using System.Linq.Expressions;
using Inventory.Common.DataLayer.EntityDataManagers;
using Inventory.Common.DataLayer.EntityOperations;
using System.Data.Entity;

namespace Inventory.ConsoleTesting {
    public class Program2 {
        private static void Main(string[] args) {
            using(var context=new InventoryContext()) {
                Part part = new Part("", "");

                Distributor distributor = new Distributor();
                distributor.Name = "LSP Industrial Ceramics Inc.";
                distributor.Description = "Boron Nitride Parts";
                
                Price price = new Price();
                price.Distributor = distributor;
                price.DistributorName = distributor.Name;
                price.Amount = 362;
                price.MinOrder = 1;
                price.LeadTime = 28;

            }
        }
    }
}
